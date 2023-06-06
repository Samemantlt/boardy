using System.Text.Json.Serialization;
using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Exceptions;

namespace TheLiar.Api.Domain.Models.StateMachine;

[JsonDerivedType(typeof(NotStartedGameState))]
[JsonDerivedType(typeof(NewRoundGameState))]
[JsonDerivedType(typeof(ShowSecretGameState))]
[JsonDerivedType(typeof(VotingGameState))]
[JsonDerivedType(typeof(ShowRoundResultGameState))]
[JsonDerivedType(typeof(WinMafiaGameState))]
[JsonDerivedType(typeof(WinPlayersGameState))]
public abstract record GameState
{
    public abstract GameStateType Type { get; }


    protected GameState(GameStateGlobals globals)
    {
        Globals = globals;
        // ReSharper disable once VirtualMemberCallInConstructor
        OnConstructed();
    }


    public abstract void OnConstructed();
    public virtual GameState AddVote(Guid sender, Guid other) => GameException.ThrowWrongAction(GetType());
    public abstract GameState Next();

    protected void RaiseEvent(IEvent @event)
    {
        Globals.RaiseEvent(@event);
    }

    protected void Invoke(Func<GameState> func, TimeSpan? invokeAfter = null)
    {
        Globals.RaiseEvent(new Invoke(Globals.Room.Id, this, func, invokeAfter));
    }


    protected GameStateGlobals Globals { get; init; }
}