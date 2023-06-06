using System.Security.Claims;
using Boardy.Domain.Core.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;
using TheLiar.Api.UseCases.Commands;

namespace TheLiar.Api.SignalR;

public class PublicEventSender : INotificationHandler<IPublicEvent>
{
    private readonly Func<GameHub> _gameHub;


    public PublicEventSender(Func<GameHub> gameHub)
    {
        _gameHub = gameHub;
    }


    public async Task Handle(IPublicEvent notification, CancellationToken cancellationToken)
    {
        var hub = _gameHub();

        await hub.Clients.Group(hub.GroupName(notification.RoomId))
            .Raise(new RaisedEvent(notification.GetType().Name, notification));
    }
}

public class GameHub : Hub<IHubUser>, IHubServer
{
    private readonly IMediator _mediator;


    public GameHub(IMediator mediator)
    {
        _mediator = mediator;
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _mediator.Send(new PlayerDisconnected.Request(Context.ConnectionId));
    }


    public async Task<Guid> CreateRoom(string playerName, GameStateTimeoutOptions timeoutOptions, bool isPublic)
    {
        var roomId = Guid.NewGuid();

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(roomId));

        var result = await _mediator.Send(new CreateRoom.Request(
            roomId,
            playerName,
            Context.ConnectionId,
            timeoutOptions,
            isPublic
        ));

        return result.RoomId;
    }

    public async Task<Guid> JoinRoom(Guid roomId, string playerName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(roomId));

        var result = await _mediator.Send(new JoinRoom.Request(roomId, playerName, Context.ConnectionId));

        return result.RoomId;
    }

    public async Task NextState(Guid roomId, Guid playerId)
    {
        await _mediator.Send(new NextState.Request(roomId, playerId));
    }

    public async Task AddVote(Guid roomId, Guid playerId, Guid targetId)
    {
        await _mediator.Send(new AddVote.Request(roomId, playerId, targetId));
    }

    public async Task<List<PublicRoomInfo>> GetPublicRooms()
    {
        var result = await _mediator.Send(new GetPublicRooms.Request());
        return result.PublicRooms;
    }


    public string GroupName(Guid roomId) => $"Room_{roomId}";


    private const string PlayerKey = "player";
    private const string RoomKey = "room";
}