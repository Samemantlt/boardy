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