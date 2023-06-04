using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Events;

public record RoomUpdated(
    Guid RoomId,
    IEnumerable<Player> Players
) : IPublicEvent;

public record GameStarted(
    Guid RoomId,
    Guid MafiaId
) : IPublicEvent;

public record RoundStarted(
    Guid RoomId,
    ISecret Secret
) : IPublicEvent;

public record RoomClosed(
    Guid RoomId
) : IPublicEvent;

public record SecretShown(
    Guid RoomId,
    ISecret Secret
) : IPublicEvent;

public record RoundResultShown(
    Guid RoomId,
    IReadOnlyDictionary<Guid, Guid> Votes,
    Player? Selected,
    bool? IsMafia
) : IPublicEvent;

public record VotesChanged(
    Guid RoomId,
    IReadOnlyDictionary<Guid, Guid> Votes
) : IPublicEvent;

public record GameEnd(
    Guid RoomId,
    bool IsMafiaWin
) : IPublicEvent;


public record Invoke(
    Guid RoomId,
    GameStateMachine Sender,
    Func<GameStateMachine> Func,
    TimeSpan? InvokeAfter = null
) : IEvent;