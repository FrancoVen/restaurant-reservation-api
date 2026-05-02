using Restaurant.Domain.Entities;

namespace Restaurant.Application.Interfaces.Persistence.Tables
{
    public interface ITableRepository
    {
        Task<IReadOnlyCollection<Table>> GetAllAsync();

        Task<Table?> GetByIdAsync(int id);

        Task<Table?> GetForUpdateAsync(int id);

        Task<Table?> GetByTableNumberAsync(int tableNumber);

        Task<Table?> CreateTableAsync(Table newTable);

        Task<bool> UpdateTableAsync(Table updatedTable);

        Task<bool> DeleteTableAsync(Table deletedTable);

        Task<bool> ExistsTableNumberAsync(int tableNumber, int excludingId);
    }
}
