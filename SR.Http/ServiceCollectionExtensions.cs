using Microsoft.Extensions.DependencyInjection;
using SR.Application;

namespace SR.Http
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseHttp(this IServiceCollection services)
        {
            services.UseApplication();
            return services;
        }
    }
}