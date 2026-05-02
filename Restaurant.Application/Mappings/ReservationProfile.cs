using AutoMapper;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Mappings
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, ReservationDTO>().ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.Customer!.Name} {src.Customer!.LastName}"))
                                                    .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => $"{src.Table!.TableNumber}"))
                                                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ReservationCreationDTO, Reservation>().ReverseMap();


        }
    }
}
