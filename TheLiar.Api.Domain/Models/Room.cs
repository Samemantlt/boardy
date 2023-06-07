using System.Collections;
using System.Globalization;
using Boardy.Domain.Core;
using MediatR;
using TheLiar.Api.Domain.Events;
using TheLiar.Api.Domain.Exceptions;
using TheLiar.Api.Domain.Models.StateMachine;
using TheLiar.Api.Domain.Services;

namespace TheLiar.Api.Domain.Models;

public class Room : EntityBase
{
    public string Id { get; }

    public Player Admin { get; }

    public bool IsPublic { get; }

    public bool IsClosed { get; private set; } = false;

    public Player Mafia => Players.Single(p => p.IsMafia);

    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    public GameState GameState { get; private set; }

    public bool IsStarted => GameState is not NotStartedGameState;

    public GameStateGlobals Globals { get; set; }

    public DateTime Created { get; } = DateTime.UtcNow;


    public Room(
        string id,
        Player admin,
        TimeoutOptions timeoutOptions,
        bool isPublic,
        ISecretSource secretSource
    )
    {
        _timeoutOptions = timeoutOptions;
        _secretSource = secretSource;

        Id = id;
        Admin = admin;
        IsPublic = isPublic;

        Globals = CreateGlobals();
        GameState = new NotStartedGameState(Globals);

        AddPlayer(admin);
    }


    public void AddPlayer(Player player)
    {
        if (IsStarted)
            throw new GameAlreadyStartedException(Id);

        _players.Add(player);
        _players.Sort((p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal));

        RaiseRoomUpdated();
    }

    public void RemovePlayer(Player player)
    {
        _players.Remove(player);

        if (player == Admin)
            Close();

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
        
        if (!IsClosed)
            RaiseRoomUpdated();
    }

    public void Invoke(Func<GameState> func)
    {
        InvokeInStateMachine(GameState!, func);
    }

    public void Close()
    {
        if (IsClosed)
            return;

        IsClosed = true;
        RaiseEvent(new RoomClosed(Id));
    }

    private GameStateGlobals CreateGlobals()
    {
        return new GameStateGlobals(
            this,
            _timeoutOptions,
            RaiseEvent,
            _secretSource.RandomSecret
        );
    }

    private void RaiseRoomUpdated()
    {
        RaiseEvent(new RoomUpdated(Id, Players, GameState, Globals.TimeoutOptions));
    }


    private readonly TimeoutOptions _timeoutOptions;
    private readonly ISecretSource _secretSource;
    private readonly List<Player> _players = new List<Player>();
}