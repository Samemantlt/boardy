using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.SignalR;

public interface IHubServer
{
    Task<Guid> CreateRoom(string playerName, TimeoutOptions timeoutOptions, bool isPublic);
    
    Task<Guid> JoinRoom(Guid roomId, string playerName);
    
    Task AddVote(Guid roomId, Guid playerId, Guid targetId);
    
    Task NextState(Guid roomId, Guid playerId);
    
    Task<List<PublicRoomInfo>> GetPublicRooms();
}