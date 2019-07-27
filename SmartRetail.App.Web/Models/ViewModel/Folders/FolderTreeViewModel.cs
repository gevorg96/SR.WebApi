using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.ViewModel.Folders
{
    public class FolderTreeViewModel
    {
        public string folder { get; set; }
        public string fullpath { get; set; }
        public List<FolderTreeViewModel> children { get; set; }
    }
}
