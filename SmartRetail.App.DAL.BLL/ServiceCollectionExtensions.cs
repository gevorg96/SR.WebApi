using Microsoft.Extensions.DependencyInjection;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.BLL.Utils;

namespace SmartRetail.App.DAL.BLL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddScoped<ISalesDataService, SalesDataService>();
            services.AddScoped<IExpensesDataService, ExpensesDataService>();
            services.AddScoped<IStocksDataService, StocksDataService>();
            services.AddScoped<IStrategy, FifoStrategy>();
            services.AddScoped<ICategoryTreeFiller, CategoryTreeFiller>();
            services.AddScoped<IFoldersDataService, FoldersDataService>();
            services.AddScoped<IOrderDataService, OrderDataService>();

            return services;
        }
    }
}
