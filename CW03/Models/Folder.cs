using System.Collections.Generic;

namespace CW03.Models
{
    public class Folder:BookmarkEntity
    {        
        HashSet<BookmarkEntity> BookmarkedEntities = new HashSet<BookmarkEntity>();

        public Folder() :base()
        {
            BookmarkType = Type.FOLDER;
        }

        public Folder(string name) : base(name)
        {
            BookmarkType = Type.FOLDER;
        }

        public void AddFolderToList(BookmarkEntity Folder)
        {
            BookmarkedEntities.Add(Folder);
        }

        public void RemoveFolderFromList(BookmarkEntity Folder)
        {
            BookmarkedEntities.Remove(Folder);            
        }
    }
}