using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Service;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.BLL.Utils;
using ReflectionIT.Mvc.Paging;

namespace SmartRetail.App.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IHostingEnvironment _env { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var conn = Configuration.GetConnectionString("DefaultConnection");
            
            var apiKey = "o9340xsv2mzn7ws";
            var apiSecret = "xzky2fzfnmssik1";
            
            
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.ISSUER,
 
                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,
 
                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

            
            services.AddTransient<IShopRepository, ShopRepository>(o =>
                new ShopRepository(conn));
            services.AddTransient<IBusinessRepository, BusinessRepository>(o =>
                new BusinessRepository(conn));
            services.AddTransient<IUserRepository, UserRepository>(o =>
                new UserRepository(conn));
            services.AddTransient<IPriceRepository, PriceRepository>(o =>
                new PriceRepository(conn));
            services.AddTransient<IProductRepository, ProductRepository>(o =>
                new ProductRepository(conn));
            services.AddTransient<IOrdersRepository, OrdersRepository>(o =>
                new OrdersRepository(conn));
            services.AddTransient<IBillsRepository, BillsRepository>(o =>
                new BillsRepository(conn));
            services.AddTransient<IExpensesRepository, ExpensesRepository>(o =>
                new ExpensesRepository(conn));
            services.AddTransient<ICostRepository, CostRepository>(o =>
                new CostRepository(conn));
            services.AddTransient<IImageRepository, ImagesRepository>(o =>
                new ImagesRepository(conn));
            services.AddTransient<IStockRepository, StockRepository>(o =>
                new StockRepository(conn));
            services.AddTransient<IUnitRepository, UnitRepository>(o =>
                new UnitRepository(conn));
            services.AddTransient<IOrderStockRepository, OrderStockRepository>(o =>
                new OrderStockRepository(conn));
            services.AddTransient<IExpensesTypeRepository, ExpensesTypeRepository>(o =>
                new ExpensesTypeRepository(conn));
            services.AddTransient<IPictureWareHouse, DropBoxBase>(o =>
                new DropBoxBase(apiKey, apiSecret));
            services.AddTransient<IFoldersRepository, FoldersRepository>(o =>
                new FoldersRepository(conn));

            services.AddTransient<ISalesDataService, SalesDataService>();
            services.AddTransient<IExpensesDataService, ExpensesDataService>();
            services.AddTransient<IStocksDataService, StocksDataService>();
            services.AddTransient<ShopsChecker>();
            services.AddTransient<IStrategy, FifoStrategy>();
            services.AddTransient<ICategoryTreeFiller, CategoryTreeFiller>();
            services.AddTransient<IFoldersDataService,FoldersDataService>();
            services.AddTransient<IOrderDataService, OrderDataService>();
            
            services.AddTransient<IShopSerivce, ShopSerivce>();
            services.AddTransient<IInformationService, SummaryService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ISalesService, SalesSerivce>();
            services.AddTransient<IStockService, StockService>();
            services.AddTransient<IExpensesService, ExpensesService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUnitService, UnitService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICancellationService, CancellationService>();
            services.AddTransient<IExpensesTypeService, ExpensesTypeService>();
            services.AddTransient<IStockMoveService, StockMoveService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/AccountView/Login");
                });
 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddPaging(options => {
                options.ViewName = "Bootstrap4";
                options.HtmlIndicatorDown = " <span>&darr;</span>";
                options.HtmlIndicatorUp = " <span>&uarr;</span>";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePages("application/json", "Status code page, status code: {0}");
            app.UseCors("MyPolicy");

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=MainPage}/{action=Index}/{id?}");
            });

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});
        }
    }
}
