using FluentValidation;
using Restaurant.Application.Dtos.Customers;

namespace Restaurant.Application.Validators
{
    public class CustomerCreationDTOValidator : AbstractValidator<CustomerCreationDTO>
    {
        public CustomerCreationDTOValidator()
        {
            RuleFor(customer => customer.Name).
                NotEmpty().WithMessage("The field 'Name' is required to register a customer.").
                MinimumLength(2).WithMessage("The 'Name' field must be at least 2 characters long.").
                Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").
                WithMessage("LastName can only contain alphabetic characters.");

            RuleFor(customer => customer.LastName).
                NotEmpty().WithMessage("The field 'LastName' is required to register a customer.").
                MinimumLength(2).WithMessage("The 'LastName' field must be at least 2 characters long.").
                Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").
                WithMessage("LastName can only contain alphabetic characters.");

            RuleFor(customer => customer.Email).
                NotEmpty().WithMessage("The field 'Email' is required.").
                EmailAddress().WithMessage("Invalid email format.").
                MaximumLength(60).WithMessage("Email cannot exceed 60 characters.");


        }
    }
}
