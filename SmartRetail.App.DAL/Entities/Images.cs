using System;

namespace SmartRetail.App.DAL.Entities
{
    public class Images:IEntity
    {
        public Guid ROWGUID { get; set; }

        public int prod_id { get; set; }

        public string img_type { get; set; }

        public string img_name { get; set; }

        public string img_url { get; set; }
        
        public string img_url_temp { get; set; }
        
        public string img_path { get; set; }
    }
}
