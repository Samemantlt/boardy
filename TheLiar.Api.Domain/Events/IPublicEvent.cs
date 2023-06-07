using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Events;

public record RoomUpdated(
    string RoomId,
    IEnumerable<Player> Players,
    GameState State,
    TimeoutOptions TimeoutOptions
) : IPublicEvent;

public record GameStarted(
    string RoomId,
    Guid MafiaId
) : IPublicEvent;

public record RoomClosed(
    string RoomId
) : IPublicEvent;

public record Invoke(
    string RoomId,
    GameState Sender,
    Func<GameState> Func,
    TimeSpan? InvokeAfter = null
) : IEvent;