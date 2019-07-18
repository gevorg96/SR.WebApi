using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartRetail.App.DAL.Helpers
{
    public static class NullChecker
    {
        public static string isNotNull(int? i)
        {
            if (i == null)
            {
                return "NULL";
            }
            return i.Value.ToString();
        }

        public static string isNotNull(string o)
        {
            if (string.IsNullOrEmpty(o))
            {
                return "NULL";
            }
            return string.Format("N'{0}'", o);
        }

        public static string isNotNull(decimal? d)
        {
            if (!d.HasValue)
            {
                return "NULL";
            }

            return d.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
