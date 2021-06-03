using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SR.Application.Persistence;

namespace SR.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<ISrContext, SrContext>(
                builder => builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("SR.Service")), 
                1000);

            return services;
        }
    }
}