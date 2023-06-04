using Boardy.Domain.Core.Events;

namespace TheLiar.Api.SignalR;


public record RaisedEvent(string EventType, dynamic Event);

public interface IHubUser
{
    Task Raise(RaisedEvent notification);
}