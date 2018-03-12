using Newtonsoft.Json;
using System.Collections.Generic;

namespace CW03.Models
{
    public class BookMarkTreeHelperModel
    {
        public BookMarkTreeHelperModel()
        {
        }

        public BookMarkTreeHelperModel(string folder, List<BookMarkTreeHelperModel> subfolders)
        {
            this.folder = folder;
            subBookmarks = subfolders;
        }

        public BookMarkTreeHelperModel(string folder, List<BookMarkTreeHelperModel> subfolders, string parentPath)
        {
            this.folder = folder;
            subBookmarks = subfolders;
            ParentPath = parentPath;
        }

        public string folder { get; set; }

        public List<BookMarkTreeHelperModel> subBookmarks { get; set; }

        public string ParentPath { get; set; }

        public BookmarkEntity.Type type { get; set; }
    }
}
