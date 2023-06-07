using TheLiar.Api.Domain.Models;

namespace TheLiar.Api.Domain.Repositories;

public interface IRoomRepository
{
    ValueTask<Room?> GetByPlayerConnectionId(string connectionId);
    ValueTask<Room> Get(string id);
    ValueTask<List<PublicRoomInfo>> GetAllPublicRooms();
    ValueTask AssertUniqueId(string id);

    void Save(Room room);
}