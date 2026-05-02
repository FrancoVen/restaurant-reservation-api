using FluentValidation;
using Restaurant.Application.Dtos.Tables;


namespace Restaurant.Application.Validators
{
    public class TableCreationDTOValidator : AbstractValidator<TableCreationDTO>
    {
        public TableCreationDTOValidator()
        {
            RuleFor(table => table.TableNumber).
                NotEmpty().WithMessage("The field 'TableNumber' is required.").
                GreaterThan(0).WithMessage("TableNumber must be greater than 0.").
                LessThan(1000).WithMessage("TableNumber must be less than 1000.");

            RuleFor(table => table.Capacity).
                NotEmpty().WithMessage("The field 'Capacity' is required.").
                GreaterThan(0).WithMessage("Capacity must be greater than 0.").
                LessThan(50).WithMessage("Capacity must be less than 1000.");
        }

    }
}
