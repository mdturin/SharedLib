using Microsoft.Extensions.DependencyInjection;
using UserManagement.Web.Controllers;

namespace UserManagement.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(AuthController).Assembly)
            .AddApplicationPart(typeof(UsersController).Assembly);

        return services;
    }
}