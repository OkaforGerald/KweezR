namespace KweezR.Hubs
{
    public interface IGameClient
    {
        Task SendMessage(string message);
    }
}
