using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contract;

namespace Services
{
	public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IRoomService> roomService;
        private readonly Lazy<IAuthService> authService;

        public ServiceManager(IRepositoryManager manager, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            this.roomService = new Lazy<IRoomService> (new RoomService(manager));
            this.authService = new Lazy<IAuthService>(new AuthService(mapper, userManager, configuration));
        }

        public IRoomService Rooms => roomService.Value;

		public IAuthService Auth => authService.Value;
	}
}
