using Boardy.Domain.Core.Events;
using MediatR;
using TheLiar.Api.Domain.Events;

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