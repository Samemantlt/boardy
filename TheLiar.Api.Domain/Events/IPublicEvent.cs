using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Events;

public record RoomUpdated(
    Guid RoomId,
    IEnumerable<Player> Players,
    GameState State,
    TimeoutOptions TimeoutOptions
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
    GameState Sender,
    Func<GameState> Func,
    TimeSpan? InvokeAfter = null
) : IEvent;