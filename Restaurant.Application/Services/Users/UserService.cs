using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Dtos.Users;
using Restaurant.Application.Interfaces.Persistence.Roles;
using Restaurant.Application.Interfaces.Persistence.Users;
using Restaurant.Application.Services.Users.Interfaces;
using Restaurant.Domain.Entities;


namespace Restaurant.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IRoleRepository roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            this._logger = logger;
            this._roleRepository = roleRepository;
            this._mapper = mapper;
        }


        public async Task<ErrorOr<UserDTO>> GetByIdAsync(string id)
        {
            var searchedUser = await _userRepository.GetByIdAsync(id);

            if (searchedUser is null)
            {
                return Error.NotFound("User.NotFound", $"The user shearched with ID '{id}' was not found.");
            }

            var userDto = _mapper.Map<UserDTO>(searchedUser);

            return userDto;

        }

        public async Task<ErrorOr<IReadOnlyCollection<UserDTO>>> GetAllAsync()
        {
            var users = await _userRepository.GetUsersAsync();

            var usersDto = _mapper.Map<IReadOnlyCollection<UserDTO>>(users);

            return usersDto.ToErrorOr();
        }


        public async Task<ErrorOr<Success>> AddRoleAsync(string userId, string role)
        {
            var validation = await ValidateUserAndRoleAsync(userId, role);

            if (validation.IsError)
            {
                return validation.Errors;
            }

            var user = validation.Value;

            if (user.Roles.Contains(role))
            {
                return Error.Conflict(
                    "User.RoleAlreadyAssigned",
                    "The role is already assigned to the user."
                );
            }

            var result = await _userRepository.AddToRoleAsync(userId, role);

            if (!result)
            {
                _logger.LogError("Failed to add role '{Role}' to user {UserId}.", role, userId);
                return Error.Unexpected("User.RoleAssignmentFailed", "Failed to assign the role.");
            }

            return Result.Success;
        }

        public async Task<ErrorOr<Deleted>> SoftDeleteAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return Error.NotFound("User.NotFound", $"The user with ID {id} was not found");
            }

            if (!user.IsActive)
            {
                return Result.Deleted;
            }

            var result = await _userRepository.SoftDeleteAsync(id);

            if (!result)
            {
                _logger.LogError("Failed to soft delete user {UserId}.", id);
                return Error.Unexpected("User.DeletionFailed", "Failed to delete the user.");
            }

            return Result.Deleted;
        }


        public async Task<ErrorOr<Success>> RemoveRoleAsync(string userId, string role)
        {
            var validation = await ValidateUserAndRoleAsync(userId, role);

            if (validation.IsError)
            {
                return validation.Errors;
            }

            var user = validation.Value;

            if (!user.Roles.Contains(role))
            {
                return Error.Conflict("User.Conflict", $"The user does not have the role {role} assigned");
            }

            var result = await _userRepository.RemoveRoleAsync(userId, role);

            if (!result)
            {
                _logger.LogError("Failed to remove role '{Role}' from user {UserId}.", role, userId);
                return Error.Unexpected("User.RoleRemovalFailed", "Failed to remove the role.");
            }

            return Result.Success;
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(string id, UpdateUserRequestDTO userRequestDTO)
        {
            var searchedUser = await _userRepository.GetByIdAsync(id);

            if (searchedUser is null)
            {
                return Error.NotFound("User.NotFound", $"User with ID '{id}' was not found");
            }

            var userNameVerifier = await _userRepository.UsernameExistsAsync(userRequestDTO.UserName, id);

            if (userNameVerifier)
            {
                return Error.Conflict("User.Conflict", "Username is already in use");
            }

            var emailVerifier = await _userRepository.EmailExistsAsync(userRequestDTO.Email, id);

            if (emailVerifier)
            {
                return Error.Conflict("User.Conflict", "Email is already in use");
            }


            _mapper.Map(userRequestDTO, searchedUser);

            var result = await _userRepository.UpdateUserAsync(searchedUser);

            return Result.Updated;

        }

        private async Task<ErrorOr<User>> ValidateUserAndRoleAsync(string id, string role)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(role))
            {
                return Error.Validation("User.Validation", "Id and role can't be null or empty");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                _logger.LogWarning("User with ID {id} was not found", id);
                return Error.NotFound("User.NotFound", $"The user with ID {id} was not found");
            }

            var roleExists = await _roleRepository.RoleExistsAsync(role);

            if (!roleExists)
            {
                _logger.LogWarning("The role searched {role} was not found", role);
                return Error.NotFound("Role.NotFound", $"The role {role} was not found");
            }

            return user;

        }

        public async Task<ErrorOr<Success>> ChangePasswordAsync(string userId, ChangePasswordRequestDTO request)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Error.Validation("User.Validation", "Id can't be null or empty");
            }

            var searchedUser = await _userRepository.GetByIdAsync(userId);

            if (searchedUser is null) return Error.NotFound("User.NotFound", $"User with ID '{userId}' was not found.");

            if (!searchedUser.IsActive) return Error.Forbidden("User.Forbidden", "Cannot change password of an inactive user.");

            var result = await _userRepository.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!result)
            {
                _logger.LogWarning("ChangePassword failed for user {UserId}.", userId);
                return Error.Validation("User.InvalidPassword", "Password could not be changed.");
            }


            _logger.LogInformation("Password changed successfully for user {UserId}.", userId);
            return Result.Success;
        }
    }
}
