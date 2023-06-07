namespace Boardy.Domain.Core.Commands;

public interface IPlayerInRoomRequest
{
    string RoomId { get; }
    
    Guid PlayerId { get; }
}

public interface IAdminRequest : IPlayerInRoomRequest
{
    
}