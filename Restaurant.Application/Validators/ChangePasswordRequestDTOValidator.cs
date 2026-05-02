using FluentValidation;
using Restaurant.Application.Dtos.Users;


namespace Restaurant.Application.Validators
{
    public class ChangePasswordRequestDTOValidator : AbstractValidator<ChangePasswordRequestDTO>
    {
        public ChangePasswordRequestDTOValidator()
        {
            RuleFor(request => request.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(request => request.NewPassword).Cascade(CascadeMode.Stop)
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
