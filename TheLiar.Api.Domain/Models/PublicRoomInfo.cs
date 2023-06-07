namespace TheLiar.Api.Domain.Models;

public record PublicRoomInfo(
    string Id,
    string AdminName,
    int PlayersCount,
    DateTime Created
);