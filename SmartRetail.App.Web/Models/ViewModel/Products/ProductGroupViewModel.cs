using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel
{
    public class ProductGroupViewModel
    {
        public List<FolderViewModel> Folders { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
