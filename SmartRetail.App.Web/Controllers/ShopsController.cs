using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/shops")]
    [Authorize]
    [ApiController]
    public class ShopsController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly IShopSerivce shopSerivce;
        public ShopsController(IUserRepository userRepository, IShopSerivce _shopService)
        {
            userRepo = userRepository;
            shopSerivce = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<ShopViewModel>> GetStocks()
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var stocks = shopSerivce.GetStocks(user).OrderBy(p => p.id);
            return stocks;
        }
    }
}