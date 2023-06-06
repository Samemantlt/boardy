using TheLiar.Api.Domain.Exceptions;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record VotingGameState(
        GameStateGlobals Globals,
        IReadOnlyDictionary<Guid, Guid> Votes
    )
    : GameState(Globals)
{
    public override GameStateType Type => GameStateType.Voting;


    public VotingGameState(GameStateGlobals globals) : this(globals, new Dictionary<Guid, Guid>()) { }


    public override void OnConstructed()
    {
        if (Votes.Count == Globals.Room.Players.Count)
            Invoke(Next);

        Invoke(Next, Globals.TimeoutOptions.VotingTimeout);
    }


    public override GameState AddVote(Guid sender, Guid other)
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

    public override GameState Next()
    {
        return new ShowRoundResultGameState(Globals, Votes);
    }
}