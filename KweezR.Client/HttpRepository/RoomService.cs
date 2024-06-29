using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
    public class RoomService : IRoomService
    {
        public RoomService() { }

        public HubConnection ConfigureHubConnection()
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7130/rooms")
                .Build();

            return connection;
        }
    }
}
