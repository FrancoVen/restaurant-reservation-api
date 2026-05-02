using AutoMapper;
using Restaurant.Application.Dtos.Tables;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Mappings
{
    public class TableProfile : Profile
    {
        public TableProfile()
        {
            CreateMap<Table, TableCreationDTO>().ReverseMap();

            CreateMap<Table, TableDTO>();

            CreateMap<Table, TableReservationDTO>().ForMember(dest => dest.Table, opt => opt.MapFrom(src => src))
                                                   .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations));
        }
    }
}
