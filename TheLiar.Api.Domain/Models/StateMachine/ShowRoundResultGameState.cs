namespace TheLiar.Api.Domain.Models.StateMachine;

public record ShowRoundResultGameState(
    GameStateGlobals Globals,
    IReadOnlyDictionary<Guid, Guid> Votes
) : GameState(Globals)
{
    public Player? Selected { get; private set; }
    public bool? IsMafia => Selected?.IsMafia;


    public override GameStateType Type => GameStateType.ShowRoundResult;


    public override void OnConstructed()
    {
        UpdateSelected();

        Invoke(Next, Globals.TimeoutOptions.ShowRoundResultTimeout);
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
    
    
    public override GameState Next()
    {
        if (IsMafia ?? false)
            return new WinMafiaGameState(Globals);
        
        return new NewRoundGameState(Globals);
    }
}