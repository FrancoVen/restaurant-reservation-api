namespace Restaurant.Application.Dtos.Reservations
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }


        public int TableNumber { get; set; }

        public DateTime ReservationTime { get; set; }

        public required string Status { get; set; }
    }
}
