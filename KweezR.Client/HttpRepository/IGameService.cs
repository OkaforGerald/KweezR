using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
	public interface IGameService
	{
		HubConnection ConfigureHubConnection(string name);
	}
}
