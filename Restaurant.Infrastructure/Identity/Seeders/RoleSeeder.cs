using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Restaurant.Infrastructure.Identity.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            string[] roles = ["Admin", "Receptionist"];

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (result.Succeeded)
                    logger.LogInformation("Role '{Role}' created successfully.", role);
                else
                    logger.LogWarning("Failed to create role '{Role}': {Errors}", role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
