namespace Restaurant.Application.Interfaces.Persistence.Roles
{
    public interface IRoleRepository
    {
        public Task<bool> RoleExistsAsync(string role);
    }
}
