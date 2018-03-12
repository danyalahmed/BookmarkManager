using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using System.Collections.Generic;
using System.Linq;

namespace CW03.Repositories
{
    class FolderRepo : IFolderRepo
    {
        private readonly CW03Context db;

        public FolderRepo(CW03Context context)
        {
            db = context;
        }
        HashSet<Folder> IFolderRepo.Folders => db.Folder.ToHashSet();
        bool IFolderRepo.Exist(Folder Folder)
        {
            return db.Folder.ToHashSet().Contains(Folder);
        }
        public HashSet<string> GetFolderNamesOnlyByPathParent(string Path)
        {
            return db.Folder.Where(f => f.ParentPath.Equals(Path)).Select(f => f.Name).ToHashSet();
        }
        async void IFolderRepo.InsertAsync(Folder Obj)
        {
            await db.Folder.AddAsync(Obj);
        }        
        void IFolderRepo.Update(Folder Obj)
        {
            db.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        async void IFolderRepo.DeleteAsync(int Id)
        {
            var f = await db.Folder.FindAsync(Id);
            db.Folder.Remove(f);
        }
        //Folders PARENTPATH must be NULL
        public Folder GetFolderByName(string Name)
        {
            return db.Folder.Where(f => f.Name.Equals(Name) && (f.ParentPath.Equals(null) || f.ParentPath.Equals("") || f.ParentPath.Equals("Root"))).FirstOrDefault();
        }
        public Folder GetFolderByNameAndParentPath(string Name, string Path)
        {
            return db.Folder.Where(f => f.Name.Equals(Name) && (f.ParentPath.Equals(Path) || f.ParentPath.Equals(null) || f.ParentPath.Equals("Root"))).FirstOrDefault();            
        }
        public bool ExistFolderByNameAndParentPath(string Name, string ParentPath)
        {
            return db.Folder.Where(f => f.Name.Equals(Name) && (f.ParentPath.Equals(ParentPath) || f.ParentPath.Equals(null))).Count() > 0;            
        }
        public List<Folder> GetListOfSubFolders(string ParentPath)
        {
            return db.Folder.Where(f => f.ParentPath.Equals(ParentPath)).ToList();            
        }
        public int NumberOfDirectSubFolders(string ParentPath)
        {
            return db.Folder.Where(f => f.ParentPath.Equals(ParentPath)).Count();            
        }
        public int NumberOfIndirectSubFolders(string ParentPath)
        {
            return db.Folder.Where(f => f.ParentPath.StartsWith(ParentPath)).Count();
        }
        public List<Folder> GetRootFolders()
        {
            return db.Folder.Where(f => f.ParentPath.Equals(null) || f.ParentPath.Equals("") || f.ParentPath.Equals("Root")).ToList();
        }

        public List<Folder> GetAll()
        {
            return db.Folder.ToList();
        }

        public Folder GetById(int id)
        {
            return db.Folder.Where(f => f.Id.Equals(id)).FirstOrDefault();
        }
    }
}
