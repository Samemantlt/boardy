import {HubConnectionBuilder, HubConnection} from "@microsoft/signalr";
import {GameRoom, PublicEvent, PublicEventHandler, TimeoutOptions} from 'logic/models/events';
import {makeAutoObservable} from "mobx";
import removeItem from "../../helpers/array";

export interface IServer {
    connected: boolean;
}
export interface IJoinServer {
    createRoom(username: string, timeoutOptions: TimeoutOptions, isPublic: boolean): Promise<string>;

    joinRoom(username: string, room: string): Promise<string>;

    getPublicRooms(): Promise<GameRoom[]>;
}

export interface IPlayerServer {
    vote(room: string, currentPlayer: string, target: string): Promise<void>;
}

export interface IEventsServer {
    on(eventType: string, handler: PublicEventHandler): () => void;
}

export interface IAdminServer {
    nextState(room: string, currentPlayer: string): Promise<void>;
}

type TypedEventHandler = {
    type: string,
    handler: PublicEventHandler
}


class SignalRServer implements IJoinServer, IAdminServer, IPlayerServer, IEventsServer, IServer {
    connection: HubConnection;
    handlers: TypedEventHandler[] = [];
    connected: boolean = false;

    constructor() {
        makeAutoObservable(this, undefined, {autoBind: true, deep: false});

        this.connection = new HubConnectionBuilder()
            .withUrl("/hub")
            .build();

        this.connection.on('Raise', this.handleEvent);
        this.connection.start().then(_ => this.connected = true, e => {
            console.error('Connecting error', e);
        });
    }


    private handleEvent(event: PublicEvent) {
        console.log('Event received', event);

        for (const handler of this.handlers) {
            try {
                if (event.eventType == handler.type)
                    handler.handler(event.event);
            } catch (e) {
                console.error('Error while handling event', e);
            }
        }
    }

    async createRoom(username: string, timeoutOptions: TimeoutOptions, isPublic: boolean): Promise<string> {
        console.log(`Room created: ${username}`);
        return await this.connection.invoke<string>('CreateRoom', username, timeoutOptions, isPublic);
    }

    async joinRoom(username: string, room: string): Promise<string> {
        console.log(`Game joined: ${room} ${username}`);
        return await this.connection.invoke<string>('JoinRoom', room, username);
    }

    async nextState(room: string, currentPlayer: string): Promise<void> {
        console.log(`nextState: ${room} ${currentPlayer}`);
        await this.connection.invoke('NextState', room, currentPlayer);
    }

    on(eventType: string, handler: PublicEventHandler): () => void {
        console.log(`Subscribed: ${eventType}`, handler)
        this.handlers.push({
            type: eventType,
            handler: handler
        });
        let disposed = false;

        return () => {
            if (disposed)
                return;

            this.handlers = removeItem(this.handlers, this.handlers.find(p => p.handler == handler));
            disposed = true;
        };
    }
    async vote(room: string, currentPlayer: string, target: string): Promise<void> {
        console.log(`Vote: ${room} ${currentPlayer} ${target}`);
        await this.connection.invoke('AddVote', room, currentPlayer, target);
    }

    async getPublicRooms(): Promise<GameRoom[]> {
        console.log(`getPublicRooms`);
        return await this.connection.invoke<GameRoom[]>('GetPublicRooms');
    }
}


const fakeServer: IJoinServer & IAdminServer & IPlayerServer & IEventsServer & IServer = {
    connected: true,
    async createRoom(username: string, timeoutOptions: TimeoutOptions, isPublic: boolean): Promise<string> {
        console.log(`Room created: ${username} ${timeoutOptions} ${isPublic}`);
        return Math.random().toString();
    },
    async joinRoom(username: string, room: string): Promise<string> {
        console.log(`Game joined: ${room} ${username}`);
        return room;
    },
    async vote(room: string, currentPlayer: string, target: string): Promise<void> {
        console.log(`Vote: ${room} ${currentPlayer} ${target}`);
    },

    async nextState(room: string, currentPlayer: string): Promise<void> {
        console.log(`nextState: ${room} ${currentPlayer}`);
    },

    on(eventType: string, handler: (PublicEvent) => void): () => void {
        console.log(`Subscribed: ${eventType}`, handler)

        return () => {
            console.log(`Unsubscribed: ${eventType}`, handler)
        };
    },

    async getPublicRooms(): Promise<GameRoom[]> {
        console.log(`getPublicRooms`);

        function fakeRoom(){
            return {
                id: Math.random().toString(),
                adminName: Math.random().toString(),
                playersCount: Math.ceil(Math.random() * 10),
            }
        }

        let output = [];

        for (let i = 0; i < 100; i++) {
            output.push(fakeRoom());
        }

        return output;
    }
}

const server: IJoinServer & IAdminServer & IPlayerServer & IEventsServer & IServer = new SignalRServer();

export default server;
