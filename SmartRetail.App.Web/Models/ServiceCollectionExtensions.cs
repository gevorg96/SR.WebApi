using Microsoft.Extensions.DependencyInjection;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Service;
using SmartRetail.App.Web.Models.Validation;

namespace SmartRetail.App.Web.Models
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ShopsChecker>();
            services.AddScoped<IShopSerivce, ShopSerivce>();
            services.AddScoped<IInformationService, SummaryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISalesService, SalesSerivce>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IExpensesService, ExpensesService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICancellationService, CancellationService>();
            services.AddScoped<IExpensesTypeService, ExpensesTypeService>();
            services.AddScoped<IStockMoveService, StockMoveService>();

            return services;
        }
    }
}


