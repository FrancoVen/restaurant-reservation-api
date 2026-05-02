using AutoMapper;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Restaurant.Application.Dtos.Customers;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Application.Services.Customers;
using Restaurant.Domain.Entities;
using Restaurant.UnitTests.TestFactories;


namespace Restaurant.UnitTests.Services.Customers
{
    public class CustomerServiceTest
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _repository;

        public CustomerServiceTest()
        {
            _repository = Substitute.For<ICustomerRepository>();
            _logger = Substitute.For<ILogger<CustomerService>>();
            _mapper = Substitute.For<IMapper>();
        }

        private CustomerService CreateSut() => new CustomerService(_logger, _mapper, _repository);

        [Fact]
        public async Task CreateAsync_WhenEmailAlreadyExists_ReturnConflict()
        {
            //Arrange
            var customerCreationDto = CustomerFactory.CreateCustomerCreationDto();


            _repository.EmailExistsAsync(customerCreationDto.Email, null).Returns(true);

            var customerSut = CreateSut();

            //Act
            var result = await customerSut.CreateAsync(customerCreationDto);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.Conflict");

            await _repository.DidNotReceive().CreateCustomer(Arg.Any<Customer>());
        }



        [Fact]
        public async Task CreateAsync_WhenEmailDoesNotExists_ReturnCustomerDto()
        {
            //Arrange
            var customerCreationDto = CustomerFactory.CreateCustomerCreationDto();

            _repository.EmailExistsAsync(customerCreationDto.Email, null).Returns(false);

            var customerToCreate = CustomerFactory.CreateCustomer(0);

            var persistedCustomer = CustomerFactory.CreateCustomer(7);

            _mapper.Map<Customer>(customerCreationDto).Returns(customerToCreate);

            _repository.CreateCustomer(customerToCreate).Returns(persistedCustomer);

            var expectedDto = CustomerFactory.CreateDto(7);

            _mapper.Map<CustomerDTO>(persistedCustomer).Returns(expectedDto);

            var customerSut = CreateSut();

            //Act

            var result = await customerSut.CreateAsync(customerCreationDto);

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedDto);

            await _repository.Received(1).CreateCustomer(customerToCreate);
        }


        [Fact]
        public async Task DeleteAsync_WhenCustomerIsNull_ReturnNotFound()
        {
            //Arrange
            _repository.GetByIdAsync(1).Returns(Task.FromResult<Customer?>(null));
            var customerSut = CreateSut();
            //Act
            var result = await customerSut.DeleteAsync(1);
            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.NotFound");

            await _repository.Received(1).GetByIdAsync(1);
            await _repository.DidNotReceive().DeleteCustomerAsync(Arg.Any<Customer>());
        }


        [Fact]
        public async Task DeleteAsync_WhenCustomerExists_ReturnDeleted()
        {
            //Arrange
            Customer customerSearched = CustomerFactory.CreateCustomer();
            _repository.GetByIdAsync(customerSearched.Id).Returns(customerSearched);
            var customerSut = CreateSut();

            //Act
            var result = await customerSut.DeleteAsync(customerSearched.Id);

            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);

            await _repository.Received(1).GetByIdAsync(customerSearched.Id);
            await _repository.Received(1).DeleteCustomerAsync(customerSearched);
        }


        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnCustomerList()
        {
            //Arrange
            IReadOnlyCollection<Customer> customerList = CustomerFactory.CreateCustomerList();
            _repository.GetAllAsync().Returns(customerList);

            IReadOnlyCollection<CustomerDTO> customerDtoList = CustomerFactory.CreateCustomerDTOList();

            _mapper.Map<IReadOnlyCollection<CustomerDTO>>(customerList).Returns(customerDtoList);

            var customerSut = CreateSut();

            //Act
            var result = await customerSut.GetAllAsync();
            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(customerDtoList);
            result.Value.First().FullName.Should().Be($"{customerList.First().Name} {customerList.First().LastName}");

            _mapper.Received(1).Map<IReadOnlyCollection<CustomerDTO>>(customerList);


            await _repository.Received(1).GetAllAsync();
        }


        [Fact]
        public async Task GetByIdAsync_WhenCustomerIsNull_ReturnNotFound()
        {
            //Arrange
            Customer customerSearched = CustomerFactory.CreateCustomer(1);

            _repository.GetByIdAsync(customerSearched.Id).Returns((Customer?)null);

            var customerSut = CreateSut();

            //Act
            var result = await customerSut.GetByIdAsync(customerSearched.Id);

            //Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.NotFound");

            await _repository.Received(1).GetByIdAsync(customerSearched.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCustomerExists_ReturnCustomerSearched()
        {
            //Arrange
            Customer customerSearched = CustomerFactory.CreateCustomer(1);

            _repository.GetByIdAsync(customerSearched.Id).Returns(customerSearched);

            var customerSut = CreateSut();

            var customerReservationDto = CustomerFactory.CreateCustomerReservationDto(customerSearched.Id);

            _mapper.Map<CustomerReservationDTO>(customerSearched).Returns(customerReservationDto);


            //Act
            var result = await customerSut.GetByIdAsync(customerSearched.Id);

            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(customerReservationDto);

            _mapper.Received(1).Map<CustomerReservationDTO>(customerSearched);
            await _repository.Received(1).GetByIdAsync(customerSearched.Id);
        }


        [Fact]

        public async Task UpdateAsync_WhenCustomerIsNull_ReturnNotFound()
        {
            //Arrange
            _repository.GetForUpdateAsync(1).Returns((Customer?)null);

            var customerSut = CreateSut();

            var customerCreationDto = CustomerFactory.CreateCustomerCreationDto();

            //Act
            var result = await customerSut.UpdateAsync(1, customerCreationDto);

            //Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.NotFound");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.DidNotReceive().EmailExistsAsync(Arg.Any<string>(), Arg.Any<int?>());
            _mapper.DidNotReceive().Map(customerCreationDto, Arg.Any<Customer>());
            await _repository.DidNotReceive().UpdateCustomerAsync(Arg.Any<Customer>());
        }


        [Fact]

        public async Task UpdateAsync_WhenEmailAlreadyExists_ReturnConflict()
        {
            //Arrange
            var customerSearched = CustomerFactory.CreateCustomer(1);

            var customerCreationDto = CustomerFactory.CreateCustomerCreationDto();

            _repository.GetForUpdateAsync(1).Returns(customerSearched);

            _repository.EmailExistsAsync(customerCreationDto.Email, 1).Returns(true);

            var customerSut = CreateSut();


            //Act
            var result = await customerSut.UpdateAsync(1, customerCreationDto);

            //Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.Conflict");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.Received(1).EmailExistsAsync(customerCreationDto.Email, 1);
            _mapper.DidNotReceive().Map(customerCreationDto, customerSearched);
            await _repository.DidNotReceive().UpdateCustomerAsync(Arg.Any<Customer>());
        }

        [Fact]

        public async Task UpdateAsync_WhenCustomerExistsAndEmailIsAvailable_ReturnUpdated()
        {
            //Arrange
            var customerSearched = CustomerFactory.CreateCustomer(1);

            var customerCreationDto = CustomerFactory.CreateCustomerCreationDto();

            _repository.GetForUpdateAsync(1).Returns(customerSearched);

            _repository.EmailExistsAsync(customerCreationDto.Email, 1).Returns(false);

            _repository.UpdateCustomerAsync(customerSearched).Returns(true);

            var customerSut = CreateSut();


            //Act
            var result = await customerSut.UpdateAsync(1, customerCreationDto);

            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(Result.Updated);

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.Received(1).EmailExistsAsync(customerCreationDto.Email, 1);
            _mapper.Received(1).Map(customerCreationDto, customerSearched);
            await _repository.Received(1).UpdateCustomerAsync(customerSearched);
        }

    }

}
