using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IRoomRepository
    {
        void CreateRoom(Room room);

        void DeleteRoom(Room room);

        Task<List<Room>> GetAllRooms(bool trackChanges);
    }
}
