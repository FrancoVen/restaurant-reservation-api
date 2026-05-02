using Restaurant.Application.Dtos.Reservations;

namespace Restaurant.Application.Dtos.Customers
{
    public class CustomerReservationDTO : CustomerDTO
    {
        public ICollection<ReservationDTO> Reservations { get; set; } = new List<ReservationDTO>();
    }
}
