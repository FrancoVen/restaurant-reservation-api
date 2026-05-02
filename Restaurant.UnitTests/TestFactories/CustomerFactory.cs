using Restaurant.Application.Dtos.Customers;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Domain.Entities;


namespace Restaurant.UnitTests.TestFactories
{
    public static class CustomerFactory
    {
        private const int DefaultId = 1;
        private const string DefaultName = "John";
        private const string DefaultLastName = "Doe";
        private const string DefaultEmail = "john.doe@example.com";

        public static Customer CreateCustomer(int? id = null, string? email = null) => new()
        {
            Id = id ?? DefaultId,
            Name = DefaultName,
            LastName = DefaultLastName,
            Email = email ?? DefaultEmail,
            PhoneNumber = "1234567890"
        };

        public static CustomerCreationDTO CreateCustomerCreationDto(string? email = null) => new()
        {
            Name = DefaultName,
            LastName = DefaultLastName,
            Email = email ?? DefaultEmail,
            PhoneNumber = "1234567890"
        };

        public static CustomerDTO CreateDto(int? id = null) => new()
        {
            Id = id ?? DefaultId,
            FullName = $"{DefaultName} {DefaultLastName}"
        };


        public static CustomerReservationDTO CreateCustomerReservationDto(int? id = null) => new()
        {
            Id = id ?? DefaultId,
            FullName = $"{DefaultName} {DefaultLastName}",
            Reservations = new List<ReservationDTO>()
            {
                new ReservationDTO()
                {
                    Id=1,
                    CustomerName = DefaultName,
                    ReservationTime = DateTime.Now,
                    Status = ReservationStatus.Cancelled.ToString(),
                    TableNumber = 1,
                }
            }
        };


        public static List<Customer> CreateCustomerList() => new()
        {
           new Customer(){ Name="Leandro", LastName = "Negri", Email="leandronegri@gmail.com", Id=1, PhoneNumber="1133225522", Reservations = null!},
           new Customer(){ Name="Juan", LastName = "Garcia", Email="juangarcia@hotmail.com", Id=2, PhoneNumber="1133321180", Reservations = null!}
        };

        public static List<CustomerDTO> CreateCustomerDTOList() => new()
        {
           new CustomerDTO(){FullName="Leandro Negri", Id= 1},
           new CustomerDTO(){FullName="Juan Garcia", Id = 2}
        };

    }
}