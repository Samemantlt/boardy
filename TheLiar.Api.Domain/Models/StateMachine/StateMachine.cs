using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Boardy.Domain.Core;
using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Events;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record GameStateGlobals(
    Room Room,
    GameStateTimeoutOptions TimeoutOptions,
    Action<IEvent> RaiseEvent,
    Func<ISecret> CreateSecret);

public record GameStateTimeoutOptions(
    TimeSpan NewRoundTimeout,
    TimeSpan ShowSecretTimeout,
    TimeSpan VotingTimeout,
    TimeSpan ShowRoundResultTimeout
);

public record BoolQuestionSecret(string Text) : ISecret;

public interface ISecret
{
    string Text { get; }
}

public class GameException : Exception
{
    public GameException() { }
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }


    public static void Throw(string? message = null)
    {
        if (message is not null)
            throw new GameException(message);
        throw new GameException();
    }

    public static GameStateMachine ThrowWrongAction(Type callerType, [CallerMemberName] string? callerName = null)
    {
        throw new GameException($"Wrong action (Now: {callerType.Name}) : {callerName ?? "Unknown"}");
    }
}

public enum GameStateName
{
    NotStarted,
    NewRound,
    ShowSecret,
    Voting,
    ShowRoundResult,
    WinMafia,
    WinPlayers
}

[JsonDerivedType(typeof(NotStartedGameState))]
[JsonDerivedType(typeof(NewRoundGameState))]
[JsonDerivedType(typeof(ShowSecretGameState))]
[JsonDerivedType(typeof(VotingGameState))]
[JsonDerivedType(typeof(ShowRoundResultGameState))]
[JsonDerivedType(typeof(WinMafiaGameState))]
[JsonDerivedType(typeof(WinPlayersGameState))]
public abstract record GameStateMachine
{
    public abstract GameStateName Name { get; }


    protected GameStateMachine(GameStateGlobals globals)
    {
        Globals = globals;
        // ReSharper disable once VirtualMemberCallInConstructor
        OnConstructed();
    }


    public abstract void OnConstructed();
    public virtual GameStateMachine NewRound() => GameException.ThrowWrongAction(GetType());
    public virtual GameStateMachine ShowSecret() => GameException.ThrowWrongAction(GetType());
    public virtual GameStateMachine StartVoting() => GameException.ThrowWrongAction(GetType());
    public virtual GameStateMachine AddVote(Guid sender, Guid other) => GameException.ThrowWrongAction(GetType());
    public virtual GameStateMachine EndVoting() => GameException.ThrowWrongAction(GetType());
    public virtual GameStateMachine EndGame() => GameException.ThrowWrongAction(GetType());


    protected void RaiseEvent(IEvent @event)
    {
        Globals.RaiseEvent(@event);
    }

    protected void Invoke(Func<GameStateMachine> func, TimeSpan? invokeAfter = null)
    {
        Globals.RaiseEvent(new Invoke(Globals.Room.Id, this, func, invokeAfter));
    }


    protected GameStateGlobals Globals { get; init; }
}

public record NotStartedGameState(GameStateGlobals Globals) : GameStateMachine(Globals)
{
    public override GameStateName Name => GameStateName.NotStarted;


    public override void OnConstructed() { }

    public override GameStateMachine NewRound()
    {
        return new NewRoundGameState(Globals);
    }
}

public record NewRoundGameState(GameStateGlobals Globals) : GameStateMachine(Globals)
{
    public ISecret Secret { get; private set; } = null!;


    public override GameStateName Name => GameStateName.NewRound;


    public override void OnConstructed()
    {
        Secret = Globals.CreateSecret();
        Invoke(ShowSecret, Globals.TimeoutOptions.NewRoundTimeout);
    }


    public override GameStateMachine ShowSecret()
    {
        return new ShowSecretGameState(Globals, Secret);
    }
}

public record ShowSecretGameState(GameStateGlobals Globals, ISecret Secret) : GameStateMachine(Globals)
{
    public override GameStateName Name => GameStateName.ShowSecret;


    public override void OnConstructed()
    {
        Invoke(StartVoting, Globals.TimeoutOptions.ShowSecretTimeout);
    }


    public override GameStateMachine StartVoting()
    {
        return new VotingGameState(Globals);
    }
}

public record VotingGameState(
        GameStateGlobals Globals,
        IReadOnlyDictionary<Guid, Guid> Votes
    )
    : GameStateMachine(Globals)
{
    public override GameStateName Name => GameStateName.Voting;


    public VotingGameState(GameStateGlobals globals) : this(globals, new Dictionary<Guid, Guid>()) { }


    public override void OnConstructed()
    {
        if (Votes.Count == Globals.Room.Players.Count)
            Invoke(EndVoting);

        Invoke(EndVoting, Globals.TimeoutOptions.VotingTimeout);
    }


    public override GameStateMachine AddVote(Guid sender, Guid other)
    {
        var newVotes = new Dictionary<Guid, Guid>(Votes);
        if (newVotes.ContainsKey(sender))
            throw new GameException("You has already voted");

        newVotes.Add(sender, other);


        return this with
        {
            Votes = newVotes
        };
    }

    public override GameStateMachine EndVoting()
    {
        return new ShowRoundResultGameState(Globals, Votes);
    }
}

public record ShowRoundResultGameState(
    GameStateGlobals Globals,
    IReadOnlyDictionary<Guid, Guid> Votes
) : GameStateMachine(Globals)
{
    public Player? Selected { get; private set; }
    public bool? IsMafia => Selected?.IsMafia;


    public override GameStateName Name => GameStateName.ShowRoundResult;


    public override void OnConstructed()
    {
        UpdateSelected();

        if (IsMafia ?? false)
            Invoke(EndGame, Globals.TimeoutOptions.ShowRoundResultTimeout);
        else
            Invoke(NewRound, Globals.TimeoutOptions.ShowRoundResultTimeout);
    }

    private void UpdateSelected()
    {
        if (Votes.Count == 0)
            return;

        var grouped = Votes
            .GroupBy(p => p.Value)
            .ToList();

        var maxVotes = grouped
            .Max(p => p.Count());

        Guid? selectedId = grouped
            .First(p => p.Count() == maxVotes)
            .Key;

        if (grouped.Count(p => p.Count() == maxVotes) > 1)
            selectedId = null;

        Selected = Globals.Room.Players.FirstOrDefault(p => p.Id == selectedId);
    }

    public override GameStateMachine EndGame()
    {
        if (IsMafia ?? false)
            return new WinMafiaGameState(Globals);

        return new WinPlayersGameState(Globals);
    }

    public override GameStateMachine NewRound()
    {
        if (IsMafia ?? false)
            return new WinMafiaGameState(Globals);
        
        return new NewRoundGameState(Globals);
    }
}

public record WinPlayersGameState(GameStateGlobals Globals) : GameStateMachine(Globals)
{
    public override GameStateName Name => GameStateName.WinPlayers;


    public override void OnConstructed() { }
}

public record WinMafiaGameState(GameStateGlobals Globals) : GameStateMachine(Globals)
{
    public override GameStateName Name => GameStateName.WinMafia;


    public override void OnConstructed() { }
}