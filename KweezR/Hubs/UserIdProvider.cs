using Microsoft.AspNetCore.SignalR;

namespace KweezR.Hubs
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection?.User?.Identity?.Name;
        }
    }
}
