using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace SmartRetail.App.DAL.Helpers
{
    public static class QueryHelper
    {
        public static string GetIds(IEnumerable<int> ids)
        {
            var sb = new StringBuilder();

            foreach (var id in ids)
            {
                sb.Append(id + ",");
            }

            return sb.ToString().Substring(0, sb.Length - 1);
        }

        public static string GetSqlString(PropertyInfo p, object o)
        {
            var pt = p.PropertyType.ToString();
            switch (pt)
            {
                case "System.String":
                    return "N'" + o + "'";
                case "System.Int32":
                case "System.Nullable`1[System.Int32]":
                    return o.ToString();
            }

            return null;
        }

    }
}