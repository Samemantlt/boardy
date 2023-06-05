using Boardy.Domain.Core.Commands;
using MediatR;
using TheLiar.Api.Domain.Extensions;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;
using TheLiar.Api.Domain.Repositories;

namespace TheLiar.Api.UseCases.Commands;

public static partial class PlayerDisconnected
{
    public record Request(string ConnectionId) : IRequest;

    public class Handler : IRequestHandler<Request>
    {
        private readonly IRoomRepository _roomRepository;


        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByPlayerConnectionId(request.ConnectionId);
            if (room == null)
                return;

            room.RemovePlayer(room.Players.First(p => p.ConnectionId == request.ConnectionId));

            _roomRepository.Save(room);
        }
    }
}

public static partial class CreateRoom
{
    public record Request(Guid RoomId, string PlayerName, string ConnectionId) : IRequest<Response>;

    public record Response(Guid RoomId);


    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IRoomRepository _roomRepository;


        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var room = new Room(request.RoomId, new Player(request.PlayerName, request.ConnectionId));
            _roomRepository.Save(room);

            return new Response(room.Id);
        }
    }
}

public static partial class JoinRoom
{
    public record Request(Guid RoomId, string PlayerName, string ConnectionId) : IRequest<Response>;

    public record Response(Guid RoomId);


    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IRoomRepository _roomRepository;


        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(request.RoomId);
            room.AddPlayer(new Player(request.PlayerName, request.ConnectionId));
            _roomRepository.Save(room);

            return new Response(room.Id);
        }
    }
}

public static partial class AddVote
{
    public record Request(Guid RoomId, Guid PlayerId, Guid TargetId) : IRequest;


    public class Handler : IRequestHandler<Request>
    {
        private readonly IRoomRepository _roomRepository;


        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(request.RoomId);

            if (room.GameStateMachine == null)
                throw new Exception("Game not started");

            room.Invoke(() => room.GameStateMachine.AddVote(request.PlayerId, request.TargetId));

            _roomRepository.Save(room);
        }
    }
}

public static partial class NextState
{
    public record Request(Guid RoomId, Guid PlayerId) : IRequest, IAdminRequest;


    public class Handler : IRequestHandler<Request>
    {
        private readonly IRoomRepository _roomRepository;


        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.Get(request.RoomId);

            if (room.GameStateMachine == null)
                throw new Exception("Game not started");

            room.Invoke(room.GameStateMachine.Next);

            _roomRepository.Save(room);
        }
    }
}