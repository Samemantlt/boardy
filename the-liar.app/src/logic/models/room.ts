import {makeAutoObservable} from "mobx";
import server from "logic/network/server";
import {GameEnd, GameStarted, RoomUpdated, RoundResultShown, RoundStarted, SecretShown, VotesChanged} from "./events";

export type Player = {
    id: string;
    name: string;
    isMafia: boolean;
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

export enum GameState {
    NotStarted,
    ShowingSecret,
    DiscussingSecret,
    Voting,
    EndVoting,
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
    secret?: string;
    state: GameState = GameState.NotStarted;
    localPlayerName: string;
    timer?: TimerInfo = new TimerInfo(30);

    constructor(localPlayerName: string) {
        this.localPlayerName = localPlayerName;

        makeAutoObservable(this, undefined, {autoBind: true});
    }


    getLocal() {
        return this.players.find(p => p.name === this.localPlayerName);
    }

    getMainText() {
        if (this.state == GameState.NotStarted)
            return "Ожидание начала игры";

        if (this.state == GameState.Voting)
            return 'Проголосуйте';

        if (this.state == GameState.DiscussingSecret)
            return `Секрет: ${this.secret}\nОбсудите с другими игроками`

        if (this.state == GameState.ShowingSecret){
            if (!this.getLocal().isMafia)
                return this.secret;
            else
                return 'Вы лжец. Сделайте то же что и окружающие. Не попадитесь'
        }

        return this.secret;
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
        server.on('RoundStarted', this.onRoundStarted);
        server.on('SecretShown', this.onSecretShown);
        server.on('RoundResultShown', this.onRoundResultShown);
        server.on('VotesChanged', this.onVotesChanged);
        server.on('GameEnd', this.onGameEnd);
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

    async startGame(){
        await server.startGame(this.room.id, this.room.getLocal().id);
    }

    async newRound(){
        await server.newRound(this.room.id, this.room.getLocal().id);
    }

    async endVoting(){
        await server.endVoting(this.room.id, this.room.getLocal().id);
    }

    async startVoting(){
        await server.startVoting(this.room.id, this.room.getLocal().id);
    }

    async showSecret(){
        await server.showSecret(this.room.id, this.room.getLocal().id);
    }

    private onRoomUpdated(event: RoomUpdated) {
        if (this.room == null) {
            this.room = new Room(this.localPlayerName);
            this.room.state = GameState.NotStarted;
        }

        this.room.id = event.roomId;
        this.room.players = event.players;
    }

    private onGameStarted(event: GameStarted) {
        this.room.state = GameState.ShowingSecret;
        this.room.players.find(p => p.id == event.mafiaId).isMafia = true;
    }

    private onRoundStarted(event: RoundStarted) {
        this.room.state = GameState.ShowingSecret;
        this.room.secret = event.secret.text;
    }

    private onSecretShown(event: SecretShown) {
        this.room.state = GameState.DiscussingSecret;
        this.room.secret = event.secret.text;
    }

    private onRoundResultShown(event: RoundResultShown) {
        this.room.state = GameState.EndVoting;
        this.room.secret = `${event.selected?.name ?? 'none'} мафия? ${event.isMafia ? 'Да' : 'Нет'}`;
    }

    private onVotesChanged(event: VotesChanged) {
        this.room.state = GameState.Voting;
    }

    private onGameEnd(event: GameEnd) {
        this.room.state = event.isMafiaWin ? GameState.WinMafia : GameState.WinPlayers;

        if (event.isMafiaWin)
            alert('Победа лжеца');
        else
            alert('Победа игроков');
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