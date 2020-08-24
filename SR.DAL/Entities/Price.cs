using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Price")]
    public class Price : Entity
    {
        public long[] ShopId { get; set; }
        public decimal? Value { get; set; }

        public Product Product { get; set; }
    }
}
