using Restaurant.Application.Dtos.Reservations;

namespace Restaurant.Application.Dtos.Tables
{
    public class TableReservationDTO
    {
        public required TableDTO Table { get; set; }
        public ICollection<ReservationDTO> Reservations { get; set; } = new List<ReservationDTO>();
    }
}
