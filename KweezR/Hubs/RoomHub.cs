using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Services.Contract;
using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoomHub : Hub<IRoomClient>
    {
        private readonly IServiceManager manager;
        private readonly UserManager<User> userManager;

        public RoomHub(IServiceManager manager, UserManager<User> userManager)
        {
            this.manager = manager;
            this.userManager = userManager;
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

        public async Task CreateRoom(CreateRoomDto room)
        {
            var UserName = Context.User?.Identity?.Name;
            var user = await userManager.FindByNameAsync(UserName!);

            room.OwnerId = user?.Id;
            var Id = await manager.Rooms.CreateRoomDto(room);

            await Clients.Caller.SendId(Id);
        } 
    }
}
