import {makeAutoObservable} from "mobx";
import server from "logic/network/server";
import {RoomClosed, GameStarted, RoomUpdated, GameState} from "./events";

export type Player = {
    id: string;
    name: string;
    isMafia: boolean;
}

export type Secret = {
    text: string;
}

export class AdminControls {
    playerId: string;
    roomId: string;


    constructor(roomId: string, playerId: string) {
        this.roomId = roomId;
        this.playerId = playerId;
    }


    async startGame() {
        await server.startGame(this.roomId, this.playerId);
    }

    async newRound() {
        await server.newRound(this.roomId, this.playerId);
    }

    async showSecret() {
        await server.showSecret(this.roomId, this.playerId);
    }

    async startVoting() {
        await server.startVoting(this.roomId, this.playerId);
    }

    async endVoting() {
        await server.endVoting(this.roomId, this.playerId);
    }
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
        this.durationSec = durationSec;
        this.started = Date.now();
    }
}

export class Room {
    id: string;
    players: Player[] = [];
    state?: GameState;

    localPlayerName: string;
    timer?: TimerInfo = new TimerInfo(30);

    constructor(localPlayerName: string) {
        this.localPlayerName = localPlayerName;

        makeAutoObservable(this, undefined, {autoBind: true, deep: true});
    }


    getLocal() {
        return this.players.find(p => p.name === this.localPlayerName);
    }

    getMainText(): string {
        if (this.state?.name == GameStateType.NotStarted)
            return "Ожидание начала игры";

        if (this.state?.name == GameStateType.NewRound) {
            if (!this.getLocal().isMafia)
                return this.state.secret.text;
            else
                return 'Вы лжец. Сделайте то же что и окружающие. Не попадитесь'
        }

        if (this.state?.name == GameStateType.ShowSecret)
            return `Секрет: ${this.state.secret.text}\nОбсудите с другими игроками`

        if (this.state?.name == GameStateType.Voting)
            return 'Проголосуйте';

        if (this.state?.name == GameStateType.ShowRoundResult)
            return `${this.state.selected.name} мафия? ${this.state.isMafia ? 'Да' : 'Нет'}`

        if (this.state?.name == GameStateType.WinPlayers)
            return `Победили игроки`

        if (this.state?.name == GameStateType.WinMafia)
            return `Победил лжец`

        return 'Ожидание подключения';
    }

    async addVote(targetId: string) {
        await server.vote(this.id, this.getLocal().id, targetId)
    }
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


    async createRoom(username: string) {
        this.localPlayerName = username;
        await server.createRoom(username);
        this.admin = true;
    }

    async joinRoom(username: string, room: string) {
        this.localPlayerName = username;
        await server.joinRoom(username, room);
        this.admin = false;
    }

    async startGame() {
        await server.startGame(this.room.id, this.room.getLocal().id);
    }

    async newRound() {
        await server.newRound(this.room.id, this.room.getLocal().id);
    }

    async endVoting() {
        await server.endVoting(this.room.id, this.room.getLocal().id);
    }

    async startVoting() {
        await server.startVoting(this.room.id, this.room.getLocal().id);
    }

    async showSecret() {
        await server.showSecret(this.room.id, this.room.getLocal().id);
    }

    private onRoomUpdated(event: RoomUpdated) {
        if (this.room == null) {
            this.room = new Room(this.localPlayerName);
        }

        this.room.id = event.roomId;
        this.room.players = event.players;
        this.room.state = event.state;
    }

    private onGameStarted(event: GameStarted) {
        this.room.players.find(p => p.id == event.mafiaId).isMafia = true;
    }

    private onRoomClosed(event: GameStarted) {
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