using System.Runtime.CompilerServices;
using Boardy.Domain.Core;
using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Events;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record GameStateGlobals(
    Room Room,
    GameStateOptions Options,
    Action<IEvent> RaiseEvent,
    Func<ISecret> CreateSecret);

public record GameStateOptions(
    TimeSpan SecretDiscussTimeout,
    TimeSpan MafiaDiscussTimeout,
    TimeSpan AfterLastVoteTimeout,
    TimeSpan AfterRoundTimeout
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

public abstract record GameStateMachine
{
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


    public GameStateGlobals Globals { get; init; }
}

public record NewRoundGameState(GameStateGlobals Globals) : GameStateMachine(Globals)
{
    public ISecret Secret { get; private set; } = null!;


    public override void OnConstructed()
    {
        Secret = Globals.CreateSecret();
        RaiseEvent(new RoundStarted(Globals.Room.Id, Secret));

        Invoke(ShowSecret, Globals.Options.SecretDiscussTimeout);
    }


    public override GameStateMachine ShowSecret()
    {
        return new ShowSecretGameState(Globals, Secret);
    }
}

public record ShowSecretGameState(GameStateGlobals Globals, ISecret Secret) : GameStateMachine(Globals)
{
    public override void OnConstructed()
    {
        Globals.RaiseEvent(new SecretShown(Globals.Room.Id, Secret));

        Invoke(StartVoting, Globals.Options.MafiaDiscussTimeout);
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
    public VotingGameState(GameStateGlobals globals) : this(globals, new Dictionary<Guid, Guid>()) { }


    public override void OnConstructed()
    {
        RaiseEvent(new VotesChanged(Globals.Room.Id, Votes));
        
        if (Votes.Count == Globals.Room.Players.Count)
            Invoke(EndVoting);

        Invoke(EndVoting, Globals.Options.AfterLastVoteTimeout);
    }


    public override GameStateMachine AddVote(Guid sender, Guid other)
    {
        var newVotes = new Dictionary<Guid, Guid>(Votes);
        if (newVotes.ContainsKey(sender))
            throw new GameException("You has already voted");

        newVotes.Add(sender, other);


        RaiseEvent(new VotesChanged(Globals.Room.Id, newVotes));

        return this with
        {
            Votes = newVotes
        };
    }

    public override GameStateMachine EndVoting()
    {
        return new ShowRoundResult(Globals, Votes);
    }
}

public record ShowRoundResult(
    GameStateGlobals Globals,
    IReadOnlyDictionary<Guid, Guid> Votes
) : GameStateMachine(Globals)
{
    public Player? Selected { get; private set; }
    public bool? IsMafia { get; private set; }


    public override void OnConstructed()
    {
        UpdateSelected();
        IsMafia = Selected?.IsMafia;

        RaiseEvent(new RoundResultShown(Globals.Room.Id, Votes, Selected, IsMafia));
        Invoke(NewRound, Globals.Options.AfterRoundTimeout);
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

    public override GameStateMachine NewRound()
    {
        return new NewRoundGameState(Globals);
    }
}