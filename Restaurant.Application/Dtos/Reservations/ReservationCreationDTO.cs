using Restaurant.Domain.Entities;

namespace Restaurant.Application.Dtos.Reservations
{
    public class ReservationCreationDTO
    {
        public int CustomerId { get; set; }

        public int TableId { get; set; }

        public DateTime ReservationTime { get; set; }

        public ReservationStatus Status { get; set; }
    }
}
