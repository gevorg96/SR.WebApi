namespace SmartRetail.App.Web.Models.ViewModel.Folders
{
    public class FolderRequestSearchVeiwModel
    {
        public string searchCriteria { get; set; }
        public string path { get; set; }
        public bool needProducts { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
    }
}