import {Player} from "./room";

export type PublicEvent = {
    eventType: string;
    event: any;
}

export type RoomUpdated = {
    roomId: string;
    players: Player[];
}

export type GameStarted = {
    roomId: string;
    mafiaId: string;
};

export type RoundStarted = {
    roomId: string;
    secret: {
        text: string;
    };
};

export type RoomClosed = {
    roomId: string;
};

export type SecretShown = {
    roomId: string;
    secret: any;
};

export type RoundResultShown = {
    roomId: string;
    votes: { [player: string]: string }
    selected?: Player;
    isMafia?: boolean;
};

export type VotesChanged = {
    roomId: string;
    votes: { [player: string]: string }
};

export type GameEnd = {
    roomId: string;
    isMafiaWin: boolean;
};


export type PublicEventHandler = (event: any) => void;

