import {makeAutoObservable} from "mobx";
import server from "logic/network/server";
import {RoomClosed, GameStarted, RoomUpdated, GameState, TimeoutOptions} from "./events";
import * as moment from "moment";

export type Player = {
    id: string;
    name: string;
    isMafia: boolean;
}

export type Secret = {
    text: string;
}

export enum GameStateType {
    NotStarted,
    NewRound,
    ShowSecret,
    Voting,
    ShowRoundResult,
    WinMafia,
    WinPlayers
}

export class TimerInfo {
    durationSec: number;
    started: number;


    constructor(durationSec: number) {
        makeAutoObservable(this, undefined, {deep: true, autoBind: true})

        this.durationSec = durationSec;
        this.started = Date.now();
    }
}

export class GameTimer {
    options?: {
        newRoundTimeout: Date,
        showSecretTimeout: Date,
        votingTimeout: Date,
        showRoundResultTimeout: Date,
    };
    current?: TimerInfo;


    constructor() {
        makeAutoObservable(this, undefined, {deep: true, autoBind: true})
    }


    updateTimeouts(options: TimeoutOptions, stateType: GameStateType) {
        this.options = {
            newRoundTimeout: moment(options.newRoundTimeout, "hh:mm:ss").toDate(),
            showSecretTimeout: moment(options.showSecretTimeout, "hh:mm:ss").toDate(),
            votingTimeout: moment(options.votingTimeout, "hh:mm:ss").toDate(),
            showRoundResultTimeout: moment(options.showRoundResultTimeout, "hh:mm:ss").toDate(),
        };

        this.refreshTimer(stateType);
    }

    private refreshTimer(stateType: GameStateType) {
        let duration = this.calcDuration(stateType);

        this.current = duration == undefined ? undefined : new TimerInfo(duration);
    }

    private calcDuration(stateType: GameStateType): number | undefined {
        if (stateType == GameStateType.NewRound)
            return this.options.newRoundTimeout.getSeconds();

        if (stateType == GameStateType.ShowSecret)
            return this.options.showSecretTimeout.getSeconds();

        if (stateType == GameStateType.Voting)
            return this.options.votingTimeout.getSeconds();

        if (stateType == GameStateType.ShowRoundResult)
            return this.options.showRoundResultTimeout.getSeconds();


        return undefined;
    }
}

export class Room {
    id: string;
    players: Player[] = [];
    state?: GameState;

    localPlayerName: string;
    timer: GameTimer = new GameTimer();

    constructor(localPlayerName: string) {
        this.localPlayerName = localPlayerName;

        makeAutoObservable(this, undefined, {autoBind: true, deep: true});
    }


    getLocal() {
        return this.players.find(p => p.name === this.localPlayerName);
    }

    getMainText(): string {
        if (this.state?.type == GameStateType.NotStarted)
            return "Ожидание начала игры";

        if (this.state?.type == GameStateType.NewRound) {
            if (!this.getLocal().isMafia)
                return this.state.secret.text;
            else
                return 'Вы лжец. Сделайте то же что и окружающие. Не попадитесь'
        }

        if (this.state?.type == GameStateType.ShowSecret)
            return `Секрет: ${this.state.secret.text}\nОбсудите с другими игроками`

        if (this.state?.type == GameStateType.Voting)
            return 'Проголосуйте';

        if (this.state?.type == GameStateType.ShowRoundResult) {
            if (this.state.selected == null)
                return 'Никого в итоге не выбрали';

            return `${this.state.selected.name} мафия? ${this.state.isMafia ? 'Да' : 'Нет'}`
        }

        if (this.state?.type == GameStateType.WinPlayers)
            return `Победили игроки`

        if (this.state?.type == GameStateType.WinMafia)
            return `Победил лжец`

        return 'Ожидание подключения';
    }

    async addVote(targetId: string) {
        await server.vote(this.id, this.getLocal().id, targetId)
    }
}

function generateFakePlayers() {
    let o: Player[] = [];

    for (let i = 0; i < 20; i++) {
        o.push({
            id: Math.ceil(Math.random() * 10000).toString(),
            name: `Player${Math.ceil(Math.random() * 10000).toString()}`,
            isMafia: false
        })
    }

    return o;
}

export class Game {
    room?: Room;
    localPlayerName: string;
    admin: boolean = false;

    constructor() {
        makeAutoObservable(this, undefined, {autoBind: true});
        server.on('RoomUpdated', this.onRoomUpdated);
        server.on('GameStarted', this.onGameStarted);
        server.on('RoomClosed', this.onRoomClosed);
    }


    setName(username: string) {
        this.localPlayerName = username;
    }

    async createRoom(timeoutOptions: TimeoutOptions, isPublic: boolean) {
        await server.createRoom(this.localPlayerName, timeoutOptions, isPublic);
        this.admin = true;
    }

    async joinRoom(room: string) {
        await server.joinRoom(this.localPlayerName, room);
        this.admin = false;
    }

    async nextState() {
        await server.nextState(this.room.id, this.room.getLocal().id);
    }

    private onRoomUpdated(event: RoomUpdated) {
        if (this.room == null) {
            this.room = new Room(this.localPlayerName);
        }

        this.room.id = event.roomId;
        this.room.players = generateFakePlayers(); // event.players;
        this.room.state = event.state;
        this.room.timer.updateTimeouts(event.timeoutOptions, event.state.type);
    }

    private onGameStarted(event: GameStarted) {
        this.room.players.find(p => p.id == event.mafiaId).isMafia = true;
    }

    private onRoomClosed(event: RoomClosed) {
        this.admin = false;
        this.room = null;
        this.localPlayerName = null;
    }
}


let game = new Game();
(window as any).game = game;
export default game;


/*
let localPlayerName = `fakePlayer_${Math.ceil(Math.random() * 1000)}`;

this.room = new Room(localPlayerName);
this.room.players.push({
    name: localPlayerName,
    isMafia: false,
    id: Math.random().toString()
}, {
    name: `fakePlayer_${Math.ceil(Math.random() * 1000)}`,
    isMafia: false,
    id: Math.random().toString()
});
this.room.secret = 'Ban 12312323';
this.room.id = Math.random().toString()
*/