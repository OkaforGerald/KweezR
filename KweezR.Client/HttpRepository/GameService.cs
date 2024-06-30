using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
	public class GameService : IGameService
	{
		public HubConnection ConfigureHubConnection(Guid Id)
		{
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl($"https://localhost:7130/games?room={Id}")
				.Build();

			return connection;
		}
	}
}
