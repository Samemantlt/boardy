namespace Boardy.Domain.Core.Commands;

public interface IPlayerInRoomRequest
{
    Guid RoomId { get; }
    
    Guid PlayerId { get; }
}

public interface IAdminRequest : IPlayerInRoomRequest
{
    
}