using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.HttpRepository
{
	public interface IGameService
	{
		Task<HubConnection> ConfigureHubConnection(Guid Id);
	}
}
