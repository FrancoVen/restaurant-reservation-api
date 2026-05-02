using ErrorOr;
using Restaurant.Application.Dtos.Auth;

namespace Restaurant.Application.Services.Auth.Interface
{
    public interface IAuthService
    {
        Task<ErrorOr<AuthResponseDto>> LoginAsync(LoginRequestDto request);


        Task<ErrorOr<AuthResponseDto>> RegisterAsync(RegisterRequestDTO request);

    }
}
