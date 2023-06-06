namespace TheLiar.Api.Domain.Models.StateMachine;

public record WinMafiaGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.WinMafia;


    public override void OnConstructed() { }
    
    public override GameState Next() => this;
}