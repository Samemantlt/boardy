using MediatR;
using TheLiar.Api.Domain.Models;

namespace TheLiar.Api.Domain.Repositories;


public class RoomRepository : IRoomRepository
{
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();


    public RoomRepository(IMediator mediator)
    {
        _mediator = mediator;
    }


    public async ValueTask<Room?> GetByPlayerConnectionId(string connectionId)
    {
        return _rooms.FirstOrDefault(p => p.Players.Any(player => player.ConnectionId == connectionId));
    }

    public async ValueTask<Room> Get(Guid id)
    {
        return _rooms.First(p => p.Id == id);
    }

    public async ValueTask<List<PublicRoomInfo>> GetAllPublicRooms()
    {
        return _rooms
            .Where(p => p.IsPublic)
            .Select(p => new PublicRoomInfo(p.Id, p.Admin.Name, p.Players.Count, p.Created))
            .OrderByDescending(p => p.Created)
            .ToList();
    }

    public void Save(Room room)
    {
        room.PublishEventsAndClear(_mediator);
        
        if (_rooms.Contains(room))
            return;
        
        _rooms.Add(room);
    }
    

    private readonly IMediator _mediator;
    private readonly List<Room> _rooms = new List<Room>();
}