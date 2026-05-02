using FluentValidation;
using Restaurant.Application.Dtos.Users;

namespace Restaurant.Application.Validators
{
    public class UpdateUserRequestDTOValidator : AbstractValidator<UpdateUserRequestDTO>
    {
        public UpdateUserRequestDTOValidator()
        {
            RuleFor(request => request.Email).Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("The field 'Email' is required")
              .EmailAddress().WithMessage("The field 'Email' must be a valid email address");


            RuleFor(request => request.UserName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("The field 'Username' is required")
                .MinimumLength(3).WithMessage("The 'Username' field must be at least 3 characters long")
                .MaximumLength(20).WithMessage("The 'Username' must not exceed 25 characters")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Username can only contain alphabetic characters.");
        }
    }
}
