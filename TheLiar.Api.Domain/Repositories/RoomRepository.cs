using MediatR;
using TheLiar.Api.Domain.Exceptions;
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

    public async ValueTask<Room> Get(string id)
    {
        return _rooms.First(p => p.Id == id);
    }

    public async ValueTask<List<PublicRoomInfo>> GetAllPublicRooms()
    {
        return _rooms
            .Where(p => !p.IsStarted)
            .Where(p => p.IsPublic)
            .Select(p => new PublicRoomInfo(p.Id, p.Admin.Name, p.Players.Count, p.Created))
            .OrderByDescending(p => p.Created)
            .ToList();
    }

    public async ValueTask AssertUniqueId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

        var exists = _rooms.Any(p => p.Id == id);

        if (exists)
            throw new RoomAlreadyExistException(id);
    }

    public void Save(Room room)
    {
        room.PublishEventsAndClear(_mediator);

        if (room.IsClosed)
            return;

        if (_rooms.Contains(room))
            return;

        _rooms.Add(room);
    }

    public void Remove(Room room)
    {
        if (_rooms.Contains(room))
            _rooms.Remove(room);
    }


    private readonly IMediator _mediator;
    private readonly List<Room> _rooms = new List<Room>();
}