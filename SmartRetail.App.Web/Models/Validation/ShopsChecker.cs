using System.Linq;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.Web.Models.Validation
{
    public class ShopsChecker
    {
        private readonly IShopRepository _shopRepository;

        public ShopsChecker(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
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
    }
}