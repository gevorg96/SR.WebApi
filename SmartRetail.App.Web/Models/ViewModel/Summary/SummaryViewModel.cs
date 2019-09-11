using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Summary
{
    public class SummaryViewModel
    {
        public List<string> imgs { get; set; }
        public decimal revenue { get; set; }
        public decimal profit { get; set; }
        public decimal salesCount { get; set; }
        public decimal averageBill { get; set; }

        public SummaryViewModel()
        {
            imgs = new List<string>();
        }
    }
}
