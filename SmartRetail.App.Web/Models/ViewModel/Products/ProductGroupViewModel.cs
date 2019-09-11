using System.Collections.Generic;
using SmartRetail.App.Web.Models.ViewModel.Folders;

namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ProductGroupViewModel
    {
        public List<FolderViewModel> Folders { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
