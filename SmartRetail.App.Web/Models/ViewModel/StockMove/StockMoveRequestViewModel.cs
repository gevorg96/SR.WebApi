using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.StockMove
{
    public class StockMoveRequestViewModel
    {
        public IEnumerable<StockMovePair> products { get; set; }
        public int shopFrom { get; set; }
        public int shopTo { get; set; }
    }
}
