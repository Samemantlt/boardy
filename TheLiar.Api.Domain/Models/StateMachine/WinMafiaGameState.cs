namespace TheLiar.Api.Domain.Models.StateMachine;

public record WinMafiaGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.WinMafia;


    public override void OnConstructed()
    {
        Invoke(Next, Globals.TimeoutOptions.EndGameTimeout);
    }

    public override GameState Next()
    {
        Globals.Room.Close();
        
        return this;
    }
}