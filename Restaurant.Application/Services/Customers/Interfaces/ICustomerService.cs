using ErrorOr;
using Restaurant.Application.Dtos.Customers;

namespace Restaurant.Application.Services.Customers.Interfaces
{
    public interface ICustomerService
    {
        Task<ErrorOr<IReadOnlyCollection<CustomerDTO>>> GetAllAsync();
        Task<ErrorOr<CustomerReservationDTO>> GetByIdAsync(int id);
        Task<ErrorOr<CustomerDTO>> CreateAsync(CustomerCreationDTO dto);
        Task<ErrorOr<Updated>> UpdateAsync(int id, CustomerCreationDTO dto);
        Task<ErrorOr<Deleted>> DeleteAsync(int id);
    }
}
