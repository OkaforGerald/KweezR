using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
	public class GameService : IGameService
	{
		public HubConnection ConfigureHubConnection(string name)
		{
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl($"https://localhost:7130/games?room={name}")
				.Build();

			return connection;
		}
	}
}
