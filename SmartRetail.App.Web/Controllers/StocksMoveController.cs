using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.StockMove;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("moving")]
    [Authorize]
    [ApiController]
    public class StocksMoveController : Controller
    {
        private readonly IStockMoveService service;
        private readonly IUserRepository userRepo;

        public StocksMoveController(IStockMoveService _service, IUserRepository _userRepo)
        {
            service = _service;
            userRepo = _userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int shopFrom, string name)
        {
            if (shopFrom == 0)
                return BadRequest("Необходимо выбрать склад.");
            var user = await userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var prods = await service.GetProducts(user, shopFrom, name);
                return Ok(prods);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveStocks([FromBody]StockMoveRequestViewModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (model.shopFrom == 0 || model.shopTo == 0)
            {
                return BadRequest("Необходимо указать склады.");
            }
            try
            {
                await service.MoveStocks(user, model);
                return Ok("Товары перемещены.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}