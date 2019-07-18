namespace SmartRetail.App.DAL.BLL.HelperClasses
{
    public class PicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string ShortName { get; set; }
        public string DirectoryPath { get; set; }
        
        public PicInfo(int id, string name, string path)
        {
            Id = id;
            Name = name;
            FullPath = path;
        }
        
        public PicInfo(int id, string name, string path, string shortName)
        {
            Id = id;
            Name = name;
            FullPath = path;
            ShortName = shortName;
        }
        
        public PicInfo(int id, string name, string path, string shortName, string directoryPath)
        {
            Id = id;
            Name = name;
            FullPath = path;
            ShortName = shortName;
            DirectoryPath = directoryPath;
        }

        public PicInfo()
        {
            
        }
        
    }
}