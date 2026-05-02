using Microsoft.AspNetCore.Identity;
using Restaurant.Application.Interfaces.Persistence.Roles;

namespace Restaurant.Infrastructure.Persistence.Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }
        public async Task<bool> RoleExistsAsync(string role)
        {
            ArgumentException.ThrowIfNullOrEmpty(role);

            return await _roleManager.RoleExistsAsync(role);
        }
    }
}
