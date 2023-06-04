using Boardy.Domain.Core.Events;
using MediatR;

namespace Boardy.Domain.Core;

public class EntityBase
{
    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();

    
    public void RaiseEvent(IEvent @event)
    {
        _events.Add(@event);
    }
    
    public void PublishEventsAndClear(IMediator mediator)
    {
        foreach (var @event in _events)
        {
            mediator.Publish(@event);
        }
        
        _events.Clear();
    }
    

    private readonly List<IEvent> _events = new();
}