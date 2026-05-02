using ErrorOr;
using Restaurant.Application.Dtos.Users;

namespace Restaurant.Application.Services.Users.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<UserDTO>> GetByIdAsync(string id);
        Task<ErrorOr<IReadOnlyCollection<UserDTO>>> GetAllAsync();
        Task<ErrorOr<Deleted>> SoftDeleteAsync(string id);
        Task<ErrorOr<Updated>> UpdateAsync(string id, UpdateUserRequestDTO userRequestDTO);
        Task<ErrorOr<Success>> AddRoleAsync(string userId, string role);
        Task<ErrorOr<Success>> RemoveRoleAsync(string userId, string role);
        Task<ErrorOr<Success>> ChangePasswordAsync(string userId, ChangePasswordRequestDTO request);

    }
}
