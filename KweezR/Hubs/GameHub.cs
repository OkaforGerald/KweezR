using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Services.Contract;
using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameHub : Hub<IGameClient>
    {
        private IHubContext<RoomHub> roomContext { get; set; }
        private readonly IServiceManager manager;
        HttpClient client = new HttpClient { BaseAddress = new Uri("https://opentdb.com/api.php") };

        public GameHub(IHubContext<RoomHub> roomContext, IServiceManager manager)
        {
            this.roomContext = roomContext;
            this.manager = manager;
        }

        public async override Task OnConnectedAsync()
        {
            var roomId = Guid.Parse(Context?.GetHttpContext()?.Request.Query["room"]!);

            var roomName = await manager.Rooms.GetRoomByIdAsync(roomId);

            await Groups.AddToGroupAsync(Context!.ConnectionId, roomName.Name!);

            await AddToDictionary(roomName.Id!, Context.User?.Identity?.Name);

            var rooms = await GetRoomsAndCount();

            await roomContext.Clients.All.SendAsync("SendUpdate", rooms);

            var lobbyDeets = GetLobbyInfo(roomName);

            await Clients.Group(roomName.Name!).SendMessage($"<SERVER>: WELCOME {Context?.User?.Identity?.Name}!");
            await Clients.Group(roomName.Name!).SendLobbyDetails(lobbyDeets);

            if(lobbyDeets.Players!.Count() == roomName.MaxCapacity)
            {
                await StartGame(roomName.Name!);
            }
        }

        public async Task BroadcastMessage(string message, Guid RoomId)
        {
            var room = await manager.Rooms.GetRoomByIdAsync(RoomId);

            await Clients.Group(room!.Name!).SendMessage($"<{Context.User?.Identity!.Name}>: {message}");
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            Guid roomId = RemoveFromDictionary(Context.User?.Identity?.Name);

			var roomName = await manager.Rooms.GetRoomByIdAsync(roomId);
			var rooms = await GetRoomsAndCount();
			await roomContext.Clients.All.SendAsync("SendUpdate", rooms);

			var lobbyDeets = GetLobbyInfo(roomName);

			await Clients.Group(roomName.Name!).SendMessage($"<SERVER>: {Context?.User?.Identity?.Name} has left the room!");
			await Clients.Group(roomName.Name!).SendLobbyDetails(lobbyDeets);
		}

        private async Task AddToDictionary(Guid roomId, string Id)
        {
            bool Exists = RoomHandler.RoomCapacities.TryGetValue(roomId, out var capacities);
            var room = await manager.Rooms.GetRoomByIdAsync(roomId);

            if (Exists)
            {
                capacities?.AddLast(Id);
                RoomHandler.RoomCapacities.TryAdd(roomId, capacities!);
            }
            else
            {
                capacities = new LinkedList<string>();
                capacities.AddFirst(Id);
                RoomHandler.RoomCapacities.TryAdd(roomId, capacities);
            }
        }

        private async Task StartGame(Guid roomId)
        {
            var roomName = await manager.Rooms.GetRoomByIdAsync(roomId);
            await GetQuestions(roomName);
            await Clients.Group(roomName.Name).StartGame();
        }

        private async Task GetQuestions(RoomsDto room)
        {
            //"https://opentdb.com/api.php?amount=10&category=12&difficulty=easy&type=multiple"
            //room.NumberOfQuestions
        }

        private Guid RemoveFromDictionary(string Id)
        {
            var list = RoomHandler.RoomCapacities.Where(x => x.Value.Contains(Id)).FirstOrDefault();

            list.Value.Remove(Id);

            return list.Key;
        }

        private async Task<List<RoomsDto>> GetRoomsAndCount()
        {
            var rooms = await manager.Rooms.GetRoomsAsync();

            foreach(var room in rooms)
            {
                bool IsExists = RoomHandler.RoomCapacities.TryGetValue(room.Id, out var currentCapacity);

                if(IsExists)
                {
                    room.CurrentCapacity = currentCapacity!.Count();
                }
                else
                {
                    room.CurrentCapacity = 0;
                }
            }

            return rooms;
        }

        private LobbyDto GetLobbyInfo(RoomsDto room)
        {
            bool IsExists = RoomHandler.RoomCapacities.TryGetValue(room.Id, out var currentCapacity);

            var lobby = new LobbyDto
            {
                Name = room.Name,
                Access = room.Access,
                Category = room.Category,
                Capacity = room.MaxCapacity,
                NumberOfQuestions = room.NumberOfQuestions
            };

            if (IsExists)
            {
                lobby.Players = currentCapacity.ToList();
                return lobby;
            }
            else
            {
                lobby.Players = Array.Empty<string>().ToList();
                return lobby;
            }
        }
    }
}
