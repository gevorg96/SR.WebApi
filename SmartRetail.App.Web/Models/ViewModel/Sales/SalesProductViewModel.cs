using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.ViewModel
{
    public class SalesProductViewModel
    {
        public string imageUrl { get; set; }
        public string ProdName { get; set; }
        public string VendorCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Count { get; set; }
        public decimal? Summ { get; set; }
        
    }
}
