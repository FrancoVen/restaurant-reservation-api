using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Errors;
using Restaurant.Application.Dtos.Auth;
using Restaurant.Application.Services.Auth.Interface;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Handles user authentication and registration.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">User credentials.</param>
        /// <returns>JWT access token and expiration date.</returns>
        /// <response code="200">Authentication successful. Returns JWT token.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Invalid credentials.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Match<ActionResult<AuthResponseDto>>(success => Ok(success), errors => this.ToActionResult<AuthResponseDto>(errors));
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">User registration details.</param>
        /// <returns>JWT access token and expiration date.</returns>
        /// <response code="200">Registration successful. Returns JWT token.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="409">Email or username is already in use.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDTO request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Match<ActionResult<AuthResponseDto>>(success => Ok(success), errors => this.ToActionResult<AuthResponseDto>(errors));
        }
    }
}
