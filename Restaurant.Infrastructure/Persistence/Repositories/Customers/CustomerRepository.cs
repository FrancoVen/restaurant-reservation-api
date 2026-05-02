using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;


namespace Restaurant.Infrastructure.Persistence.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IReadOnlyCollection<Customer>> GetAllAsync()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var searchedCustomer = await _context.Customers.AsNoTracking().Include(x => x.Reservations).ThenInclude(x => x.Table).FirstOrDefaultAsync(x => x.Id == id);

            return searchedCustomer;
        }

        public async Task<Customer?> GetForUpdateAsync(int id)
        {
            var searchedCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return searchedCustomer;
        }

        public async Task<Customer?> CreateCustomer(Customer newCustomer)
        {
            _context.Customers.Add(newCustomer);

            await _context.SaveChangesAsync();

            return newCustomer;
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludingId = null)
        {
            if (excludingId is null) return await _context.Customers.AnyAsync(c => c.Email == email);

            return await _context.Customers.AnyAsync(x => x.Email == email && x.Id != excludingId);
        }

        public async Task<bool> UpdateCustomerAsync(Customer updatedCustomer)
        {
            _context.Customers.Update(updatedCustomer);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCustomerAsync(Customer deletedCustomer)
        {
            _context.Customers.Remove(deletedCustomer);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
