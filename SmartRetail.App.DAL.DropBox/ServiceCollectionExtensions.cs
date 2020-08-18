using Microsoft.Extensions.DependencyInjection;

namespace SmartRetail.App.DAL.DropBox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPictureSupport(this IServiceCollection services, string apiKey, string apiSecret)
        {
            services.AddScoped<IPictureWareHouse, DropBoxBase>(o => new DropBoxBase(apiKey, apiSecret));
            return services;
        }
    }
}
