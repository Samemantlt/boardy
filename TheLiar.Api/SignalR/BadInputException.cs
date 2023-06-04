namespace TheLiar.Api.SignalR;

public class BadInputException : Exception
{
    public BadInputException() { }
    public BadInputException(string message) : base(message) { }
    public BadInputException(string message, Exception inner) : base(message, inner) { }
}