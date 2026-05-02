using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces.Persistence.Reservations;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Persistence.Repositories.Reservations
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            this._context = context;

        }

        public async Task<Reservation?> CreateReservation(Reservation newReservation)
        {
            _context.Reservations.Add(newReservation);

            await _context.SaveChangesAsync();

            return newReservation;
        }

        public async Task<bool> DeleteReservationAsync(Reservation deletedReservation)
        {
            _context.Remove(deletedReservation);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IReadOnlyCollection<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.AsNoTracking().AsSplitQuery().Include(x => x.Table).Include(x => x.Customer).ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.AsNoTracking().AsSplitQuery().Include(x => x.Table).Include(x => x.Customer).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reservation?> GetForUpdateAsync(int id)
        {
            return await _context.Reservations.AsSplitQuery().Include(x => x.Table).Include(x => x.Customer).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> UpdateReservationAsync(Reservation updatedReservation)
        {
            _context.Update(updatedReservation);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IReadOnlyCollection<Reservation>> FindConflictingReservationAsync(int tableId, DateTime reservationTime, int? reservationId = null)
        {
            //Simplified logic, we will only check that there are 2 hours or more between reservations.
            var conflicts = _context.Reservations.AsNoTracking().
                            Where
                            (
                                  x => x.TableId == tableId
                                  && Math.Abs(EF.Functions.DateDiffMinute(x.ReservationTime, reservationTime)) < 120
                            );

            if (reservationId.HasValue) conflicts = conflicts.Where(x => x.Id != reservationId.Value);

            return await conflicts.ToListAsync();
        }

    }
}
