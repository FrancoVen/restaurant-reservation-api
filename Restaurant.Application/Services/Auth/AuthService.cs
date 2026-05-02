using ErrorOr;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Dtos.Auth;
using Restaurant.Application.Interfaces.Authentication;
using Restaurant.Application.Interfaces.Persistence.Users;
using Restaurant.Application.Services.Auth.Interface;

namespace Restaurant.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, ILogger<AuthService> logger)
        {
            this._userRepository = userRepository;
            this._tokenGenerator = tokenGenerator;
            this._logger = logger;
        }


        public async Task<ErrorOr<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var (user, isValid) = await _userRepository.ValidateUserAsync(request.Email, request.Password);

            if (user is null || !isValid)
            {
                _logger.LogWarning("Failed login attempt for {Email}", request.Email);
                return Error.Unauthorized("Auth.Unauthorized", "Invalid credentials");
            }

            var (token, expires) = _tokenGenerator.GenerateToken(user);

            var response = new AuthResponseDto()
            {
                AccessToken = token,
                ExpiresAtUtc = expires
            };

            return response;
        }

        public async Task<ErrorOr<AuthResponseDto>> RegisterAsync(RegisterRequestDTO request)
        {
            var searchedUser = await _userRepository.EmailExistsAsync(request.Email);

            if (searchedUser)
            {
                return Error.Conflict("User.Conflict", $"An account with this email already exists.");
            }

            var userNameVerifier = await _userRepository.UsernameExistsAsync(request.Username);

            if (userNameVerifier)
            {
                return Error.Conflict("User.Conflict", "Username is already in use");
            }

            var user = await _userRepository.CreateUserAsync(request.Email, request.Password, request.Username);

            if (user is null)
            {
                _logger.LogError("Registration failed: Internal data inconsistency for {Email}", request.Email);
                return Error.Failure("Auth.Failure", "The user could not be processed");
            }


            _logger.LogInformation("Registration successful for user: {Email}", request.Email);

            var (token, expires) = _tokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                AccessToken = token,
                ExpiresAtUtc = expires
            };
        }

    }
}
