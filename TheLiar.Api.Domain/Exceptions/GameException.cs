using System.Runtime.CompilerServices;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Exceptions;

public class RoomAlreadyExistException : GameException
{
    public string RoomId { get; }

    public override string Message => $"Room already exist. Id: {RoomId}";
    
    
    public RoomAlreadyExistException(string roomId, Exception? inner = null) : base(null, inner)
    {
        RoomId = roomId;
    }
}


public class GameException : Exception
{
    public GameException() {}
    public GameException(string? message) : base(message) { }
    public GameException(string? message, Exception? inner) : base(message, inner) { }

    
    public static GameState ThrowWrongAction(Type callerType, [CallerMemberName] string? callerName = null)
    {
        throw new GameException($"Wrong action (Now: {callerType.Name}) : {callerName ?? "Unknown"}");
    }
}