using HondaERP.Application.Interfaces;
using HondaERP.Application.Services;
using HondaERP.Infrastructure.Identity;
using HondaERP.Infrastructure.Persistence;
using HondaERP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection missing hai.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Application services and repositories
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");

if (!jwtSection.Exists())
{
    throw new InvalidOperationException(
        "Jwt section missing hai. Startup API project ke " +
        "Manage User Secrets mein settings add karein.");
}

var jwt = jwtSection.Get<JwtOptions>();

if (jwt is null)
{
    throw new InvalidOperationException(
        "Jwt settings ko JwtOptions mein load nahi kar saka.");
}

if (string.IsNullOrWhiteSpace(jwt.Key))
{
    throw new InvalidOperationException(
        "Jwt:Key missing ya empty hai.");
}

if (jwt.Key.StartsWith(
        "REPLACE_",
        StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Jwt:Key abhi REPLACE_ placeholder hai. " +
        "Apni generated key paste karein.");
}

if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key kam az kam 32 bytes honi chahiye.");
}

if (string.IsNullOrWhiteSpace(jwt.Issuer))
{
    throw new InvalidOperationException(
        "Jwt:Issuer missing hai.");
}

if (string.IsNullOrWhiteSpace(jwt.Audience))
{
    throw new InvalidOperationException(
        "Jwt:Audience missing hai.");
}

if (jwt.ExpiryMinutes <= 0)
{
    throw new InvalidOperationException(
        "Jwt:ExpiryMinutes zero se zyada hona chahiye.");
}

builder.Services.Configure<JwtOptions>(jwtSection);

// JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt.Key)),

                ValidAlgorithms = new[]
                {
                    SecurityAlgorithms.HmacSha256
                },

                NameClaimType = "name",
                RoleClaimType = "role",

                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Development setup
if (app.Environment.IsDevelopment())
{
    var ownerEmail = builder.Configuration["SeedOwner:Email"];
    var ownerPassword = builder.Configuration["SeedOwner:Password"];

    if (string.IsNullOrWhiteSpace(ownerEmail))
    {
        throw new InvalidOperationException(
            "SeedOwner:Email missing hai.");
    }

    if (string.IsNullOrWhiteSpace(ownerPassword))
    {
        throw new InvalidOperationException(
            "SeedOwner:Password missing hai.");
    }

    if (ownerPassword.StartsWith(
            "REPLACE_",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "SeedOwner:Password abhi placeholder hai. " +
            "Apna strong password set karein.");
    }

    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        await IdentitySeeder.SeedRolesAsync(roleManager);

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await IdentitySeeder.SeedOwnerAsync(
            userManager,
            ownerEmail,
            ownerPassword);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Request pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();