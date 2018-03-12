using CW03.Models;
using System.Collections.Generic;

namespace CW03.IRepositories
{
    public interface IBookmarkEntityRepo
    {
        void RemoveBookmarkEntitiesByParentPath(string ParentPath);
        int NumberOfDirectItems(string ParentPath);
        int NumberOfIndirectItems(string ParentPath);
        List<BookmarkEntity> GetRootBookmarks();
        List<BookmarkEntity> GetListOfSubBookMarks(string ParentPath);
        BookmarkEntity GetById(int Id);
        BookmarkEntity GetBookMarkByNameAndParentPath(string Name, string Path);
        BookmarkEntity.Type GetTypeById(int Id);
        bool NameExistById(string name, string parent, int id);
    }
}
