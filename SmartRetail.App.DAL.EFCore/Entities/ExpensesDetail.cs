namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class ExpensesDetail
    {
        public int id { get; set; }
        public decimal sum { get; set; }
        
        public virtual Expense Expense { get; set; }
        public virtual ExpensesType ExpensesType { get; set; }
    }
}