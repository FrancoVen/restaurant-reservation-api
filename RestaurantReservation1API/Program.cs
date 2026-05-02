using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Common.Settings;
using Restaurant.Application.Interfaces.Authentication;
using Restaurant.Application.Interfaces.Persistence.Customers;
using Restaurant.Application.Interfaces.Persistence.Reservations;
using Restaurant.Application.Interfaces.Persistence.Roles;
using Restaurant.Application.Interfaces.Persistence.Tables;
using Restaurant.Application.Interfaces.Persistence.Users;
using Restaurant.Application.Mappings;
using Restaurant.Application.Services.Auth;
using Restaurant.Application.Services.Auth.Interface;
using Restaurant.Application.Services.Customers;
using Restaurant.Application.Services.Customers.Interfaces;
using Restaurant.Application.Services.Reservations;
using Restaurant.Application.Services.Reservations.Interfaces;
using Restaurant.Application.Services.Reservations.Validators;
using Restaurant.Application.Services.Tables;
using Restaurant.Application.Services.Tables.Interfaces;
using Restaurant.Application.Services.Users;
using Restaurant.Application.Services.Users.Interfaces;
using Restaurant.Infrastructure.Authentication;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Identity;
using Restaurant.Infrastructure.Identity.Seeders;
using Restaurant.Infrastructure.Persistence.Repositories.Customers;
using Restaurant.Infrastructure.Persistence.Repositories.Reservations;
using Restaurant.Infrastructure.Persistence.Repositories.Roles;
using Restaurant.Infrastructure.Persistence.Repositories.Tables;
using Restaurant.Infrastructure.Persistence.Repositories.Users;
using Restaurant.Infrastructure.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfileMarker));


builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(MappingProfileMarker));
builder.Services.AddFluentValidationAutoValidation();



builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<ITableService, TableService>();


builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationValidator, ReservationValidator>();
builder.Services.AddScoped<IReservationService, ReservationService>();


builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IDateTimerProvider, DateTimerProvider>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddProblemDetails();


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("Restaurant.Infrastructure")));


builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<ApplicationDbContext>();




var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);


builder.Services.AddAuthorization();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
opt.TokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,

    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
}
);


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo()
    {
        Title = "Restaurant Reservation API",
        Version = "v1",
        Description = """
        RESTful API built with ASP.NET Core and Clean Architecture (5 layers) 
        for managing the complete reservation lifecycle of a restaurant.
        
        Key features:
        - JWT Authentication with ASP.NET Core Identity
        - Role-based authorization (Admin, Receptionist)
        - Reservation validation with conflict detection
        - Full customer and table management
        - Standardized error handling with ErrorOr pattern
        - Input validation with FluentValidation
        
        Testing:
        - Unit tests with xUnit, NSubstitute and FluentAssertions
        """,
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Name = "Franco Venialgo",
            Email = "francovenialgo_97@hotmail.com"
        }
    });


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer.\nEjemplo: \"Bearer {tu_token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

});



var app = builder.Build();
//Inicio de area de middlewares

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (dbcontext.Database.IsRelational())
    {
        dbcontext.Database.Migrate();
    }
}


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    await RoleSeeder.SeedAsync(roleManager, logger);
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var configuration = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    await RoleSeeder.SeedAsync(roleManager, logger);
    await AdminSeeder.SeedAsync(userManager, configuration, logger);
}


//Fin del area de middlewares
app.Run();
