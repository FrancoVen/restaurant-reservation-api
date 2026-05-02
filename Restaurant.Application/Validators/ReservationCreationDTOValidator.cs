using FluentValidation;
using Restaurant.Application.Dtos.Reservations;


namespace Restaurant.Application.Validators
{
    public class ReservationCreationDTOValidator : AbstractValidator<ReservationCreationDTO>
    {
        public ReservationCreationDTOValidator()
        {
            RuleFor(reservation => reservation.CustomerId).
                NotEmpty().WithMessage("The field 'CustomerId' is required.").
                GreaterThan(0).WithMessage("CustomerId must be a positive number."); ;

            RuleFor(reservation => reservation.TableId).
                NotEmpty().WithMessage("The field 'TableId' is required.").
                GreaterThan(0).WithMessage("TableId must be a positive number."); ;

            RuleFor(reservation => reservation.ReservationTime).
                NotEmpty().WithMessage("The field 'ReservationTime' is required.").
                Must(time => time >= DateTime.UtcNow.AddMinutes(5)).WithMessage("The field 'ReservationTime' cannot be in the past.");

            RuleFor(reservation => reservation.Status).
                NotEmpty().WithMessage("The field 'Status' is required.").
                IsInEnum().WithMessage("The number entered for the field 'Status' is not valid. Allowed values are Pending (0), Confirmed (1), and Cancelled (2).");
        }
    }
}
