using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.ViewModel.Folders
{
    public class FolderReplaceViewModel
    {
        public string pathToFolder { get; set; }
        public string newPath { get; set; }
        public bool Copy { get; set; }
    }
}
