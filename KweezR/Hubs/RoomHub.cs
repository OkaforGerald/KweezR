using Microsoft.AspNetCore.SignalR;
using Services.Contract;
using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    public class RoomHub : Hub<IRoomClient>
    {
        private readonly IServiceManager manager;

        public RoomHub(IServiceManager manager)
        {
            this.manager = manager;
        }

        public async override Task OnConnectedAsync()
        {
            var rooms = await GetRoomsAndCount();

            await Clients.Caller.SendRooms(rooms);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        private async Task<List<RoomsDto>> GetRoomsAndCount()
        {
            var rooms = await manager.Rooms.GetRoomsAsync();

            foreach (var room in rooms)
            {
                bool IsExists = RoomHandler.RoomCapacities.TryGetValue(room.Id, out var currentCapacity);

                if (IsExists)
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
