using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class FilteredProductViewModel
    {
        public IEnumerable<ProductViewModel> Products;
        public PageViewModel PageViewModel { get; set; }
        public string SelectedProductName { get; set; }
        public string SelectedProductColor { get; set; }
        public string SelectedProductSize { get; set; }
    }
}