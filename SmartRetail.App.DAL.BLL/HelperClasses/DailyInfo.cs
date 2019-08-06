using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRetail.App.DAL.BLL.HelperClasses
{
    public class DailyInfo
    {
        public decimal totalRevenue { get; set; }
        public decimal totalProfit { get; set; }
        public int billsCount { get; set; }
        public decimal averageBill {get;set;}
    }
}
