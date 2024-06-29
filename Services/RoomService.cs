using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Services.Contract;
using SharedAPI.TransferObjects;

namespace Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepositoryManager manager;

        public RoomService(IRepositoryManager manager)
        {
            this.manager = manager;
        }

        public async Task<List<RoomsDto>> GetRoomsAsync()
        {
            var rooms = await manager.Rooms.GetAllRooms(false);

            var response = rooms.Select(x => new RoomsDto
            {
                Name = x.Name,
                Access = x.Access.ToString(),
                Category = x.Category.ToString(),
                NumberOfQuestions = x.NumberOfQuestions,
                MaxCapacity = x.MaxCapacity,
            }).ToList();

            return response;
        }
    }
}
