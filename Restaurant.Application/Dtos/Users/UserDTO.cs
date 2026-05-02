
namespace Restaurant.Application.Dtos.Users
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = new List<string>();
    }
}
