using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Restaurant.Infrastructure.Identity.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
        {
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];
            var username = configuration["AdminSeed:Username"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                logger.LogWarning("AdminSeed configuration is missing. Skipping admin seeding.");
                return;
            }

            if (await userManager.FindByEmailAsync(email) is not null)
            {
                logger.LogInformation("Admin user already exists. Skipping.");
                return;
            }

            var admin = new ApplicationUser
            {
                Email = email,
                UserName = username,
                FirstName = "Admin",
                LastName = "System"
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(admin, "Admin");

            logger.LogInformation("Admin user created successfully.");
        }
    }
}