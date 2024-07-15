using System.Diagnostics;
using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using SharedAPI.TransferObjects;
using Timer = System.Timers.Timer;

namespace KweezR.Client.Pages
{
	public partial class Game
	{
		[Parameter]
		public string Id { get; set; }
        public string Username { get; set; }
		private LobbyDto? Lobby { get; set; }
		private string? message { get; set; }
		private HubConnectionState State { get; set; }
		private List<string>? Messages { get; set; } = new List<string>();
		private HubConnection? hubConnection;
		private GameState GameState { get; set; } = GameState.Lobby;
        private int QuestionRound { get; set; } = 1;
        private bool Delay { get; set; }
		private int ElapsedTime { get; set; } = 5;
        Timer timer;
        private string CurrentQuestion { get; set; }
        private List<string> CurrentOptions { get; set; } = new List<string> ();
        private QuestionDto? AllQuestions { get; set; }
        private Dictionary<string, int> PlayerScores { get; set; } = new Dictionary<string, int>();

        protected override async Task OnInitializedAsync()
		{
			hubConnection = await GameService.ConfigureHubConnection(Guid.Parse(Id));

			hubConnection.On<string>("SendMessage", (message) =>
			{
				Messages!.Add(message);
				StateHasChanged();
			});

            hubConnection.On<string>("SendPlayerName", (username) =>
            {
                Username = username;
            });

			hubConnection.On<LobbyDto>("SendLobbyDetails", (lobby) =>
			{
				Lobby = lobby;
                StateHasChanged();
			});

			hubConnection.On<QuestionDto>("StartGame", (questions) =>
			{
				GameState = GameState.InProgress;
                AllQuestions = questions;
                foreach (var player in Lobby!.Players!)
                {
                    PlayerScores.Add(player, 0);
                }
                StateHasChanged();
                Delay = true;
                StartTimer();
			});

            hubConnection.On<Tuple<string, int>>("SendScores", (PlayerAndScore) =>
            {
                PlayerScores[PlayerAndScore.Item1] = PlayerAndScore.Item2;
                StateHasChanged();
            });

			await hubConnection.StartAsync();
		}

		public async Task BroadcastMessage()
		{
			await hubConnection!.SendAsync("BroadcastMessage", message, Id);
			message = "";
            StateHasChanged();
        }

        public async Task Enter(KeyboardEventArgs e)
        {
			if(e.Code == "Enter" || e.Code == "NumpadEnter")
			{
                await hubConnection!.SendAsync("BroadcastMessage", message, Id);
                message = "";
            }
        }

        public void StartTimer()
        {
            timer = new Timer(1000);
            timer.Elapsed += CountDownTimer;
            timer.Enabled = true;
        }

        public void StartAnswerTimer()
        {
            timer = new Timer(100);
            timer.Elapsed += CountDownTimer;
            timer.Enabled = true;
        }

        public void CountDownTimer(Object? source, ElapsedEventArgs e)
        {
            if(ElapsedTime > 0)
            {
                ElapsedTime--;
            }
            else
            {
                if (Delay)
                {
                    ElapsedTime = 10;
                    Delay = false;
                    AssignNextQuestion();
                }
                else
                {
                    ElapsedTime = 5;
                    Delay = true;
                }
                timer.Enabled = false;
                timer.Dispose();
            }
            InvokeAsync(StateHasChanged);
        }

        public void AssignNextQuestion()
        {
            var rawQuest = AllQuestions!.Results[QuestionRound - 1];

            rawQuest.Incorrect_Answers!.Add(rawQuest.Correct_Answer!);

            var AllOptions = rawQuest.Incorrect_Answers.OrderBy(x => Guid.NewGuid()).ToList();

            CurrentQuestion = rawQuest.Question!;
            CurrentOptions = AllOptions;
            QuestionRound++;

            StartTimer();
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

        private async Task SelectAnswer(string option)
        {
            var currentQuestion = AllQuestions!.Results[QuestionRound - 2];
            if(option.Equals(currentQuestion.Correct_Answer, StringComparison.CurrentCultureIgnoreCase))
            {
                PlayerScores[Username] += 10;
                StateHasChanged();

                await hubConnection!.SendAsync("UpdateScore", Id, Username, PlayerScores[Username]);
            }

            CurrentOptions = Enumerable.Empty<string>().ToList();
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
