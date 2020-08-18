using Microsoft.Extensions.DependencyInjection;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, string connString)
        {
            services.AddScoped<IShopRepository, ShopRepository>(o =>
                new ShopRepository(connString));
            services.AddScoped<IBusinessRepository, BusinessRepository>(o =>
                new BusinessRepository(connString));
            services.AddScoped<IUserRepository, UserRepository>(o =>
                new UserRepository(connString));
            services.AddScoped<IPriceRepository, PriceRepository>(o =>
                new PriceRepository(connString));
            services.AddScoped<IProductRepository, ProductRepository>(o =>
                new ProductRepository(connString));
            services.AddScoped<IOrdersRepository, OrdersRepository>(o =>
                new OrdersRepository(connString));
            services.AddScoped<IBillsRepository, BillsRepository>(o =>
                new BillsRepository(connString));
            services.AddScoped<IExpensesRepository, ExpensesRepository>(o =>
                new ExpensesRepository(connString));
            services.AddScoped<ICostRepository, CostRepository>(o =>
                new CostRepository(connString));
            services.AddScoped<IImageRepository, ImagesRepository>(o =>
                new ImagesRepository(connString));
            services.AddScoped<IStockRepository, StockRepository>(o =>
                new StockRepository(connString));
            services.AddScoped<IUnitRepository, UnitRepository>(o =>
                new UnitRepository(connString));
            services.AddScoped<IOrderStockRepository, OrderStockRepository>(o =>
                new OrderStockRepository(connString));
            services.AddScoped<IExpensesTypeRepository, ExpensesTypeRepository>(o =>
                new ExpensesTypeRepository(connString));
            services.AddScoped<IFoldersRepository, FoldersRepository>(o =>
                new FoldersRepository(connString));

            return services;
        } 
    }
}
