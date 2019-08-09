using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartRetail.App.Web.Models.ViewModel.Expenses
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExpensesRequestViewModel
    {
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public DateTime reportDate { get; set; }
        [JsonProperty]
        public decimal totalSum { get; set; }
        [JsonProperty]
        public int? shopId { get; set; }
        [JsonProperty]
        public IEnumerable<ExpensesRequestDetailViewModel> expenses { get; set; }

    }
}