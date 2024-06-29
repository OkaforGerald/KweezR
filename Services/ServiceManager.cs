using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Services.Contract;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IRoomService> roomService;

        public ServiceManager(IRepositoryManager manager)
        {
            this.roomService = new Lazy<IRoomService> (new RoomService(manager));
        }

        public IRoomService Rooms => roomService.Value;
    }
}
