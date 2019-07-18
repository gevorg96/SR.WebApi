using System.Collections.Generic;
using System.Linq;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Validation
{
    public class ShopsChecker
    {
        private readonly IShopRepository _shopRepository;
        private readonly IBusinessRepository _businessRepository;

        public ShopsChecker(IShopRepository shopRepository, IBusinessRepository businessRepository)
        {
            _shopRepository = shopRepository;
            _businessRepository = businessRepository;
        }

        public AvailabilityModel CheckAvailability(UserProfile user, int? shopId)
        {
            var avl = new AvailabilityModel {isAdmin = !user.shop_id.HasValue};


            if (!shopId.HasValue || shopId.Value == 0)
            {
                avl.hasShop = false;
                if (avl.isAdmin)
                {
                    avl.isCorrectShop = true;
                    return avl;
                }

                avl.isCorrectShop = false;
                return avl;
            }

            avl.hasShop = true;
            if (!avl.isAdmin)
            {
                avl.isCorrectShop = user.shop_id.Value == shopId.Value;
                return avl;
            }

            var shops = _shopRepository.GetShopsByBusiness(user.business_id.Value).ToList();
            avl.isCorrectShop = shops.Exists(p => p.id == shopId.Value);
            return avl;
        }

        public Shop GetCorrectShop(UserProfile user, ProductDetailViewModel product)
        {
            List<Shop> shops = null;
            Business business = null;
            if (user.shop_id.HasValue && user.shop_id != 0)
            {
                // get shop
                shops = new List<Shop> { _shopRepository.GetById(user.shop_id.Value) };
            }
            else
            {
                business = _businessRepository.GetWithFilter("id", user.business_id.ToString()).FirstOrDefault();

                if (business != null)
                {
                    //get shops
                    shops = _shopRepository.GetShopsByBusiness(business.id).ToList();
                }
            }

            //get correct shop 
            var shop = shops?.FirstOrDefault(p => p.id == product.ShopId);
            if (shop != null)
            {
                shop.Business = _businessRepository.GetById(shop.business_id.Value);
            }
            return shop;
        }
    }
}