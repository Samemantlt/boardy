using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Events;

public record RoomUpdated(
    Guid RoomId,
    IEnumerable<Player> Players,
    GameStateMachine State
) : IPublicEvent;

public record GameStarted(
    Guid RoomId,
    Guid MafiaId
) : IPublicEvent;

public record RoomClosed(
    Guid RoomId
) : IPublicEvent;

public record Invoke(
    Guid RoomId,
    GameStateMachine Sender,
    Func<GameStateMachine> Func,
    TimeSpan? InvokeAfter = null
) : IEvent;