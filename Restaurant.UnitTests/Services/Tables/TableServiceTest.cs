using AutoMapper;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Restaurant.Application.Dtos.Tables;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Application.Services.Tables;
using Restaurant.Domain.Entities;
using Restaurant.UnitTests.TestFactories;

namespace Restaurant.UnitTests.Services.Tables
{
    public class TableServiceTest
    {
        private readonly ILogger<TableService> _logger;
        private readonly IMapper _mapper;
        private readonly ITableRepository _repository;

        public TableServiceTest()
        {
            _logger = Substitute.For<ILogger<TableService>>();
            _mapper = Substitute.For<IMapper>();
            _repository = Substitute.For<ITableRepository>();
        }

        public TableService CreateSut() => new TableService(_logger, _mapper, _repository);

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnTablesList()
        {
            //Arrange
            var tablesList = TableFactory.CreateTableList();

            _repository.GetAllAsync().Returns(Task.FromResult<IReadOnlyCollection<Table>>(tablesList));

            var tablesDTOList = TableFactory.CreateTableDTOList();

            _mapper.Map<IReadOnlyCollection<TableDTO>>(tablesList).Returns(tablesDTOList);

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.GetAllAsync();

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(tablesDTOList);
            _mapper.Received(1).Map<IReadOnlyCollection<TableDTO>>(tablesList);

            await _repository.Received(1).GetAllAsync();
        }


        [Fact]

        public async Task GetByIdAsync_WhenTableIsNull_ReturnNotFound()
        {
            //Arrange

            _repository.GetByIdAsync(1).Returns(Task.FromResult<Table?>(null));

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.GetByIdAsync(1);

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.NotFound");

            _mapper.DidNotReceive().Map<TableReservationDTO>(Arg.Any<Table>());
            await _repository.Received(1).GetByIdAsync(1);

        }

        [Fact]
        public async Task GetByIdAsync_WhenTableExists_ReturnTable()
        {
            //Arrange
            var tableSearched = TableFactory.CreateTable(1);

            _repository.GetByIdAsync(1).Returns(Task.FromResult<Table?>(tableSearched));

            var tableReservationDTO = TableFactory.CreateTableReservationDTO();

            _mapper.Map<TableReservationDTO>(tableSearched).Returns(tableReservationDTO);

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.GetByIdAsync(1);

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(tableReservationDTO);
            _mapper.Received(1).Map<TableReservationDTO>(tableSearched);

            await _repository.Received(1).GetByIdAsync(1);

        }

        [Fact]
        public async Task CreateAsync_WhenTableAlreadyExists_ReturnConflict()
        {
            //Arrange
            var tableCreationDTO = TableFactory.CreateTableCreationDTO(1);
            var table = TableFactory.CreateTable(1);

            _repository.GetByTableNumberAsync(tableCreationDTO.TableNumber).Returns(Task.FromResult<Table?>(table));

            var tableSut = CreateSut();

            //Act
            var result = await tableSut.CreateAsync(tableCreationDTO);
            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.Conflict");
            _mapper.DidNotReceive().Map<Table>(Arg.Any<TableCreationDTO>());
            await _repository.DidNotReceive().CreateTableAsync(Arg.Any<Table>());
            await _repository.Received(1).GetByTableNumberAsync(tableCreationDTO.TableNumber);
        }


        [Fact]
        public async Task CreateAsync_WhenTableDoesNotExist_ReturnTableDTO()
        {
            //Arrange
            var tableCreationDTO = TableFactory.CreateTableCreationDTO();
            var table = TableFactory.CreateTable(1);
            var tableDTO = TableFactory.CreateTableDTO(1);

            _repository.GetByTableNumberAsync(tableCreationDTO.TableNumber).Returns(Task.FromResult<Table?>(null));

            _mapper.Map<Table>(tableCreationDTO).Returns(table);

            _mapper.Map<TableDTO>(table).Returns(tableDTO);

            var tableSut = CreateSut();

            //Act
            var result = await tableSut.CreateAsync(tableCreationDTO);
            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(tableDTO);
            _mapper.Received(1).Map<Table>(tableCreationDTO);
            _mapper.Received(1).Map<TableDTO>(table);
            await _repository.Received(1).CreateTableAsync(table);
            await _repository.Received(1).GetByTableNumberAsync(tableCreationDTO.TableNumber);
        }

        [Fact]

        public async Task DeleteAsync_WhenTableIsNull_ReturnNotFound()
        {
            //Arrange
            _repository.GetForUpdateAsync(1).Returns(Task.FromResult<Table?>(null));

            var tableSut = CreateSut();

            //Act
            var result = await tableSut.DeleteAsync(1);
            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.NotFound");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.DidNotReceive().DeleteTableAsync(Arg.Any<Table>());
        }


        [Fact]
        public async Task DeleteAsync_WhenTableHasReservations_ReturnConflict()
        {
            //Arrange
            var tableSearched = TableFactory.CreateTable(1);
            tableSearched.Reservations.Add(new Reservation());

            _repository.GetForUpdateAsync(1).Returns(tableSearched);

            var tableSut = CreateSut();

            //Act
            var result = await tableSut.DeleteAsync(1);
            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.Conflict");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.DidNotReceive().DeleteTableAsync(Arg.Any<Table>());
        }


        [Fact]
        public async Task DeleteAsync_WhenTableExistsAndHasNoReservations_ReturnDeleted()
        {
            //Arrange
            var tableSearched = TableFactory.CreateTable(1);
            _repository.GetForUpdateAsync(1).Returns(tableSearched);

            var tableSut = CreateSut();

            //Act
            var result = await tableSut.DeleteAsync(1);
            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.Received().DeleteTableAsync(tableSearched);
        }

        [Fact]

        public async Task UpdateAsync_WhenTableIsNull_ReturnNotFound()
        {
            //Arrange
            _repository.GetForUpdateAsync(1).Returns(Task.FromResult<Table?>(null));

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.UpdateAsync(1, new TableCreationDTO());

            //Assert

            result.IsError.Should().BeTrue();

            result.FirstError.Code.Should().Be("Table.NotFound");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.DidNotReceive().UpdateTableAsync(Arg.Any<Table>());
        }

        [Fact]

        public async Task UpdateAsync_WhenTableIsDuplicated_ReturnConflict()
        {
            //Arrange
            var tableSearched = TableFactory.CreateTable(1);
            _repository.GetForUpdateAsync(1).Returns(tableSearched);

            _repository.ExistsTableNumberAsync(tableSearched.TableNumber, tableSearched.Id).Returns(true);

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.UpdateAsync(1, new TableCreationDTO() { TableNumber = tableSearched.TableNumber, Capacity = 5 });

            //Assert

            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Table.Conflict");

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.Received(1).ExistsTableNumberAsync(tableSearched.TableNumber, tableSearched.Id);
            await _repository.DidNotReceive().UpdateTableAsync(Arg.Any<Table>());
        }


        [Fact]

        public async Task UpdateAsync_WhenValidRequest_ReturnUpdated()
        {
            //Arrange
            var tableSearched = TableFactory.CreateTable(1);
            _repository.GetForUpdateAsync(1).Returns(tableSearched);

            _repository.ExistsTableNumberAsync(tableSearched.TableNumber, tableSearched.Id).Returns(false);

            var tableSut = CreateSut();

            //Act

            var result = await tableSut.UpdateAsync(1, new TableCreationDTO() { TableNumber = tableSearched.TableNumber, Capacity = 5 });

            //Assert

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(Result.Updated);

            await _repository.Received(1).GetForUpdateAsync(1);
            await _repository.Received(1).ExistsTableNumberAsync(tableSearched.TableNumber, tableSearched.Id);
            await _repository.Received(1).UpdateTableAsync(tableSearched);
        }


    }
}
