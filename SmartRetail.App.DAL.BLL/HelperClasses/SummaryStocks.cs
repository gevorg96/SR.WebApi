using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRetail.App.DAL.BLL.HelperClasses
{
    public class SummaryStocks
    {
        public decimal cost { get; set; }
        public decimal goodsCount { get; set; }
        public Dictionary<string, decimal> goods { get; set; }

        public SummaryStocks()
        {
            goods = new Dictionary<string, decimal>();
        }
    }
}
