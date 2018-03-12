using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW03.Controllers
{
    [Produces("application/json")]
    public class GoogleApiController : Controller
    {
        private readonly CW03Context _context;
        private readonly IBookmarkEntityRepo bookmarkEntityRepo;
        private readonly IFolderRepo folderRepo;
        private readonly IItemLinkRepo itemLinkRepo;
        private readonly IItemLocationRepo itemLocationRepo;
        private readonly IItemTextfileRepo itemTextfileRepo;

        public GoogleApiController(CW03Context context,
            IBookmarkEntityRepo bookmarkEntityRepo,
            IFolderRepo folderRepo,
            IItemLinkRepo itemLinkRepo,
            IItemLocationRepo itemLocationRepo,
            IItemTextfileRepo itemTextfileRepo)
        {
            _context = context;
            this.bookmarkEntityRepo = bookmarkEntityRepo;
            this.folderRepo = folderRepo;
            this.itemLinkRepo = itemLinkRepo;
            this.itemLocationRepo = itemLocationRepo;
            this.itemTextfileRepo = itemTextfileRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OrgChart()
        {
            return View();
        }
        public IActionResult TreeMap()
        {
            return View();
        }

        public List<OrgChartHelpModel> GetOrgChart()
        {
            return PrepareList();
        }

        public List<TreeMapHelperModel> GetTreeMap()
        {
            return PrepareTreeMap();
        }

        private List<TreeMapHelperModel> PrepareTreeMap()
        {
            List<TreeMapHelperModel> toReturn = new List<TreeMapHelperModel>
            {
                new TreeMapHelperModel
                {
                    name = "Root",
                    id = -1,
                    parentId = -1,
                    parentName = null,
                    items = 1
                }
            };
            foreach (BookmarkEntity bookmarkEntity in _context.BookmarkEntity.ToList())
            {
                Tuple<string, string> tuple = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
               ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
                var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
                if (parent == null)
                {
                    return null;
                }
                if (!bookmarkEntity.Equals(parent))
                {
                    if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly)
                    {
                        var items = bookmarkEntityRepo.NumberOfIndirectItems((bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name).Replace("Root|", ""));
                        toReturn.Add(new TreeMapHelperModel
                        {
                            name = bookmarkEntity.Name,
                            id = bookmarkEntity.Id,
                            parentId = -1,
                            parentName = "Root",
                            items = (items> 0) ? items :1
                        });
                    }
                    else
                    {
                        var items = bookmarkEntityRepo.NumberOfIndirectItems((bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name).Replace("Root|", ""));
                        toReturn.Add(new TreeMapHelperModel
                        {
                            name = bookmarkEntity.Name,
                            id = bookmarkEntity.Id,
                            parentId = parent.Id,
                            parentName = parent.Name,
                            items = (items > 0) ? items : 1
                        });
                    }
                }
                else
                {
                    var items = bookmarkEntityRepo.NumberOfIndirectItems((bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name).Replace("Root|", ""));
                    toReturn.Add(new TreeMapHelperModel
                    {
                        name = bookmarkEntity.Name,
                        id = bookmarkEntity.Id,
                        parentId = -1,
                        parentName = "Root",
                        items = (items > 0) ? items : 1
                    });
                }
            }
            return toReturn;
        }

        private List<OrgChartHelpModel> PrepareList()
        {
            List<OrgChartHelpModel> toReturn = new List<OrgChartHelpModel>();

            foreach (BookmarkEntity bookmarkEntity in _context.Folder.ToList())
            {
                Tuple<string, string> tuple = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
               ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
                var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
                if (parent == null)
                {
                    return null;
                }
                if (!bookmarkEntity.Equals(parent))
                {
                    if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly)
                    {
                        toReturn.Add(new OrgChartHelpModel
                        {
                            id = bookmarkEntity.Id,
                            name = bookmarkEntity.Name,
                            parentId = -1,
                            parentName = ""
                        });
                    }
                    else
                    {
                        toReturn.Add(new OrgChartHelpModel
                        {
                            id = bookmarkEntity.Id,
                            name = bookmarkEntity.Name,
                            parentId = parent.Id,
                            parentName = parent.Name
                        });
                    }
                }
                else
                {
                    toReturn.Add(new OrgChartHelpModel
                    {
                        id = bookmarkEntity.Id,
                        name = bookmarkEntity.Name,
                        parentId = 0,
                        parentName = ""
                    });
                }

            }
            return toReturn;
        }

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
    }
}