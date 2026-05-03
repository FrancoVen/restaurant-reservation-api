using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Errors;
using Restaurant.Application.Dtos.Roles;
using Restaurant.Application.Dtos.Users;
using Restaurant.Application.Services.Users.Interfaces;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Manages system users.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>List of all users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyCollection<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyCollection<UserDTO>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return result.Match<ActionResult<IReadOnlyCollection<UserDTO>>>(users => Ok(users), errors => this.ToActionResult<IReadOnlyCollection<UserDTO>>(errors));
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user matching the given ID.</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.Match<ActionResult<UserDTO>>(user => Ok(user), errors => this.ToActionResult<UserDTO>(errors));
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID to update.</param>
        /// <param name="request">Updated user details.</param>
        /// <response code="204">User updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        /// <response code="409">Username or email is already in use.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequestDTO request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return result.Match<IActionResult>(succes => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Deactivates a user by ID (soft delete).
        /// </summary>
        /// <param name="id">The user ID to deactivate.</param>
        /// <response code="204">User deactivated successfully.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            return result.Match<IActionResult>(succes => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="request">Role to assign.</param>
        /// <response code="204">Role assigned successfully.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">User or role not found.</response>
        /// <response code="409">Role is already assigned to the user.</response>
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddRole(string id, [FromBody] RoleRequestDTO request)
        {
            var result = await _userService.AddRoleAsync(id, request.Role);
            return result.Match<IActionResult>(ucces => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="request">Role to remove.</param>
        /// <response code="204">Role removed successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">User or role not found.</response>
        /// <response code="409">User does'nt have the role assigned.</response>
        [HttpDelete("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveRole(string id, [FromBody] RoleRequestDTO request)
        {
            var result = await _userService.RemoveRoleAsync(id, request.Role);
            return result.Match<IActionResult>(succes => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="request">Current and new password.</param>
        /// <response code="204">Password changed successfully.</response>
        /// <response code="400">Current password is incorrect or new password does not meet requirements.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is inactive.</response>
        /// <response code="404">User not found.</response>
        [HttpPatch("{id}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequestDTO request)
        {
            var result = await _userService.ChangePasswordAsync(id, request);
            return result.Match<IActionResult>(succes => NoContent(), errors => this.ToActionResult(errors));
        }
    }
}