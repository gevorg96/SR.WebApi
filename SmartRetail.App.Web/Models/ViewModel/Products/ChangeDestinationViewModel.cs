using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ChangeDestinationViewModel
    {
        public int shopId { get; set; }
        public IEnumerable<ChangeDestinationDetailViewModel> productsCount { get; set; }
    }
}
