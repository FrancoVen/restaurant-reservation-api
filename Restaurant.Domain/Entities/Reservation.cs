namespace Restaurant.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int TableId { get; set; }

        public DateTime ReservationTime { get; set; }

        public ReservationStatus Status { get; set; }


        //Navigation properties
        public Table Table { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

}
