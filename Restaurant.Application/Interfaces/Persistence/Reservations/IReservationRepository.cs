using Restaurant.Domain.Entities;

namespace Restaurant.Application.Interfaces.Persistence.Reservations
{
    public interface IReservationRepository
    {
        Task<IReadOnlyCollection<Reservation>> GetAllAsync();

        Task<Reservation?> GetByIdAsync(int id);

        Task<Reservation?> GetForUpdateAsync(int id);

        Task<Reservation?> CreateReservation(Reservation newReservation);

        Task<bool> UpdateReservationAsync(Reservation updatedReservation);

        Task<bool> DeleteReservationAsync(Reservation deletedReservation);

        Task<IReadOnlyCollection<Reservation>> FindConflictingReservationAsync(int tableId, DateTime reservationTime, int? reservationId = null);

    }
}

