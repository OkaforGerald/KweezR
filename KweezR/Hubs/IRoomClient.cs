using Entities.Models;
using SharedAPI.TransferObjects;

namespace KweezR.Hubs
{
    public interface IRoomClient
    {
        Task SendRooms(List<RoomsDto> Rooms);

        Task SendUpdate(List<RoomsDto> Rooms);
    }
}
