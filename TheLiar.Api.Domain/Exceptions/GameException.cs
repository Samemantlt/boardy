using System.Runtime.CompilerServices;
using TheLiar.Api.Domain.Models.StateMachine;

namespace TheLiar.Api.Domain.Exceptions;

public class GameException : Exception
{
    public GameException() { }
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }


    public static void Throw(string? message = null)
    {
        if (message is not null)
            throw new GameException(message);
        throw new GameException();
    }

    public static GameState ThrowWrongAction(Type callerType, [CallerMemberName] string? callerName = null)
    {
        throw new GameException($"Wrong action (Now: {callerType.Name}) : {callerName ?? "Unknown"}");
    }
}