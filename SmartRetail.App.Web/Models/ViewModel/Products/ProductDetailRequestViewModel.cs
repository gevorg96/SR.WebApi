using System.Collections.Generic;
using SmartRetail.App.Web.Models.ViewModel.Units;

namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ProductDetailRequestViewModel
    {
        public IEnumerable<UnitViewModel> Units { get; set; }
        public IEnumerable<ShopViewModel> Shops { get; set; }

        public ProductDetailRequestViewModel()
        {
            Units = new List<UnitViewModel>();
            Shops = new List<ShopViewModel>();
        }
    }
}