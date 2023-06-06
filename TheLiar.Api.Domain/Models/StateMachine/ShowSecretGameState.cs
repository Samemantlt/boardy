using TheLiar.Api.Domain.Models.Secrets;

namespace TheLiar.Api.Domain.Models.StateMachine;

public record ShowSecretGameState(GameStateGlobals Globals, ISecret Secret) : GameState(Globals)
{
    public override GameStateType Type => GameStateType.ShowSecret;


    public override void OnConstructed()
    {
        Invoke(Next, Globals.TimeoutOptions.ShowSecretTimeout);
    }


    public override GameState Next()
    {
        return new VotingGameState(Globals);
    }
}