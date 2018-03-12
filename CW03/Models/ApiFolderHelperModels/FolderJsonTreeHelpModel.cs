using CW03.IRepositories;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CW03.Models
{
    public class FolderJsonTreeHelpModel
    {
        public FolderJsonTreeHelpModel()
        {
        }

        public FolderJsonTreeHelpModel(string folder,
            List<FolderJsonTreeHelpModel> subfolders)
        {
            this.folder = folder;
            this.subfolders = subfolders;
        }

        public string folder { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FolderJsonTreeHelpModel> subfolders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ParentPath { get; set; }
    }
}
