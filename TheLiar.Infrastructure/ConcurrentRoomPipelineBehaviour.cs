using Boardy.Domain.Core.Commands;
using Boardy.Domain.Core.Events;
using MediatR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Repositories;

namespace TheLiar.Infrastructure;

// TODO: Use semaphore per room
public class ConcurrentRoomPipelineBehaviour : IPipelineBehavior<IPlayerInRoomRequest, object>
{
    public async Task<object> Handle(IPlayerInRoomRequest request,
        RequestHandlerDelegate<object> next,
        CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            return await next();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }


    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
}

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
        
        if (room.GameStateMachine == notification.Sender)
            room.Invoke(notification.Func);

        _repository.Save(room);
    }
}