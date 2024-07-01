using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
    public interface IRoomService
    {
        Task<HubConnection> ConfigureHubConnection();
    }
}
