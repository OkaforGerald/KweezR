using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
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
                Id = x.Id,
                Name = x.Name,
                Access = x.Access.ToString(),
                Category = x.Category.ToString(),
                NumberOfQuestions = x.NumberOfQuestions,
                MaxCapacity = x.MaxCapacity,
            }).ToList();

            return response;
        }

		public async Task<RoomsDto> GetRoomByIdAsync(Guid Id)
		{
            var room = await manager.Rooms.GetRoomById(Id, false);

			var response = new RoomsDto
			{
				Id = room.Id,
				Name = room.Name,
				Access = room.Access.ToString(),
				Category = room.Category.ToString(),
				NumberOfQuestions = room.NumberOfQuestions,
				MaxCapacity = room.MaxCapacity,
			};

			return response;
		}

        public async Task<Guid> CreateRoomDto(CreateRoomDto room)
        {
            var newRoom = new Room
            {
                Name = room.Name,
                Access = room.Access,
                Category = room.Category,
                OwnerId = room.OwnerId,
                MaxCapacity = room.MaxCapacity,
                NumberOfQuestions = room.NumberOfQuestions,
                CreatedAt = DateTime.Now
            };

            if (room.Access == RoomAccess.Private)
            {
                var password = HashPassword(room.Password!, out byte[] salt);

                newRoom.Salt = Convert.ToBase64String(salt);
                newRoom.Hash = password;
            }

            manager.Rooms.CreateRoom(newRoom);
            await manager.SaveChangesAsync();

            return newRoom.Id;
        }

        private string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(64);

            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, 3000, HashAlgorithmName.SHA512, 64);

            return Convert.ToHexString(hash);
        }

        private bool VerifyPassword(string password, string saltString, string hashString)
        {
            var salt = Convert.FromBase64String(saltString);
            var hash = Convert.FromHexString(hashString);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, 3000, HashAlgorithmName.SHA512, 64);

            return CryptographicOperations.FixedTimeEquals(hash, newHash);
        }
	}
}
