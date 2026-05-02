using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Dtos.Tables;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Application.Services.Tables.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services.Tables
{
    public class TableService : ITableService
    {
        private readonly ILogger<TableService> _logger;
        private readonly IMapper _mapper;
        private readonly ITableRepository _repository;

        public TableService(ILogger<TableService> logger, IMapper mapper, ITableRepository repository)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._repository = repository;
        }


        public async Task<ErrorOr<IReadOnlyCollection<TableDTO>>> GetAllAsync()
        {
            var tables = await _repository.GetAllAsync();

            _logger.LogInformation("GET - All tables were listed.");
            var tablesDTO = _mapper.Map<IReadOnlyCollection<TableDTO>>(tables);

            return tablesDTO.ToErrorOr();
        }

        public async Task<ErrorOr<TableReservationDTO>> GetByIdAsync(int id)
        {
            Table? tableSearched = await _repository.GetByIdAsync(id);

            if (tableSearched is null)
            {
                _logger.LogInformation("GET: Table with ID {id} was not found.", id);

                return Error.NotFound
                (
                     code: "Table.NotFound",
                     description: $"Table with ID {id} was not found."
                );

            }
            _logger.LogInformation("GET: Table with ID {id} was found.", id);

            var tableReservationDTO = _mapper.Map<TableReservationDTO>(tableSearched);

            return tableReservationDTO;
        }

        public async Task<ErrorOr<TableDTO>> CreateAsync(TableCreationDTO dto)
        {
            var searchedTable = await _repository.GetByTableNumberAsync(dto.TableNumber);

            if (searchedTable is not null)
            {
                _logger.LogWarning("POST: A table with number {TableNumber} already exists.", dto.TableNumber);
                return Error.Conflict("Table.Conflict", $"The table with number {dto.TableNumber} already exists ");
            }

            var newTable = _mapper.Map<Table>(dto);

            await _repository.CreateTableAsync(newTable);

            _logger.LogInformation("POST: Table (ID {Id}, Number {Number}) was created.", newTable.Id, newTable.TableNumber);

            return _mapper.Map<TableDTO>(newTable);
        }

        public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
        {
            Table? tableSearched = await _repository.GetForUpdateAsync(id);

            if (tableSearched is null)
            {
                _logger.LogWarning("DELETE: The table with ID {id} was not found for deletion", id);

                return Error.NotFound("Table.NotFound", $"The table with ID {id} you are trying to delete was not found.");

            }

            if (tableSearched.Reservations.Any())
            {
                _logger.LogWarning("DELETE: The table with ID {id} has active reservations and cannot be deleted.", id);

                return Error.Conflict("Table.Conflict", $"The table with ID {tableSearched.Id} has active reservations and cannot be deleted.");
            }

            await _repository.DeleteTableAsync(tableSearched);

            _logger.LogInformation("DELETE: The table with ID {Id} has been deleted successfully.", id);

            return Result.Deleted;
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(int id, TableCreationDTO dto)
        {
            Table? tableSearched = await _repository.GetForUpdateAsync(id);

            if (tableSearched is null)
            {
                _logger.LogWarning("PUT: Table with ID {id} was not found", id);

                return Error.NotFound("Table.NotFound", $"Table with ID {id} was not found. Update operation cannot be completed.");
            }


            bool tableDuplicate = await _repository.ExistsTableNumberAsync(dto.TableNumber, id);

            if (tableDuplicate)
            {
                _logger.LogWarning("PUT: A table with number {TableNumber} already exists.", dto.TableNumber);

                return Error.Conflict("Table.Conflict", $"Table number {dto.TableNumber} is already assigned to another table.");
            }

            _mapper.Map(dto, tableSearched);

            await _repository.UpdateTableAsync(tableSearched);

            _logger.LogInformation("PUT: Table with ID {Id} has been updated successfully.", id);

            return Result.Updated;
        }
    }
}
