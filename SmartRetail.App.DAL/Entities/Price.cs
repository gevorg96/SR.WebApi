using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class Price : IEntity
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        public int? shop_id { get; set; }
        public decimal? price { get; set; }

        [Write(false)]
        [Computed]
        public virtual Product Product { get; set; }
    }
}
