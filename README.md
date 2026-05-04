🍽️ Restaurant Reservation API

RESTful API built with ASP.NET Core and Clean Architecture (5 layers) for managing the complete reservation lifecycle of a restaurant.

🌐 **Live Demo:** [Swagger UI](https://restaurantapi20260430005542-d8aybah0asg6ekgq.brazilsouth-01.azurewebsites.net/swagger/index.html)

### Tech Stack
- ASP.NET Core 9 — Web API
- Clean Architecture — 5 layers (Domain, Application, Infrastructure, API, UnitTests)
- Entity Framework Core — ORM
- ASP.NET Core Identity — User management
- JWT Authentication — Role-based authorization (Admin, Receptionist)
- ErrorOr — Standardized error handling
- FluentValidation — Input validation
- AutoMapper — Object mapping
- xUnit + NSubstitute + FluentAssertions — Unit testing
- Swagger / OpenAPI — API documentation
- SQL Server — Database
- Azure App Service — Cloud deployment

### Features
- JWT Authentication with ASP.NET Core Identity
- Role-based authorization (Admin, Receptionist)
- Full reservation lifecycle management
- Customer and table management
- Reservation conflict detection
- Soft delete for users
- Standardized error responses with ProblemDetails
- Automatic role and admin seeding on startup
- Unit tests following AAA pattern

### Architecture
- **Domain** → Entities and business rules
- **Application** → Use cases, services, interfaces, DTOs
- **Infrastructure** → Identity, EF Core, repositories
- **API** → Controllers, middleware, configuration
- **UnitTests** → Unit tests with xUnit, NSubstitute and FluentAssertions

### Getting Started

1. **Clone the repository**

git clone https://github.com/FrancoVen/restaurant-reservation-api.git

2. **Create appsettings.Development.json **
Create this file inside the Restaurant.API folder:

{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection-string"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters",
    "Issuer": "restaurant-api",
    "Audience": "restaurant-client",
    "ExpiryMinutes": 60
  },
  "AdminSeed": {
    "Email": "admin@restaurant.com",
    "Password": "Admin123!",
    "Username": "admin"
  }
}

Only `DefaultConnection` needs to match your local SQL Server instance. The rest can be copied as-is.

### 3. Run the following command to apply migrations:
update-database -project Restaurant.Infrastructure -startupproject Restaurant.API

