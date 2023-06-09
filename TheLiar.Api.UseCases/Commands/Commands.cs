﻿using Boardy.Domain.Core.Commands;
using MediatR;
using TheLiar.Api.Domain.Extensions;
using TheLiar.Api.Domain.Models;
using TheLiar.Api.Domain.Models.StateMachine;
using TheLiar.Api.Domain.Repositories;
using TheLiar.Api.Domain.Services;

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
    public record Request(
        string RoomId,
        string PlayerName,
        string ConnectionId,
        TimeoutOptions TimeoutOptions,
        bool IsPublic
    ) : IRequest<Response>;

    public record Response(string RoomId);


    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ISecretSource _secretSource;


        public Handler(IRoomRepository roomRepository, ISecretSource secretSource)
        {
            _roomRepository = roomRepository;
            _secretSource = secretSource;
        }


        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await _roomRepository.AssertUniqueId(request.RoomId);
            
            var room = new Room(
                request.RoomId,
                new Player(request.PlayerName, request.ConnectionId),
                request.TimeoutOptions,
                request.IsPublic,
                _secretSource
            );
            _roomRepository.Save(room);

            return new Response(room.Id);
        }
    }
}

public static partial class JoinRoom
{
    public record Request(string RoomId, string PlayerName, string ConnectionId) : IRequest<Response>;

    public record Response(string RoomId);


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
    public record Request(string RoomId, Guid PlayerId, Guid TargetId) : IRequest, IPlayerInRoomRequest;


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

            if (room.GameState == null)
                throw new Exception("Game not started");

            room.Invoke(() => room.GameState.AddVote(request.PlayerId, request.TargetId));

            _roomRepository.Save(room);
        }
    }
}

public static partial class NextState
{
    public record Request(string RoomId, Guid PlayerId) : IRequest, IAdminRequest;


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

            if (room.GameState == null)
                throw new Exception("Game not started");

            room.Invoke(room.GameState.Next);

            _roomRepository.Save(room);
        }
    }
}

public static partial class GetPublicRooms
{
    public record Request() : IRequest<Response>;

    public record Response(List<PublicRoomInfo> PublicRooms);
    
    
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IRoomRepository _roomRepository;
        
        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return new Response(await _roomRepository.GetAllPublicRooms());
        }
    }
}