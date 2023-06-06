namespace TheLiar.Api.Domain.Models.StateMachine;

public record WinPlayersGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.WinPlayers;


    public override void OnConstructed() { }
    
    public override GameState Next() => this;
}