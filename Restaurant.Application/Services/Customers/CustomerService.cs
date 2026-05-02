using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Dtos.Customers;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Application.Services.Customers.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _repository;

        public CustomerService(ILogger<CustomerService> logger, IMapper mapper, ICustomerRepository repository)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._repository = repository;
        }

        public async Task<ErrorOr<CustomerDTO>> CreateAsync(CustomerCreationDTO dto)
        {
            var emailAlreadyExists = await _repository.EmailExistsAsync(dto.Email, null);


            if (emailAlreadyExists)
            {
                _logger.LogWarning("POST: Customer with email {Email} already exists", dto.Email);

                return Error.Conflict("Customer.Conflict", $"The customer with email {dto.Email} already exists.");
            }

            Customer customer = _mapper.Map<Customer>(dto);

            var createdCustomer = await _repository.CreateCustomer(customer);

            _logger.LogInformation("POST: Customer with ID {CustomerId} created successfully.", createdCustomer!.Id);

            return _mapper.Map<CustomerDTO>(createdCustomer);
        }

        public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
        {
            Customer? searchedCustomer = await _repository.GetByIdAsync(id);

            if (searchedCustomer is null)
            {
                _logger.LogWarning("DELETE: Customer with ID {id} was not found for deletion.", id);

                return Error.NotFound("Customer.NotFound", $"The customer with ID {id} was not found");
            }

            await _repository.DeleteCustomerAsync(searchedCustomer);

            _logger.LogInformation("DELETE: Customer with ID {id} deleted successfully.", id);
            return Result.Deleted;
        }

        public async Task<ErrorOr<IReadOnlyCollection<CustomerDTO>>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();

            _logger.LogInformation("GET: All customers were obtained from the database.");

            var customersDTO = _mapper.Map<IReadOnlyCollection<CustomerDTO>>(customers);

            return customersDTO.ToErrorOr();
        }

        public async Task<ErrorOr<CustomerReservationDTO>> GetByIdAsync(int id)
        {
            Customer? customerSearched = await _repository.GetByIdAsync(id);

            if (customerSearched is null)
            {
                _logger.LogWarning("GET: Customer with ID {id} was not found.", id);

                return Error.NotFound("Customer.NotFound", $"The customer with ID {id} was not found");
            }

            _logger.LogInformation("GET: Customer with ID {id} was obtained.", id);

            return _mapper.Map<CustomerReservationDTO>(customerSearched);
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(int id, CustomerCreationDTO dto)
        {
            Customer? searchedCustomer = await _repository.GetForUpdateAsync(id);

            if (searchedCustomer is null)
            {
                _logger.LogWarning("PUT: Customer with ID {id} was not found for update", id);

                return Error.NotFound("Customer.NotFound", $"The customer with ID {id} was not found for update");
            }

            var emailVerifier = await _repository.EmailExistsAsync(dto.Email, id);

            if (emailVerifier)
            {
                _logger.LogWarning("This email address '{Email}' is already in use", dto.Email);

                return Error.Conflict("Customer.Conflict", $"The email address '{dto.Email}' is already in use");
            }

            _mapper.Map(dto, searchedCustomer);
            await _repository.UpdateCustomerAsync(searchedCustomer);

            _logger.LogInformation("PUT: Customer with ID {id} updated successfully.", id);
            return Result.Updated;
        }
    }
}
