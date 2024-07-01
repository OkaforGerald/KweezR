using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace KweezR.Client.Pages
{
	public partial class Game
	{
		[Parameter]
		public string Id { get; set; }
		private HubConnection? hubConnection;

		protected override async Task OnInitializedAsync()
		{
			hubConnection = await GameService.ConfigureHubConnection(Guid.Parse(Id));

			hubConnection.On<string>("SendMessage", (message) =>
			{
				Console.WriteLine(message);
			});

			await hubConnection.StartAsync();
		}

		public async ValueTask DisposeAsync()
		{
			await hubConnection!.DisposeAsync();
		}
	}
}
