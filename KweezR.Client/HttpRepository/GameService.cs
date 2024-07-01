using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
	public class GameService : IGameService
	{
		private readonly ILocalStorageService _localStorage;

        public GameService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<HubConnection> ConfigureHubConnection(Guid Id)
		{
            var token = await _localStorage.GetItemAsync<string>("authToken");

            HubConnection connection = new HubConnectionBuilder()
				.WithUrl($"https://localhost:7130/games?room={Id}", option =>
				{
					option.AccessTokenProvider = () => Task.FromResult(token);
				})
				.Build();

			return connection;
		}
	}
}
