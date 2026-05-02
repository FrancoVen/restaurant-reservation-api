
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Persistence.Repositories.Tables
{
    public class TableRepository : ITableRepository
    {
        //“El repository decide QUÉ y CÓMO se consulta.
        // El consumidor solo recibe los datos ya materializados.


        private readonly ApplicationDbContext _context;

        public TableRepository(ApplicationDbContext context)
        {
            this._context = context;
        }


        public async Task<IReadOnlyCollection<Table>> GetAllAsync()
        {
            var tableList = await _context.Tables.AsNoTracking().ToListAsync();
            return tableList;
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            var table = await _context.Tables.AsNoTracking().Include(x => x.Reservations).ThenInclude(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id);

            return table;
        }

        public async Task<Table?> GetForUpdateAsync(int id)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(x => x.Id == id);
            return table;
        }

        public async Task<Table?> GetByTableNumberAsync(int tableNumber)
        {
            var table = await _context.Tables.AsNoTracking().FirstOrDefaultAsync(x => x.TableNumber == tableNumber);

            return table;
        }

        public async Task<Table?> CreateTableAsync(Table table)
        {
            _context.Tables.Add(table);

            await _context.SaveChangesAsync();

            return table;
        }

        public async Task<bool> UpdateTableAsync(Table table)
        {
            _context.Tables.Update(table);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTableAsync(Table deletedTable)
        {
            _context.Tables.Remove(deletedTable);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> ExistsTableNumberAsync(int tableNumber, int excludingId)
        {
            return await _context.Tables.AnyAsync(x => x.TableNumber == tableNumber && x.Id != excludingId);
        }

    }
}
