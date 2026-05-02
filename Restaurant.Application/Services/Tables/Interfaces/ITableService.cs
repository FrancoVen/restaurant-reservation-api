using ErrorOr;
using Restaurant.Application.Dtos.Tables;

namespace Restaurant.Application.Services.Tables.Interfaces
{
    public interface ITableService
    {
        Task<ErrorOr<IReadOnlyCollection<TableDTO>>> GetAllAsync();

        Task<ErrorOr<TableReservationDTO>> GetByIdAsync(int id);

        Task<ErrorOr<TableDTO>> CreateAsync(TableCreationDTO dto);

        Task<ErrorOr<Updated>> UpdateAsync(int id, TableCreationDTO dto);

        Task<ErrorOr<Deleted>> DeleteAsync(int id);
    }
}
