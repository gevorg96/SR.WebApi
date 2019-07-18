namespace SmartRetail.App.DAL.Templates
{
    public static class QueryTemplates
    {
        public static class Select
        {
            public const string BasicSelect = "SELECT fields FROM table";

            public const string ConditionalSelect = "SELECT fields FROM table WHERE condition";

            public const string InConditionalSelect = "SELECT fields FROM table WHERE field IN(query)";
        }
    }
}