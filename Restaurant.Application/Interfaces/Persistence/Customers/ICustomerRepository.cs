using Restaurant.Domain.Entities;

namespace Restaurant.Application.Interfaces.Persistence.Customers
{
    public interface ICustomerRepository
    {
        Task<IReadOnlyCollection<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer?> GetForUpdateAsync(int id);

        Task<Customer?> CreateCustomer(Customer newCustomer);

        Task<bool> UpdateCustomerAsync(Customer updatedCustomer);

        Task<bool> DeleteCustomerAsync(Customer deletedCustomer);

        Task<bool> EmailExistsAsync(string email, int? excludingId);
    }
}
