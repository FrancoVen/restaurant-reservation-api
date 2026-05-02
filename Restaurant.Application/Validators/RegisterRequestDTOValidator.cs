using FluentValidation;
using Restaurant.Application.Dtos.Auth;


namespace Restaurant.Application.Validators
{
    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {
            RuleFor(request => request.Email).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("The field 'Email' is required")
                .EmailAddress().WithMessage("The field 'Email' must be a valid email address");


            RuleFor(request => request.Username).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("The field 'Username' is required")
                .MinimumLength(3).WithMessage("The 'Username' field must be at least 3 characters long")
                .MaximumLength(20).WithMessage("The 'Username' must not exceed 25 characters")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Username can only contain alphabetic characters.");

            RuleFor(request => request.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(16).WithMessage("Password must not exceed 16 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
                .Must(p => !p.Contains(" ")).WithMessage("Password must not contain spaces.");
        }
    }
}
