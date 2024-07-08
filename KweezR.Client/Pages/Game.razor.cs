using System.Diagnostics;
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
		private GameState GameState { get; set; }
		private int ElapsedTime { get; set; }
        Stopwatch sw = new Stopwatch();
        private string CurrentQuestion { get; set; }
        private List<string> CurrentOptions { get; set; } = new List<string>();
        private Dictionary<string, int> PlayerScores { get; set; } = new Dictionary<string, int>();

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
				foreach(var player in Lobby!.Players!)
				{
					PlayerScores.Add(player, 0);
				}
                StateHasChanged();
			});

			hubConnection.On("StartGame", () =>
			{
				GameState = GameState.InProgress;
				StartTimer();
				StateHasChanged();
			});

			GameState = GameState.Lobby;

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

        private string GetPlayerBoxStyle(int index)
        {
            if (Lobby.Players.Count == 2)
            {
                return $"flex-basis: 45%;";
            }
            else if (Lobby.Players.Count == 3)
            {
                return index == 2 ? "flex-basis: 100%;" : "flex-basis: 45%;";
            }
            return "flex-basis: 45%;"; // For 4 players
        }

        private void StartTimer()
		{
			sw.Start();
			while(sw.ElapsedMilliseconds <= 10_000)
			{
				ElapsedTime = (int) Math.Floor(sw.ElapsedMilliseconds / (double) 1_000);
				StateHasChanged();
			}
		}

		private void StopTimer()
		{
			sw.Stop();
		}

        private void SelectAnswer(string option)
        {
            // Implement answer selection logic
        }

        public async ValueTask DisposeAsync()
		{
			await hubConnection!.DisposeAsync();
		}
	}

    public enum GameState
    {
        Lobby,
        InProgress,
        Finished
    }
}
