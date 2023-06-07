namespace TheLiar.Api.Domain.Models.StateMachine;

public record WinPlayersGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.WinPlayers;


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