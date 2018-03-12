using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using System.Collections.Generic;
using System.Linq;

namespace CW03.Repositories
{
    class BookmarkEntityRepo : IBookmarkEntityRepo
    {
        private readonly CW03Context db;

        public BookmarkEntityRepo(CW03Context context)
        {
            db = context;
        }
        public void RemoveBookmarkEntitiesByParentPath(string ParentPath)
        {
            db.BookmarkEntity.RemoveRange(db.BookmarkEntity.Where(b => b.ParentPath.StartsWith(ParentPath)));
        }

        public int NumberOfDirectItems(string ParentPath)
        {
            return db.BookmarkEntity.Where(b => b.ParentPath.Equals(ParentPath)).Count();
        }
        public int NumberOfIndirectItems(string ParentPath)
        {
            return db.BookmarkEntity.Where(b => b.ParentPath.StartsWith(ParentPath)).Count();
        }
        public List<BookmarkEntity> GetRootBookmarks()
        {
            return db.BookmarkEntity.Where(b => b.ParentPath.Equals(null) || b.ParentPath.Equals("") || b.ParentPath.Equals("Root")).ToList();
        }
        public List<BookmarkEntity> GetListOfSubBookMarks(string ParentPath)
        {
            return db.BookmarkEntity.Where(b => b.ParentPath.Equals(ParentPath)).ToList();
        }

        public BookmarkEntity GetBookMarkByNameAndParentPath(string Name, string Path)
        {
            return db.BookmarkEntity.Where(b => b.Name.Equals(Name) && (b.ParentPath.Equals(Path) || b.ParentPath.Equals("Root") || b.ParentPath.Equals(null))).FirstOrDefault();
        }

        public BookmarkEntity.Type GetTypeById(int Id)
        {
            return db.BookmarkEntity.Where(b => b.Id.Equals(Id)).FirstOrDefault().BookmarkType;            
        }

        public BookmarkEntity GetById(int Id)
        {
            return db.BookmarkEntity.Where(b => b.Id.Equals(Id)).FirstOrDefault();
        }

        public bool NameExistById(string name, string parent, int id)
        {
            return db.BookmarkEntity.Where(b => b.Name.Equals(name) && b.ParentPath.Equals(parent) && !b.Id.Equals(id)).Count() > 0;
        }
    }
}
