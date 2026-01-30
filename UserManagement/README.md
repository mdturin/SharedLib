# User Management Library for .NET

[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://github.com/YourUsername/UserManagement.Library/packages)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A comprehensive, production-ready user management library for .NET applications with JWT authentication, role-based authorization, and complete user management features using ASP.NET Core Identity.

---

## 🌟 Why Use This Library?

- **⚡ Quick Integration** - Add user management to any .NET project in minutes
- **🔐 Enterprise Security** - JWT tokens, refresh tokens, password hashing
- **🎯 Production Ready** - Battle-tested patterns and best practices
- **📦 Fully Featured** - Registration, login, roles, password recovery, and more
- **🔧 Highly Customizable** - Easily extend and customize to your needs
- **📚 Well Documented** - Comprehensive guides and examples included

## Features

- ✅ User Registration
- ✅ User Login with JWT Authentication
- ✅ Refresh Token Support
- ✅ Password Management (Change, Forgot, Reset)
- ✅ User Profile Management
- ✅ Role-Based Authorization
- ✅ User Activation/Deactivation
- ✅ Complete CRUD Operations for Users
- ✅ Extensible and Customizable
- ✅ Easy Integration with Dependency Injection

## Installation

### 1. Build the Library

```bash
dotnet build UserManagement.csproj
```

### 2. Add Reference to Your Project

Add the library as a project reference or create a NuGet package:

```bash
dotnet add reference path/to/UserManagement.csproj
```

Or pack and install as NuGet:

```bash
dotnet pack
dotnet add package UserManagement
```

## Quick Start

### 1. Configure appsettings.json

Add the following configuration to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "YourAppName",
    "Audience": "YourAppName",
    "TokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 2. Register Services in Program.cs

```csharp
using UserManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add User Management Library
builder.Services.AddUserManagement(
    builder.Configuration,
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    options =>
    {
        // Customize Identity options if needed
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    }
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed default roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Seed roles
    await services.SeedRolesAsync("Admin", "User", "Manager");
    
    // Seed admin user (optional)
    await services.SeedAdminUserAsync(
        "admin@example.com",
        "Admin@123",
        "Admin",
        "User"
    );
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 3. Run Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate --context ApplicationDbContext

# Update database
dotnet ef database update --context ApplicationDbContext
```

### 4. Use the Controllers

Copy the sample controllers from `SampleControllers/` folder to your project's Controllers folder, or create your own using the services.

## Usage Examples

### Authentication Service

```csharp
public class MyController : ControllerBase
{
    private readonly IAuthService _authService;

    public MyController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
```

### User Service

```csharp
public class MyUserController : ControllerBase
{
    private readonly IUserService _userService;

    public MyUserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
```

## API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login user | No |
| POST | `/api/auth/refresh-token` | Refresh access token | No |
| POST | `/api/auth/logout` | Logout user | Yes |
| POST | `/api/auth/change-password` | Change password | Yes |
| POST | `/api/auth/forgot-password` | Request password reset | No |
| POST | `/api/auth/reset-password` | Reset password | No |

### User Management Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users/me` | Get current user | Yes |
| GET | `/api/users/{id}` | Get user by ID | Yes |
| GET | `/api/users/email/{email}` | Get user by email | Yes |
| GET | `/api/users` | Get all users | Yes (Admin) |
| PUT | `/api/users/me` | Update current user | Yes |
| PUT | `/api/users/{id}` | Update user | Yes (Admin) |
| DELETE | `/api/users/{id}` | Delete user | Yes (Admin) |
| POST | `/api/users/{id}/deactivate` | Deactivate user | Yes (Admin) |
| POST | `/api/users/{id}/activate` | Activate user | Yes (Admin) |
| POST | `/api/users/{id}/roles/{role}` | Add role to user | Yes (Admin) |
| DELETE | `/api/users/{id}/roles/{role}` | Remove role from user | Yes (Admin) |
| GET | `/api/users/{id}/roles` | Get user roles | Yes |

## Request/Response Examples

### Register User

**Request:**
```json
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password@123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "bGFzdExvZ2luQXQ...",
  "tokenExpiration": "2024-01-31T15:30:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "createdAt": "2024-01-31T14:30:00Z",
    "lastLoginAt": null,
    "isActive": true,
    "roles": ["User"]
  }
}
```

### Login User

**Request:**
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "Password@123"
}
```

**Response:** (Same as Register response)

## Customization

### Custom User Model

Extend `ApplicationUser` to add custom properties:

```csharp
public class CustomApplicationUser : ApplicationUser
{
    public string? Department { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
```

### Custom Identity Options

Configure Identity options during registration:

```csharp
builder.Services.AddUserManagement(
    builder.Configuration,
    connectionString,
    options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.User.RequireUniqueEmail = true;
    }
);
```

### Custom JWT Settings

Modify JWT settings in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "YourCompany",
    "Audience": "YourApp",
    "TokenExpirationMinutes": 120,
    "RefreshTokenExpirationDays": 30
  }
}
```

## Security Considerations

1. **Secret Key**: Use a strong, randomly generated secret key of at least 32 characters
2. **HTTPS**: Always use HTTPS in production
3. **Password Policy**: Configure strong password requirements
4. **Token Expiration**: Set appropriate token expiration times
5. **Refresh Tokens**: Store refresh tokens securely and implement rotation
6. **Email Verification**: Implement email verification for production use
7. **Rate Limiting**: Add rate limiting to prevent brute force attacks

## Database Support

The library uses Entity Framework Core and supports:
- SQL Server (default)
- PostgreSQL
- MySQL
- SQLite

To use a different database, change the provider in your project:

```csharp
// For PostgreSQL
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// For MySQL
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
```

## Testing

Example unit test setup:

```csharp
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IAuthService _authService;

    [Fact]
    public async Task Register_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password@123",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Token);
    }
}
```

## Troubleshooting

### Common Issues

1. **Migration Errors**: Ensure the connection string is correct
2. **JWT Validation Failed**: Check SecretKey, Issuer, and Audience match in configuration
3. **Token Expired**: Implement proper refresh token logic
4. **Role Not Found**: Ensure roles are seeded before use

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## License

This library is provided as-is for educational and commercial use.

## Support

For issues and questions, please create an issue in the repository.

---

**Version**: 1.0.0
**Last Updated**: January 2026