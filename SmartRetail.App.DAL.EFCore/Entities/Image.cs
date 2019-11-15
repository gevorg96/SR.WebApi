using System;

namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Image
    {
        public Guid ROWGUID { get; set; }

        public string img_type { get; set; }

        public string img_name { get; set; }

        public string img_url { get; set; }
        
        public string img_url_temp { get; set; }
        
        public string img_path { get; set; }
        
        public virtual Product Product { get; set; }
    }
}