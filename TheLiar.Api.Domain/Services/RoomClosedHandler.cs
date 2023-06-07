using MediatR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Repositories;

namespace TheLiar.Api.Domain.Services;

public class RoomClosedHandler : INotificationHandler<RoomClosed>
{
    private readonly IRoomRepository _repository;

    
    public RoomClosedHandler(IRoomRepository repository)
    {
        _repository = repository;
    }
    
    
    public async Task Handle(RoomClosed notification, CancellationToken cancellationToken)
    {
        var room = await _repository.Get(notification.RoomId);

        _repository.Remove(room);
    }
}