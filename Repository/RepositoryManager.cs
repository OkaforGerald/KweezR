using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext context;
        private readonly Lazy<IRoomRepository> roomRepository;

        public RepositoryManager(RepositoryContext context)
        {
            this.context = context;
            roomRepository = new Lazy<IRoomRepository>(new RoomRepository(context));
        }

        public IRoomRepository Rooms => roomRepository.Value;

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
