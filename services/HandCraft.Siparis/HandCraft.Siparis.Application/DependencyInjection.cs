using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HandCraft.Siparis.Application
{
    public static class DependencyInjection
    {
        // Application katmaninin MediatR handler'larini kaydeder.
        public static IServiceCollection AddSiparisApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
