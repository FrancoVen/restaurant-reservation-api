using ErrorOr;
using Restaurant.Application.Dtos.Reservations;

namespace Restaurant.Application.Services.Reservations.Interfaces
{
    public interface IReservationService
    {
        Task<ErrorOr<IReadOnlyCollection<ReservationDTO>>> GetAllAsync();
        Task<ErrorOr<ReservationDTO>> GetByIdAsync(int id);

        Task<ErrorOr<ReservationDTO>> CreateAsync(ReservationCreationDTO dto);

        Task<ErrorOr<Updated>> UpdateAsync(int reservationId, ReservationCreationDTO dto);

        Task<ErrorOr<Deleted>> DeleteAsync(int id);
    }
}
