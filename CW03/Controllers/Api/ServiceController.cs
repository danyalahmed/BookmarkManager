using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using CW03.Models.ApiFolderHelperModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CW03.Controllers.Api
{
    [Produces("application/json")]
    [Route("service/")]
    public class ServiceController : Controller
    {
        private readonly CW03Context db;
        private readonly IFolderRepo folderRepo;
        private readonly IBookmarkEntityRepo bookmarkEntityRepo;

        public ServiceController(CW03Context context,
            IFolderRepo folderRepo,
            IBookmarkEntityRepo bookmarkEntityRepo)
        {
            db = context;
            this.folderRepo = folderRepo;
            this.bookmarkEntityRepo = bookmarkEntityRepo;
        }

        [Route("create")]
        public bool Create(string Folder, string Parent)
        {
            if (Folder != null && !Folder.Equals(string.Empty))
            {
                if (!folderRepo.GetFolderNamesOnlyByPathParent(Parent).Contains(Folder))
                {
                    Folder newFolder = new Folder
                    {
                        BookmarkType = BookmarkEntity.Type.FOLDER,
                        Name = Folder,
                        ReadOnly = false
                    };
                    if (Parent != null && !Parent.Equals(string.Empty))
                    {
                        newFolder.ParentPath = Parent;
                    }
                    else
                    {
                        newFolder.ParentPath = "Root";
                    }
                    folderRepo.InsertAsync(newFolder);
                    db.SaveChangesAsync();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        [Route("delete")]
        public bool Delete(string Folder)
        {
            if (Folder != null && !Folder.Equals(string.Empty))
            {
                Folder tempFolder = null;
                if (Folder.Contains("|"))
                {
                    //item1 is the FOLDER and item2 is PARENTPATH
                    Tuple<string, string> tupleFolderNames = GetFolderAndParentFolder(Folder);
                    if (tupleFolderNames == null)
                    {
                        return false;
                    }
                    tempFolder = folderRepo.GetFolderByNameAndParentPath(tupleFolderNames.Item1, tupleFolderNames.Item2);
                }
                else
                {
                    tempFolder = folderRepo.GetFolderByName(Folder);
                }
                if (tempFolder != null)
                {
                    //removes all items in the FOLDER
                    bookmarkEntityRepo.RemoveBookmarkEntitiesByParentPath(Folder);
                    //removes Folder itself
                    folderRepo.DeleteAsync(tempFolder.Id);
                    db.SaveChangesAsync();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        [Route("structure")]
        public ActionResult Structure(string Folder)
        {
            if (Folder != null && !Folder.Equals(string.Empty))
            {
                Folder tempFolder = null;
                if (Folder.Contains("|"))
                {
                    //item1 is the FOLDER and item2 is PARENTPATH
                    Tuple<string, string> tupleFolderNames = GetFolderAndParentFolder(Folder);
                    if (tupleFolderNames == null)
                    {
                        return Json(new FolderJsonTreeHelpModel("", null));
                    }
                    tempFolder = folderRepo.GetFolderByNameAndParentPath(tupleFolderNames.Item1, tupleFolderNames.Item2);
                }
                else
                {
                    tempFolder = folderRepo.GetFolderByName(Folder);
                }
                if (tempFolder != null)
                {
                    return Json(CreateFolderTree(tempFolder));
                }
                else return Json(new FolderJsonTreeHelpModel("", null));
            }
            else return Json(new FolderJsonTreeHelpModel("", null));
        }

        [Route("count")]
        public ActionResult Count(string Folder)
        {
            if (Folder != null && !Folder.Equals(string.Empty))
            {
                Folder tempFolder = null;
                if (Folder.Contains("|"))
                {
                    //item1 is the FOLDER and item2 is PARENTPATH
                    Tuple<string, string> tupleFolderNames = GetFolderAndParentFolder(Folder);
                    if (tupleFolderNames == null)
                    {
                        return Json(new FolderJsonTreeHelpModel("", null));
                    }
                    tempFolder = folderRepo.GetFolderByNameAndParentPath(tupleFolderNames.Item1, tupleFolderNames.Item2);
                }
                else
                {
                    tempFolder = folderRepo.GetFolderByName(Folder);
                }
                if (tempFolder != null)
                {
                    return Json(new FolderCountHelperModel
                    {
                        direct = folderRepo.NumberOfDirectSubFolders(Folder),
                        indirect = folderRepo.NumberOfIndirectSubFolders(Folder)
                    });
                }
                else return Json(new FolderCountHelperModel(0, 0));
            }
            else return Json(new FolderCountHelperModel(0, 0));
        }

        [Route("createStructure")]
        public bool CreateStructure(string Tree, string Root)
        {
            if (Root != null
                && !Root.Equals(string.Empty))
            {
                //check if root of this tree is actualy root, or a subfolder
                Tuple<string, string> RootFolder = GetFolderAndParentFolder(Root);
                if (RootFolder == null) return false;

                //if root does not exist, return false
                if (!folderRepo.ExistFolderByNameAndParentPath(RootFolder.Item1, ((RootFolder.Item2.Equals("") ? "Root": RootFolder.Item2)))) return false;

                List<Tuple<string, string>> FoldersToCreate = GetFoldersList(Tree, Root);
                if (FoldersToCreate == null) return false;

                foreach (Tuple<string, string> FolderToCreate in FoldersToCreate)
                {
                    Folder folder = new Folder
                    {
                        BookmarkType = BookmarkEntity.Type.FOLDER,
                        Name = FolderToCreate.Item1,                        
                        ReadOnly = false
                    };
                    if (FolderToCreate.Item2  != null && !FolderToCreate.Item2.Equals(string.Empty))
                    {
                        folder.ParentPath = FolderToCreate.Item2;
                    }
                    else
                    {
                        folder.ParentPath = "Root";
                    }
                    if (!folderRepo.ExistFolderByNameAndParentPath(folder.Name, folder.ParentPath))
                        folderRepo.InsertAsync(folder);
                    else
                        return false;
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        #region HELPERMETHODS
        //Given a construct tree and root, creates a tuple with folder and its ParentPath
        private List<Tuple<string, string>> GetFoldersList(string tree, string root)
        {
            List<Tuple<string, string>> toReturn = new List<Tuple<string, string>>();

            Dictionary<int, string> dictionaryParentPath = new Dictionary<int, string>();

            int Level = -1;
            string FolderName = "";
            string LastParentFolder = "";
            for (int i = 0; i < tree.Length; i++)
            {
                if (tree[i].Equals('|')) continue;
                if (tree[i].Equals('['))
                {
                    Level++;
                    LastParentFolder = FolderName;
                    continue;
                }
                else if (tree[i].Equals(']'))
                {
                    Level--;
                    //get the previous parent on this level
                    if (Level < 0) continue;
                    int toSubString = dictionaryParentPath[Level].LastIndexOf('|');
                    if (toSubString > 0)
                        LastParentFolder = dictionaryParentPath[Level].Substring(toSubString + 1);
                    else LastParentFolder = FolderName;
                    continue;
                }
                if (Level < 0) continue;
                else
                {
                    int NameEndsAt = tree.IndexOf('[', i) - i;

                    if (NameEndsAt < 0
                        || NameEndsAt > tree.IndexOf(']', i) - i)
                    {
                        NameEndsAt = tree.IndexOf(']', i) - i;
                    }
                    if (NameEndsAt < 0
                        || (NameEndsAt > tree.IndexOf("|", i) - i
                            && tree.IndexOf("|", i) - i > 0))
                    {
                        NameEndsAt = tree.IndexOf('|', i) - i;
                    }

                    FolderName = tree.Substring(i, NameEndsAt);
                    if (Level == 0)
                    {
                        dictionaryParentPath[Level] = root;
                        toReturn.Add(new Tuple<string, string>(FolderName, root));
                    }
                    else
                    {
                        dictionaryParentPath[Level] = dictionaryParentPath[Level - 1] + "|" + LastParentFolder;
                        toReturn.Add(new Tuple<string, string>(FolderName, dictionaryParentPath[Level]));
                    }
                    i += FolderName.Length - 1;
                }
            }
            return toReturn;
        }
        //Given path returns Fodler name and its Parent Folder path
        private Tuple<string, string> GetFolderAndParentFolder(string path)
        {
            //splits on every |, so the last index is to have FOLDERNAME
            var f = path.Split("|");
            //splits on |FOLDERNAME                
            string FolderName = f[f.Length - 1];
            var p = path.Split("|" + FolderName);

            string ParentPath = "";
            if (p.Length > 1)
                ParentPath = p[0];
            //Suppose: Document|Folder|Folder
            if (p.Length > 2)
            {
                string temp = "";
                for (int i = 0; i < f.Length - 1; i++)
                {
                    temp += f[i];
                    if (i != f.Length - 2)
                    {
                        temp += "|";
                    }
                }
                ParentPath = temp;
            }
            return new Tuple<string, string>(FolderName, ParentPath);
        }
        //Given a FOLDER creates a tree of type FOLDERJSONTREEHELPERMODEIL.
        private FolderJsonTreeHelpModel CreateFolderTree(Folder tempFolder)
        {
            var tree = new FolderJsonTreeHelpModel
            {
                folder = tempFolder.Name
            };
            List<Folder> subFolders = null;

            if (tempFolder.ParentPath != null
                && !tempFolder.ParentPath.Equals(""))
            {
                subFolders = folderRepo.GetListOfSubFolders(tempFolder.ParentPath + "|" + tempFolder.Name);
            }
            else
            {
                subFolders = folderRepo.GetListOfSubFolders(tempFolder.Name);
            }

            if (subFolders != null
                && subFolders.Count > 0)
            {
                tree.subfolders = new List<FolderJsonTreeHelpModel>();
                foreach (Folder subFolder in subFolders)
                {
                    tree.subfolders.Add(CreateFolderTree(subFolder));
                }
            }
            return tree;
        }
        #endregion
    }
}