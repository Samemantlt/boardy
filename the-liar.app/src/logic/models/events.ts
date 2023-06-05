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
    name: GameStateType.NotStarted;
};

export type NewRound = {
    name: GameStateType.NewRound;
    secret: Secret;
};

export type ShowSecret = {
    name: GameStateType.ShowSecret;
    secret: Secret;
};

export type Voting = {
    name: GameStateType.Voting;
    votes: {
        [player: string]: string
    }
};

export type ShowRoundResult = {
    name: GameStateType.ShowRoundResult;
    votes: { [player: string]: string }
    selected?: Player;
    isMafia?: boolean;
};

export type WinMafia = {
    name: GameStateType.WinMafia;
};

export type WinPlayers = {
    name: GameStateType.WinPlayers;
};


export type PublicEventHandler = (event: any) => void;

