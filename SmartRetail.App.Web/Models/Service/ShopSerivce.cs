using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Service
{
    public class ShopSerivce: IShopSerivce
    {
        private readonly IShopRepository shopRepo;
        
        public ShopSerivce(IShopRepository shopRepo)
        {
            this.shopRepo = shopRepo;
        }
        
        public IEnumerable<ShopViewModel> GetStocks(UserProfile user)
        {
            var shops = user.shop_id != null ? new List<Shop> { shopRepo.GetById(user.shop_id.Value) } : shopRepo.GetShopsByBusiness(user.business_id.Value);

            var list = new List<ShopViewModel>();
            foreach (var shop in shops)
                list.Add(new ShopViewModel
                {
                    id = shop.id,
                    name = shop.name
                });
            if (user.shop_id == null)
            {
                list.Add(new ShopViewModel
                {
                    id = 0,
                    name = "Все склады"
                });
            }
            
            return list;
        }
    }
}