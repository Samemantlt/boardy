using MediatR;

namespace Boardy.Domain.Core.Events;

public interface IEvent : INotification
{
    public string RoomId { get; }
}

public interface IPublicEvent : IEvent
{
    
}