using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
    public class RoomService : IRoomService
    {
        private ILocalStorageService _localStorage;
        public RoomService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<HubConnection> ConfigureHubConnection()
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7130/rooms", options =>
                {
                    options.AccessTokenProvider = async () => { return await _localStorage.GetItemAsync<string>("authToken"); };
                })
                .WithAutomaticReconnect()
                .Build();

            return connection;
        }
    }
}
