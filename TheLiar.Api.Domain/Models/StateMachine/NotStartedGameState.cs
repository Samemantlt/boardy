using TheLiar.Api.Domain.Events;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record NotStartedGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.NotStarted;


    public override void OnConstructed() { }

    public override GameState Next()
    {
        Globals.Room.Players.ElementAt(Random.Shared.Next(Globals.Room.Players.Count)).SetMafia();

        RaiseEvent(new GameStarted(Globals.Room.Id, Globals.Room.Mafia.Id));
        
        return new NewRoundGameState(Globals);
    }
}