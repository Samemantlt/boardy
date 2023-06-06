namespace TheLiar.Api.Domain.Models.StateMachine;

public enum GameStateType
{
    NotStarted,
    NewRound,
    ShowSecret,
    Voting,
    ShowRoundResult,
    WinMafia,
    WinPlayers
}