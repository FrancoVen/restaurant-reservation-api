namespace Restaurant.Application.Dtos.Customers
{
    public class CustomerCreationDTO
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }

        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }

    }
}
