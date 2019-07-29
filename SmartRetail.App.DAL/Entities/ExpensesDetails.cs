namespace SmartRetail.App.DAL.Entities
{
    public class ExpensesDetails: IEntity
    {
        public int id { get; set; }
        public int expenses_id { get; set; }
        public int expenses_type_id { get; set; }
        public decimal sum { get; set; }

        public virtual ExpensesType ExpensesType { get; set; }
    }
}
