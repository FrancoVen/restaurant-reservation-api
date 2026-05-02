namespace Restaurant.Application.Common.Interfaces
{
    public interface IDateTimerProvider
    {
        DateTime UtcNow { get; }
    }
}
