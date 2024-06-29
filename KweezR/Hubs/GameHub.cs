using Microsoft.AspNetCore.SignalR;
using Services.Contract;
using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private IHubContext<RoomHub> roomContext { get; set; }
        private readonly IServiceManager manager;

        public GameHub(IHubContext<RoomHub> roomContext, IServiceManager manager)
        {
            this.roomContext = roomContext;
            this.manager = manager;
        }

        public async override Task OnConnectedAsync()
        {
            var roomName = Context?.GetHttpContext()?.Request.Query["room"];
            
            await Groups.AddToGroupAsync(Context!.ConnectionId, roomName!);

            AddToDictionary(roomName!, Context.ConnectionId);

            var rooms = await GetRoomsAndCount();

            await roomContext.Clients.All.SendAsync("SendUpdate", rooms);
            
            await Clients.Group(roomName!).SendMessage($"<SERVER>: WELCOME {Context.ConnectionId}");
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            string roomName = RemoveFromDictionary(Context.ConnectionId);

			var rooms = await GetRoomsAndCount();
			await roomContext.Clients.All.SendAsync("SendUpdate", rooms);

			await Clients.Group(roomName).SendMessage($"<SERVER>: {Context.ConnectionId} has left the server");
        }

        private void AddToDictionary(string room, string Id)
        {
            bool Exists = RoomHandler.RoomCapacities.TryGetValue(room, out var capacities);

            if(Exists)
            {
                capacities?.Add(Id);
                RoomHandler.RoomCapacities.TryAdd(room, capacities!);
            }
            else
            {
                capacities = new List<string> { Id };
                RoomHandler.RoomCapacities.TryAdd(room, capacities);
            }
        }

        private string RemoveFromDictionary(string Id)
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
                bool IsExists = RoomHandler.RoomCapacities.TryGetValue(room!.Name!, out var currentCapacity);

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
    }
}
