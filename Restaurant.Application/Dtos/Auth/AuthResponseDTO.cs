
namespace Restaurant.Application.Dtos.Auth
{
    public class AuthResponseDto
    {
        public required string AccessToken { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}
