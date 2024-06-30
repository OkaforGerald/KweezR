using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateRoom(Room room)
        {
            Create(room);
        }

        public void DeleteRoom(Room room)
        {
            Delete(room);
        }

        public async Task<List<Room>> GetAllRooms(bool trackChanges)
        {
            return await FindAll(trackChanges).ToListAsync();
        }

		public async Task<Room> GetRoomById(Guid Id, bool trackChanges)
		{
			return await FindByCondition(x => x.Id.Equals(Id), trackChanges).FirstOrDefaultAsync();
		}
	}
}
