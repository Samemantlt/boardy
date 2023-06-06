using System.Collections;
using System.Globalization;
using Boardy.Domain.Core;
using Boardy.Domain.Core.Events;
using MediatR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Models.Secrets;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Models;

public class Player
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string ConnectionId { get; }
    public bool IsMafia { get; private set; }


    public Player(string name, string connectionId)
    {
        Name = name;
        ConnectionId = connectionId;
    }


    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public void SetMafia()
    {
        IsMafia = true;
    }
}

public class Room : EntityBase
{
    public Guid Id { get; }

    public Player Admin { get; }
    
    public bool IsPublic { get; }

    public Player Mafia => Players.Single(p => p.IsMafia);

    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    public GameState GameState { get; private set; }

    public GameStateGlobals Globals { get; set; }
    

    public Room(Guid id, Player admin, TimeoutOptions timeoutOptions, bool isPublic)
    {
        _timeoutOptions = timeoutOptions;
        
        Id = id;
        Admin = admin;
        IsPublic = isPublic;

        Globals = CreateGlobals();
        GameState = new NotStartedGameState(Globals);
        
        AddPlayer(admin);
    }

    
    public void AddPlayer(Player player)
    {
        _players.Add(player);
        _players.Sort((p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal));

        RaiseRoomUpdated();
    }

    public void RemovePlayer(Player player)
    {
        _players.Remove(player);
        
        if (player == Admin)
            RaiseEvent(new RoomClosed(Id));
        
        if (player.IsMafia)
            Invoke(() => new WinPlayersGameState(Globals));
        
        if (_players.Count <= 2)
            Invoke(() => new WinMafiaGameState(Globals));

        RaiseRoomUpdated();
    }
    
    private void InvokeInStateMachine(GameState sender, Func<GameState> func)
    {
        if (GameState != sender)
            return;

        GameState = func();
        RaiseRoomUpdated();
    }
    
    public void Invoke(Func<GameState> func)
    {
        InvokeInStateMachine(GameState!, func);
    }

    private GameStateGlobals CreateGlobals()
    {
        return new GameStateGlobalsSource(this, _timeoutOptions).Build();
    }

    private void RaiseRoomUpdated()
    {
        RaiseEvent(new RoomUpdated(Id, Players, GameState, Globals.TimeoutOptions));
    }


    private readonly TimeoutOptions _timeoutOptions;
    private readonly List<Player> _players = new List<Player>();
}

public class GameStateGlobalsSource
{
    public Room Room { get; }
    public TimeoutOptions TimeoutOptions { get; }


    public GameStateGlobalsSource(Room room, TimeoutOptions timeoutOptions)
    {
        Room = room;
        TimeoutOptions = timeoutOptions;
    }
    

    public GameStateGlobals Build()
    {
        return new GameStateGlobals(
            Room,
            TimeoutOptions,
            /*
            new GameStateTimeoutOptions(
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8)
            ),
            new GameStateTimeoutOptions(
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(90),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(90)
            ),*/
            RaiseEvent,
            CreateSecret
        );
    }

    private ISecret CreateSecret()
    {
        return new HandUpSecret("Is this the secret?");
    }
    
    private void RaiseEvent(IEvent @event)
    {
        Room.RaiseEvent(@event);
    }
}