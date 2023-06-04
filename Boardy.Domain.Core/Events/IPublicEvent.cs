using MediatR;

namespace Boardy.Domain.Core.Events;

public interface IEvent : INotification
{
    public Guid RoomId { get; }
}

public interface IPublicEvent : IEvent
{
    
}