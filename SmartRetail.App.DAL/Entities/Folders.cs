using System;

namespace SmartRetail.App.DAL.Entities
{
    public class Folders: IEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public int? parent_id { get; set; }
        public string folder { get; set; }

        public override bool Equals(object obj)
        {
            var folder = obj as Folders;
            if (folder != null)
            {
                return id.Equals(folder.id);
            }
            else
                throw new Exception("Используется объект не того типа.");
        }
    }
}
