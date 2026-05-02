using AutoMapper;
using Restaurant.Application.Dtos.Customers;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerReservationDTO>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.LastName}"))
                                                         .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations));

            CreateMap<Customer, CustomerDTO>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.LastName}"));

            CreateMap<CustomerCreationDTO, Customer>();
        }
    }
}
