using AutoMapper;
using Restaurant.Application.Dtos.Users;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Mappings
{
    internal class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserRequestDTO, User>();
        }
    }
}
