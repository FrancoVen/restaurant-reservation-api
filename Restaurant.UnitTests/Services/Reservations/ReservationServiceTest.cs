using AutoMapper;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Application.Interfaces.Persistence.Reservations;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Application.Services.Reservations;
using Restaurant.Application.Services.Reservations.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.UnitTests.TestFactories;

namespace Restaurant.UnitTests.Services.Reservations
{
    public class ReservationServiceTest
    {
        private readonly ILogger<ReservationService> _logger;
        private readonly IMapper _mapper;
        private readonly IReservationRepository _repository;
        private readonly IReservationValidator _reservationValidator;
        private readonly ICustomerRepository _customerRep;
        private readonly ITableRepository _tableRep;

        public ReservationServiceTest()
        {
            _logger = Substitute.For<ILogger<ReservationService>>();
            _mapper = Substitute.For<IMapper>();
            _repository = Substitute.For<IReservationRepository>();
            _reservationValidator = Substitute.For<IReservationValidator>();
            _customerRep = Substitute.For<ICustomerRepository>();
            _tableRep = Substitute.For<ITableRepository>();
        }

        private ReservationService CreateSut() => new ReservationService(_logger, _mapper, _repository, _reservationValidator, _customerRep, _tableRep);




        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnReservations()
        {
            //Arrange
            var reservationsList = ReservationFactory.CreateReservationList();
            _repository.GetAllAsync().Returns(reservationsList);

            var reservationDTOList = ReservationFactory.CreateReservationDTOList();

            _mapper.Map<IReadOnlyCollection<ReservationDTO>>(reservationsList).Returns(reservationDTOList);
            var reservationSut = CreateSut();

            //Act
            var result = await reservationSut.GetAllAsync();

            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(reservationDTOList);

            _mapper.Received(1).Map<IReadOnlyCollection<ReservationDTO>>(reservationsList);
            await _repository.Received(1).GetAllAsync();
        }


        [Fact]
        public async Task GetByIdAsync_WhenReservationIsNull_ReturnNotFound()
        {
            //Arrange
            var reservationSearched = ReservationFactory.CreateReservation(1);

            _repository.GetByIdAsync(reservationSearched.Id).Returns(Task.FromResult<Reservation?>(null));

            var reservationSut = CreateSut();

            //Act
            var result = await reservationSut.GetByIdAsync(reservationSearched.Id);

            //Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Reservation.NotFound");

            _mapper.DidNotReceive().Map<ReservationDTO>(reservationSearched);
            await _repository.Received().GetByIdAsync(reservationSearched.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenReservationExists_ReturnReservationDTO()
        {
            //Arrange
            var reservationSearched = ReservationFactory.CreateReservation(1);

            _repository.GetByIdAsync(reservationSearched.Id).Returns(reservationSearched);

            var reservationDTO = ReservationFactory.CreateReservationDTO(1);

            _mapper.Map<ReservationDTO>(reservationSearched).Returns(reservationDTO);

            var reservationSut = CreateSut();

            //Act
            var result = await reservationSut.GetByIdAsync(reservationSearched.Id);

            //Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(reservationDTO);

            _mapper.Received().Map<ReservationDTO>(reservationSearched);
            await _repository.Received().GetByIdAsync(reservationSearched.Id);
        }


        [Fact]

        public async Task CreateAsync_WhenCustomerIsNull_ReturnNotFound()
        {
            //Arrange
            _customerRep.GetByIdAsync(1).Returns(Task.FromResult<Customer?>(null));
            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO(1);
            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.CreateAsync(reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.NotFound");

            await _customerRep.Received(1).GetByIdAsync(1);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }


        [Fact]
        public async Task CreateAsync_WhenTableIsNull_ReturnNotFound()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO(1);

            var customer = CustomerFactory.CreateCustomer(reservationCreationDTO.CustomerId);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(Task.FromResult<Table?>(null));

            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.CreateAsync(reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.NotFound");

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }


        [Fact]
        public async Task CreateAsync_WhenReservationTimeConflicts_ReturnConflict()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO();

            var customer = CustomerFactory.CreateCustomer(1);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            var table = TableFactory.CreateTable(reservationCreationDTO.TableId);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(table);

            _reservationValidator.ValidateReservationTimeAsync(reservationCreationDTO)
                .Returns(Task.FromResult<ErrorOr<Success>>(Error.Conflict("Reservation.Conflict")));



            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.CreateAsync(reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Reservation.Conflict");

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _reservationValidator.Received(1).ValidateReservationTimeAsync(reservationCreationDTO);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }


        [Fact]
        public async Task CreateAsync_WhenCalled_ReturnReservationDTO()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO();

            var customer = CustomerFactory.CreateCustomer(1);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            var table = TableFactory.CreateTable(reservationCreationDTO.TableId);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(table);

            _reservationValidator.ValidateReservationTimeAsync(reservationCreationDTO)
                .Returns(Task.FromResult<ErrorOr<Success>>(Result.Success));

            var reservation = ReservationFactory.CreateReservation(1, reservationCreationDTO.CustomerId, reservationCreationDTO.TableId, reservationCreationDTO.ReservationTime, reservationCreationDTO.Status);

            _mapper.Map<Reservation>(reservationCreationDTO).Returns(reservation);

            _repository.CreateReservation(reservation).Returns(reservation);

            var reservationDTO = ReservationFactory.CreateReservationDTO(reservation.Id);

            _mapper.Map<ReservationDTO>(reservation).Returns(reservationDTO);

            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.CreateAsync(reservationCreationDTO);

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(reservationDTO);
            _mapper.Received(1).Map<Reservation>(reservationCreationDTO);
            _mapper.Received(1).Map<ReservationDTO>(reservation);

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _reservationValidator.Received(1).ValidateReservationTimeAsync(reservationCreationDTO);
            await _repository.Received(1).CreateReservation(reservation);
        }



        [Fact]

        public async Task UpdateAsync_WhenCustomerIsNull_ReturnNotFound()
        {
            //Arrange
            _customerRep.GetByIdAsync(1).Returns(Task.FromResult<Customer?>(null));
            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO(1);
            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.UpdateAsync(1, reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Customer.NotFound");

            await _customerRep.Received(1).GetByIdAsync(1);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }



        [Fact]
        public async Task UpdateAsync_WhenTableIsNull_ReturnNotFound()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO(1);

            var customer = CustomerFactory.CreateCustomer(reservationCreationDTO.CustomerId);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(Task.FromResult<Table?>(null));

            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.UpdateAsync(1, reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.NotFound");

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }

        [Fact]
        public async Task UpdateAsync_WhenReservationTimeConflicts_ReturnConflict()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO();

            var customer = CustomerFactory.CreateCustomer(1);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            var table = TableFactory.CreateTable(reservationCreationDTO.TableId);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(table);

            _reservationValidator.ValidateReservationTimeAsync(reservationCreationDTO)
                .Returns(Task.FromResult<ErrorOr<Success>>(Error.Conflict("Reservation.Conflict")));



            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.UpdateAsync(1, reservationCreationDTO);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Reservation.Conflict");

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _reservationValidator.Received(1).ValidateReservationTimeAsync(reservationCreationDTO);
            await _repository.DidNotReceive().CreateReservation(Arg.Any<Reservation>());
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_ReturnUpdated()
        {
            //Arrange

            var reservationCreationDTO = ReservationFactory.CreateReservationCreationDTO();

            var customer = CustomerFactory.CreateCustomer(1);

            _customerRep.GetByIdAsync(reservationCreationDTO.CustomerId).Returns(customer);

            var table = TableFactory.CreateTable(reservationCreationDTO.TableId);

            _tableRep.GetByIdAsync(reservationCreationDTO.TableId).Returns(table);

            _reservationValidator.ValidateReservationTimeAsync(reservationCreationDTO)
                .Returns(Task.FromResult<ErrorOr<Success>>(Result.Success));

            var reservation = ReservationFactory.CreateReservation(1, reservationCreationDTO.CustomerId, reservationCreationDTO.TableId, reservationCreationDTO.ReservationTime, reservationCreationDTO.Status);

            _repository.GetByIdAsync(reservation.Id).Returns(reservation);

            var ReservationSut = CreateSut();

            //Act
            var result = await ReservationSut.UpdateAsync(1, reservationCreationDTO);

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(Result.Updated);

            _mapper.Received(1).Map(reservationCreationDTO, reservation);

            await _customerRep.Received(1).GetByIdAsync(reservationCreationDTO.CustomerId);
            await _tableRep.Received(1).GetByIdAsync(reservationCreationDTO.TableId);
            await _repository.Received(1).GetByIdAsync(reservation.Id);
            await _reservationValidator.Received(1).ValidateReservationTimeAsync(reservationCreationDTO);
            await _repository.Received(1).UpdateReservationAsync(reservation);
        }


        [Fact]

        public async Task DeleteAsync_WhenReservationIsNull_ReturnNotFound()
        {
            //Arrange
            int reservationId = 1;
            _repository.GetByIdAsync(reservationId).Returns(Task.FromResult<Reservation?>(null));

            var reservationSut = CreateSut();

            //Act

            var result = await reservationSut.DeleteAsync(reservationId);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Reservation.NotFound");

            await _repository.DidNotReceive().DeleteReservationAsync(Arg.Any<Reservation>());
        }


        [Fact]
        public async Task DeleteAsync_WhenReservationIsNotNull_ReturnDeleted()
        {
            //Arrange
            var reservationSearched = ReservationFactory.CreateReservation(1);

            _repository.GetByIdAsync(reservationSearched.Id).Returns(reservationSearched);

            var reservationSut = CreateSut();

            //Act

            var result = await reservationSut.DeleteAsync(reservationSearched.Id);

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(Result.Deleted);

            await _repository.Received().DeleteReservationAsync(reservationSearched);
        }

    }
}
