using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces.Persistence.Users;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Identity;



namespace Restaurant.Infrastructure.Persistence.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRepository(SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            this._context = context;
        }

        public async Task<bool> EmailExistsAsync(string email, string? excludeId = null)
        {
            return await _userManager.Users
                .AnyAsync(x => x.Email == email && (excludeId == null || x.Id != excludeId));
        }

        public async Task<bool> UsernameExistsAsync(string userName, string? excludeId = null)
        {
            return await _userManager.Users
                .AnyAsync(x => x.UserName == userName && (excludeId == null || x.Id != excludeId));
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser is null)
                return null;

            return await MapToUserAsync(appUser);
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync()
        {
            var appUsers = await _userManager.Users.AsNoTracking().ToListAsync();

            var userRoles = await _context.UserRoles.AsNoTracking().ToListAsync();

            var roles = await _context.Roles.AsNoTracking().ToListAsync();

            var users = appUsers.Select(appUser =>
            {
                var userRoleIds = userRoles
                    .Where(ur => ur.UserId == appUser.Id)
                    .Select(ur => ur.RoleId);

                var userRoleNames = roles
                    .Where(r => userRoleIds.Contains(r.Id))
                    .Select(r => r.Name!)
                    .ToList();

                return new User
                {
                    Id = appUser.Id,
                    UserName = appUser.UserName ?? string.Empty,
                    Email = appUser.Email ?? string.Empty,
                    IsActive = appUser.IsActive,
                    Roles = userRoleNames
                };
            }).ToList();

            return users.AsReadOnly();
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser is null)
                return null;

            return await MapToUserAsync(appUser);
        }

        public async Task<(User? user, bool isValid)> ValidateUserAsync(string email, string password)
        {
            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser is null)
                return (null, false);

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);

            if (!result.Succeeded)
                return (null, false);

            var user = await MapToUserAsync(appUser);

            return (user, true);
        }

        public async Task<User?> CreateUserAsync(string email, string password, string username)
        {
            var appUser = new ApplicationUser
            {
                Email = email,
                UserName = username
            };

            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
                return null;

            return await MapToUserAsync(appUser);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);

            appUser!.Email = user.Email;
            appUser.UserName = user.UserName;
            appUser.IsActive = user.IsActive;
            appUser.NormalizedUserName = user.UserName.ToUpper();

            var result = await _userManager.UpdateAsync(appUser);


            return result.Succeeded;
        }

        public async Task<bool> SoftDeleteAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            appUser!.IsActive = false;

            var result = await _userManager.UpdateAsync(appUser);

            return result.Succeeded;
        }

        public async Task<bool> AddToRoleAsync(string userId, string role)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.AddToRoleAsync(appUser!, role);

            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.RemoveFromRoleAsync(appUser!, role);

            return result.Succeeded;
        }

        private async Task<User> MapToUserAsync(ApplicationUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);

            return new User
            {
                Id = appUser.Id,
                UserName = appUser.UserName ?? string.Empty,
                Email = appUser.Email ?? string.Empty,
                IsActive = appUser.IsActive,
                Roles = roles.ToList()
            };
        }



        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            if (appUser is null)
                return false;

            var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);

            return result.Succeeded;
        }

    }
}
