using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    public interface IGameClient
    {
        Task SendMessage(string message);

        Task SendLobbyDetails(LobbyDto lobby);

        Task StartGame(QuestionDto questionDto);

        Task SendPlayerName(string username);

        Task SendScores(Tuple<string, int> playerScores);
    }
}
