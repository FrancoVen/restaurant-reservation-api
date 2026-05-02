namespace Restaurant.Application.Dtos.Users
{
    public class UpdateUserRequestDTO
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}
