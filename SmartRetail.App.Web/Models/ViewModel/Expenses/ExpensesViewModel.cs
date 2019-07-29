using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Expenses
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExpensesViewModel
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
        public IEnumerable<ExpensesValueViewModel> expenses { get; set; }
    }
}