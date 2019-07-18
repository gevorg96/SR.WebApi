using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRetail.App.DAL.Templates;

namespace SmartRetail.App.DAL.Helpers
{
    public static class QueryHelper
    {
        public static string GetBasicQuery(List<string> fields, string table)
        {
            var query = QueryTemplates.Select.BasicSelect;

            if (string.IsNullOrEmpty(table))
            {
                return null;
            }
            query = query.Replace("table", table);
            
            if (fields == null || fields.Count == 0)
            {
                query = query.Replace("fields", "*");
            }
            else if (fields.Count > 1)
            {
                query = query.Replace("fields", string.Join(", ", fields));
            }
            else if (fields.Count == 1)
            {
                query = query.Replace("fields", fields.First());
            }

            return query;
        }

        public static string GetIds(IEnumerable<int> ids)
        {
            var sb = new StringBuilder();

            foreach (var id in ids)
            {
                sb.Append(id + ",");
            }

            return sb.ToString().Substring(0, sb.Length - 1);
        }

        }
}