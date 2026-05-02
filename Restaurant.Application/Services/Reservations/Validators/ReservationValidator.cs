using ErrorOr;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Application.Interfaces.Persistence.Reservations;
using Restaurant.Application.Services.Reservations.Interfaces;

namespace Restaurant.Application.Services.Reservations.Validators
{
    public class ReservationValidator : IReservationValidator
    {
        private readonly IReservationRepository _repository;

        public ReservationValidator(IReservationRepository repository)
        {
            this._repository = repository;
        }

        public async Task<ErrorOr<Success>> ValidateReservationTimeAsync(ReservationCreationDTO dto, int? reservationId = null)
        {
            var reservationChecker = await _repository.FindConflictingReservationAsync(dto.TableId, dto.ReservationTime, reservationId);

            if (reservationChecker.Any())
            {
                string reservationsCollision = string.Join(" - ", reservationChecker.Select(x => x.ReservationTime.ToString("HH:mm")));

                return Error.Conflict("Reservation.Conflict", $"The reservation time '{dto.ReservationTime:HH:mm}' conflicts with the following times: {reservationsCollision}.");
            }

            if (dto.ReservationTime <= DateTime.UtcNow.AddMinutes(5))
            {
                return Error.Validation("Reservation.Validation", $"Reservation time '{dto.ReservationTime:yyyy-MM-dd HH:mm}' must be at least 5 minutes in the future.");
            }

            return Result.Success;
        }

    }
}
