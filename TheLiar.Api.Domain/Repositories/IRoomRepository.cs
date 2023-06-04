using TheLiar.Api.Domain.Models;

namespace TheLiar.Api.Domain.Repositories;

public interface IRoomRepository
{
    ValueTask<Room?> GetByPlayerConnectionId(string connectionId);
    ValueTask<Room> Get(Guid id);
    
    void Save(Room room);
}