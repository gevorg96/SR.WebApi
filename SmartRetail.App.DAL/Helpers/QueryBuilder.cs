using System.Runtime.InteropServices;
using System.Text;

namespace SmartRetail.App.DAL.Helpers
{
    public class QueryBuilder
    {
        private StringBuilder queryBuilder;

        public QueryBuilder()
        {
            queryBuilder = new StringBuilder();
        }

        public QueryBuilder Select(string value)
        {
            queryBuilder.Append("select ")
                .Append(value)
                .Append(" ");
            return this;
        }

        public QueryBuilder From(string value)
        {
            queryBuilder.Append("from ")
                .Append(value)
                .Append(" ");
            return this;
        }

        public QueryBuilder Where(string value)
        {
            queryBuilder.Append("where ")
                .Append(value)
                .Append(" ");
            return this;
        }

        public QueryBuilder Op(Ops op, string value)
        {
            switch (op)
            {
                case Ops.Equals:
                    queryBuilder.Append(" = ");
                    break;
                case Ops.Greater:
                    queryBuilder.Append(" > ");
                    break;
                case Ops.GreaterThen:
                    queryBuilder.Append(" >= ");
                    break;
                case Ops.Less:
                    queryBuilder.Append(" < ");
                    break;
                case Ops.LessThen:
                    queryBuilder.Append(" <= ");
                    break;
            }

            queryBuilder.Append(value);
            return this;
        }

        public override string ToString()
        {
            return queryBuilder.ToString();
        }

        public void Clear()
        {
            queryBuilder = new StringBuilder();
        }
    }
}