using TheLiar.Api.Domain.Models;

namespace TheLiar.Api.Domain.Extensions;

public static class PlayerExtensions
{
    public static void AssertAdminId(this Room room, Guid playerId)
    {
        if (room.Admin.Id != playerId)
            throw new Exception($"You are not the admin of this room: {playerId}");
    }
}