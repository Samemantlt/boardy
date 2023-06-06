namespace TheLiar.Api.Domain.Models;

public record PublicRoomInfo(
    Guid Id,
    string AdminName,
    int PlayersCount,
    DateTime Created
);