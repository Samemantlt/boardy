namespace TheLiar.Api.Domain.Models;

public record TimeoutOptions(
    TimeSpan NewRoundTimeout,
    TimeSpan ShowSecretTimeout,
    TimeSpan VotingTimeout,
    TimeSpan ShowRoundResultTimeout,
    TimeSpan EndGameTimeout
);