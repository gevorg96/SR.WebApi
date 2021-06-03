using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SR.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceCollectionExtensions));
            
            return services;
        }
    }
}