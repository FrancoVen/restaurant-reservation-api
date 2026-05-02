using Restaurant.Application.Dtos.Reservations;
using Restaurant.Domain.Entities;


namespace Restaurant.UnitTests.TestFactories
{
    public static class ReservationFactory
    {
        private const int DefaultId = 1;

        private const int DefaultCustomerId = 1;

        private const int DefaultTableId = 1;

        private const ReservationStatus Status = ReservationStatus.Pending;


        public static Reservation CreateReservation(int id = DefaultId, int customerId = DefaultCustomerId, int tableId = DefaultTableId, DateTime? reservationTimer = null, ReservationStatus status = Status) => new()
        {
            Id = id,
            CustomerId = customerId,
            TableId = tableId,
            ReservationTime = reservationTimer ?? DateTime.UtcNow,
            Status = status
        };


        public static ReservationCreationDTO CreateReservationCreationDTO(int customerId = DefaultCustomerId, int tableId = DefaultTableId, DateTime? reservationTimer = null, ReservationStatus status = Status) => new()
        {
            CustomerId = customerId,
            TableId = tableId,
            ReservationTime = reservationTimer ?? DateTime.UtcNow,
            Status = status
        };


        public static ReservationDTO CreateReservationDTO(int id = DefaultId, string? customerName = null, int? tableNumber = null, DateTime? reservationTimer = null, ReservationStatus status = Status) => new()
        {
            Id = id,
            CustomerName = customerName ?? "Test123",
            TableNumber = tableNumber ?? 1,
            ReservationTime = reservationTimer ?? DateTime.UtcNow,
            Status = status.ToString()
        };

        public static List<Reservation> CreateReservationList() => new()
        {
            CreateReservation(1,2,3),
            CreateReservation(2,3,4),
            CreateReservation(3,4,5)
        };

        public static List<ReservationDTO> CreateReservationDTOList() => new()
        {
            CreateReservationDTO(1,"Marcelo Tapia", 3),
            CreateReservationDTO(2,"Leandro Perez", 4),
            CreateReservationDTO(3,"Mauro Garcia", 5)
        };
    }
}
