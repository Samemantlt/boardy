using Boardy.Domain.Core.Commands;
using Boardy.Domain.Core.Events;
using MediatR;

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