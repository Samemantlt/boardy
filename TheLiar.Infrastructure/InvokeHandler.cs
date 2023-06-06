using MediatR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Repositories;

namespace TheLiar.Infrastructure;

public class InvokeHandler : INotificationHandler<Invoke>
{
    private readonly IRoomRepository _repository;


    public InvokeHandler(IRoomRepository repository)
    {
        _repository = repository;
    }


    public async Task Handle(Invoke notification, CancellationToken cancellationToken)
    {
        var room = await _repository.Get(notification.RoomId);
        
        await Task.Delay(notification.InvokeAfter ?? TimeSpan.Zero, cancellationToken);
        
        if (room.GameState == notification.Sender)
            room.Invoke(notification.Func);

        _repository.Save(room);
    }
}