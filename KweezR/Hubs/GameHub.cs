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
            var roomId = Guid.Parse(Context?.GetHttpContext()?.Request.Query["room"]!);

            var roomName = await manager.Rooms.GetRoomByIdAsync(roomId);

            await Groups.AddToGroupAsync(Context!.ConnectionId, roomName.Name!);

            AddToDictionary(roomName.Id!, Context.ConnectionId);

            var rooms = await GetRoomsAndCount();

            await roomContext.Clients.All.SendAsync("SendUpdate", rooms);
            
            await Clients.Group(roomName.Name!).SendMessage($"<SERVER>: WELCOME {Context.ConnectionId}");
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            Guid roomId = RemoveFromDictionary(Context.ConnectionId);

			var roomName = await manager.Rooms.GetRoomByIdAsync(roomId);
			var rooms = await GetRoomsAndCount();
			await roomContext.Clients.All.SendAsync("SendUpdate", rooms);

			await Clients.Group(roomName.Name!).SendMessage($"<SERVER>: {Context.ConnectionId} has left the server");
        }

        private void AddToDictionary(Guid roomId, string Id)
        {
            bool Exists = RoomHandler.RoomCapacities.TryGetValue(roomId, out var capacities);

            if(Exists)
            {
                capacities?.Add(Id);
                RoomHandler.RoomCapacities.TryAdd(roomId, capacities!);
            }
            else
            {
                capacities = new List<string> { Id };
                RoomHandler.RoomCapacities.TryAdd(roomId, capacities);
            }
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
    }
}
