using Restaurant.Domain.Entities;


namespace Restaurant.Application.Interfaces.Persistence.Users
{
    public interface IUserRepository
    {
        public Task<IReadOnlyCollection<User>> GetUsersAsync();
        public Task<(User? user, bool isValid)> ValidateUserAsync(string email, string password);
        public Task<User?> FindByEmailAsync(string email);

        public Task<User?> GetByIdAsync(string id);
        public Task<User?> CreateUserAsync(string email, string password, string username);

        public Task<bool> UpdateUserAsync(User updatedUser);

        public Task<bool> SoftDeleteAsync(string userId);

        public Task<bool> RemoveRoleAsync(string userId, string role);
        public Task<bool> AddToRoleAsync(string userId, string role);

        public Task<bool> EmailExistsAsync(string email, string? excludeId = null);

        public Task<bool> UsernameExistsAsync(string userName, string? excludeId = null);

        public Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
