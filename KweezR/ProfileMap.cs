using AutoMapper;
using Entities.Models;
using SharedAPI.TransferObjects;

namespace KweezR
{
	public class ProfileMap : Profile
	{
        public ProfileMap()
        {
            CreateMap<UserCreationDto, User>();
        }
    }
}
