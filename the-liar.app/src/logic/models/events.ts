import {Secret, Player, GameStateType} from "./room";


export type PublicEvent = {
    eventType: string;
    event: any;
}

export type GameStarted = {
    roomId: string;
    mafiaId: string;
};

export type RoomClosed = {
    roomId: string;
};

export type GameRoom = {
    id: string;
    playersCount: number;
    adminName: string;
}

export type GameState = NotStarted | NewRound | ShowSecret | Voting | ShowRoundResult | WinMafia | WinPlayers;

export type TimeoutOptions = {
    newRoundTimeout: string,
    showSecretTimeout: string,
    votingTimeout: string,
    showRoundResultTimeout: string
};

export type RoomUpdated = {
    roomId: string;
    players: Player[];
    state: GameState;
    timeoutOptions: TimeoutOptions
}


export type NotStarted = {
    type: GameStateType.NotStarted;
};

export type NewRound = {
    type: GameStateType.NewRound;
    secret: Secret;
};

export type ShowSecret = {
    type: GameStateType.ShowSecret;
    secret: Secret;
};

export type Voting = {
    type: GameStateType.Voting;
    votes: {
        [player: string]: string
    }
};

export type ShowRoundResult = {
    type: GameStateType.ShowRoundResult;
    votes: { [player: string]: string }
    selected?: Player;
    isMafia?: boolean;
};

export type WinMafia = {
    type: GameStateType.WinMafia;
};

export type WinPlayers = {
    type: GameStateType.WinPlayers;
};


export type PublicEventHandler = (event: any) => void;

