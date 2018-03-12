using CW03.Models;
using System;
using System.Collections.Generic;

namespace CW03.IRepositories
{
    public interface IFolderRepo
    {
        bool Exist(Folder Folder);
        bool ExistFolderByNameAndParentPath(string Name, string ParentPath);
        Folder GetById(int id);
        Folder GetFolderByName(string Name);
        Folder GetFolderByNameAndParentPath(string Name, string Path);
        HashSet<Folder> Folders { get; }
        HashSet<String> GetFolderNamesOnlyByPathParent(string Path);
        List<Folder> GetAll();
        List<Folder> GetRootFolders();
        List<Folder> GetListOfSubFolders(string ParentPath);
        void InsertAsync(Folder Obj);
        void Update(Folder Obj);
        void DeleteAsync(int Id);
        int NumberOfDirectSubFolders(string ParentPath);
        int NumberOfIndirectSubFolders(string ParentPath);
    }
}
