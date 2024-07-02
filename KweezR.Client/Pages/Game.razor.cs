using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using SharedAPI.TransferObjects;

namespace KweezR.Client.Pages
{
	public partial class Game
	{
		[Parameter]
		public string Id { get; set; }
		private LobbyDto? Lobby { get; set; }
		private string? message { get; set; }
		private HubConnectionState State { get; set; }
		private List<string>? Messages { get; set; } = new List<string>();
		private HubConnection? hubConnection;

		protected override async Task OnInitializedAsync()
		{
			hubConnection = await GameService.ConfigureHubConnection(Guid.Parse(Id));

			hubConnection.On<string>("SendMessage", (message) =>
			{
				Messages!.Add(message);
				StateHasChanged();
			});

			hubConnection.On<LobbyDto>("SendLobbyDetails", (lobby) =>
			{
				Lobby = lobby;
				StateHasChanged();
			});

			await hubConnection.StartAsync();
		}

		public async Task BroadcastMessage()
		{
			await hubConnection!.SendAsync("BroadcastMessage", message, Id);
			message = "";
		}

        public async Task Enter(KeyboardEventArgs e)
        {
			if(e.Code == "Enter" || e.Code == "NumpadEnter")
			{
                await hubConnection!.SendAsync("BroadcastMessage", message, Id);
                message = "";
            }
        }

        public async ValueTask DisposeAsync()
		{
			await hubConnection!.DisposeAsync();
		}
	}
}
