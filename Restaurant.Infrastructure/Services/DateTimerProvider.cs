using Restaurant.Application.Common.Interfaces;

namespace Restaurant.Infrastructure.Services
{
    public class DateTimerProvider : IDateTimerProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
