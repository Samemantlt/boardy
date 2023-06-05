namespace TheLiar.Api.SignalR;

public interface IHubServer
{
    Task<Guid> CreateRoom(string playerName);
    
    Task<Guid> JoinRoom(Guid roomId, string playerName);
    
    Task AddVote(Guid roomId, Guid playerId, Guid targetId);
    
    Task NextState(Guid roomId, Guid playerId);
    
    Task StartGame(Guid roomId, Guid playerId);
    
    Task NewRound(Guid roomId, Guid playerId);
    
    Task ShowSecret(Guid roomId, Guid playerId);
    
    Task StartVoting(Guid roomId, Guid playerId);
    
    Task EndVoting(Guid roomId, Guid playerId);
}