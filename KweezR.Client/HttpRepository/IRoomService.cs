using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
    public interface IRoomService
    {
        HubConnection ConfigureHubConnection();
    }
}
