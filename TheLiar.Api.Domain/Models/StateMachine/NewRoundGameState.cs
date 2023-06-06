using TheLiar.Api.Domain.Models.Secrets;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record NewRoundGameState(GameStateGlobals Globals) : GameState(Globals)
{
    public ISecret Secret { get; private set; } = null!;


    public override GameStateType Type => GameStateType.NewRound;


    public override void OnConstructed()
    {
        Secret = Globals.CreateSecret();
        Invoke(Next, Globals.TimeoutOptions.NewRoundTimeout);
    }


    public override GameState Next()
    {
        return new ShowSecretGameState(Globals, Secret);
    }
}