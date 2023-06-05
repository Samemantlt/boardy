namespace TheLiar.Api.SignalR;

public interface IHubServer
{
    Task<Guid> CreateRoom(string playerName);
    
    Task<Guid> JoinRoom(Guid roomId, string playerName);
    
    Task AddVote(Guid roomId, Guid playerId, Guid targetId);
    
    Task NextState(Guid roomId, Guid playerId);
}