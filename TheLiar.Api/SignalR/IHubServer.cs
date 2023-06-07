using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.SignalR;

public interface IHubServer
{
    Task<string> CreateRoom(string roomId, string playerName, TimeoutOptions timeoutOptions, bool isPublic);
    
    Task<string> JoinRoom(string roomId, string playerName);
    
    Task AddVote(string roomId, Guid playerId, Guid targetId);
    
    Task NextState(string roomId, Guid playerId);
    
    Task<List<PublicRoomInfo>> GetPublicRooms();
}