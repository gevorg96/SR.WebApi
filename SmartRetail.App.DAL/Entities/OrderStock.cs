using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class OrderStock : IEntity
    {
        public int id { get; set; }
        public int? order_id { get; set; }
        public int? prod_id { get; set; }
        public decimal? curr_stocks { get; set; }
        public int shop_id { get; set; }

        [Write(false)]
        [Computed]
        public virtual OrderDetail OrderDetail {get; set;}
    }
}
