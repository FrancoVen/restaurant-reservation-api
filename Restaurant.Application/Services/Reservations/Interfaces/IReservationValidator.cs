using ErrorOr;
using Restaurant.Application.Dtos.Reservations;

namespace Restaurant.Application.Services.Reservations.Interfaces
{
    public interface IReservationValidator
    {
        Task<ErrorOr<Success>> ValidateReservationTimeAsync(ReservationCreationDTO dto, int? reservationId = null);
    }
}
