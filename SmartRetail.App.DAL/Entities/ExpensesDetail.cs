using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class ExpensesDetail: IEntity
    {
        public int id { get; set; }
        public int expenses_id { get; set; }
        public int expenses_type_id { get; set; }
        public decimal sum { get; set; }

        [Write(false)]
        [Computed]
        public virtual ExpensesType ExpensesType { get; set; }
    }
}
