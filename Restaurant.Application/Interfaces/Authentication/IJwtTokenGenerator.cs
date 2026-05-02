using Restaurant.Domain.Entities;


namespace Restaurant.Application.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        (string, DateTime) GenerateToken(User user);
    }
}
