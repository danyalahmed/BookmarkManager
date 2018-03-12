using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CW03.Controllers
{
    [Route("/")]
    [Route("/Home/")]
    public class HomeController : Controller
    {
        private readonly CW03Context db;
        private readonly IFolderRepo folderRepo;
        private readonly IBookmarkEntityRepo bookmarkEntityRepo;

        public HomeController(CW03Context context,
            IFolderRepo folderRepo,
            IBookmarkEntityRepo bookmarkEntityRepo)
        {
            db = context;
            this.folderRepo = folderRepo;
            this.bookmarkEntityRepo = bookmarkEntityRepo;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            ViewBag.TreeStringList = GetBookMarksTreeStringList(CreaeBookMarkTree(), 1);
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region HelperMethods
        private dynamic CreaeBookMarkTree()
        {
            List<BookMarkTreeHelperModel> tree = new List<BookMarkTreeHelperModel>();

            var rootBookMarks = bookmarkEntityRepo.GetRootBookmarks();
            foreach (BookmarkEntity root in rootBookMarks)
                tree.Add(GenerateSubTree(root));

            return tree;
        }
        private dynamic GenerateSubTree(BookmarkEntity temp)
        {
            var tree = new BookMarkTreeHelperModel
            {
                folder = temp.Name,
                ParentPath = temp.ParentPath,
                type = temp.BookmarkType
            };
            List<BookmarkEntity> bookMark = null;

            if (temp.ParentPath != null
                && (temp.ParentPath != "" && temp.ParentPath != "Root"))
            {
                bookMark = bookmarkEntityRepo.GetListOfSubBookMarks(temp.ParentPath + "|" + temp.Name);
            }
            else
            {
                bookMark = bookmarkEntityRepo.GetListOfSubBookMarks(temp.Name);
            }

            if (bookMark != null
                && bookMark.Count > 0)
            {
                tree.subBookmarks = new List<BookMarkTreeHelperModel>();
                foreach (BookmarkEntity subBookMark in bookMark)
                {
                    tree.subBookmarks.Add(GenerateSubTree(subBookMark));
                }
            }
            return tree;
        }
        private dynamic GetBookMarksTreeStringList(dynamic dynamic, int Level)
        {
            if (dynamic != null)
            {
                StringBuilder @string = new StringBuilder();
                foreach (BookMarkTreeHelperModel root in dynamic)
                {
                    string id = "item-";
                    string parent = string.Empty;

                    if (root.ParentPath == null || root.ParentPath.Equals(""))
                    {
                        id += root.folder;
                        parent = root.folder;
                    }
                    else
                    {
                        id += root.ParentPath.Replace("|", "-") + "-" + root.folder;
                        parent = root.ParentPath + "|" + root.folder;
                    }

                    bool subFolder = (root.subBookmarks != null
                    && root.subBookmarks.Count > 0);
                    @string.Append("<a ").Append(FormatAppend("data-toggle", "collapse"))
                        .Append(FormatAppend("style", "padding-left:" + Level * 25 + "px;"))
                        .Append(FormatAppend("data-parent", "#accordian"))
                        .Append(FormatAppend("class", "list-group-item"))
                        .Append(FormatAppend("href", "#" + id)).Append(">");
                    if (subFolder)
                        //glyphicon arrow
                        @string.Append("<span ").Append(FormatAppend("class", "glyphicon glyphicon-chevron-right"))
                            .Append("></span>");
                    //glyphicon Type

                    switch (root.type)
                    {
                        case BookmarkEntity.Type.FOLDER:
                            {
                                @string.Append("<span ").Append(FormatAppend("class", "glyphicon glyphicon-folder-open")).Append("></span>");
                                break;
                            }
                        case BookmarkEntity.Type.LINK:
                            {
                                @string.Append("<span ").Append(FormatAppend("class", "glyphicon glyphicon-link")).Append("></span>");
                                break;
                            }
                        case BookmarkEntity.Type.LOCATION:
                            {
                                @string.Append("<span ").Append(FormatAppend("class", "glyphicon glyphicon-map-marker")).Append("></span>");
                                break;
                            }
                        case BookmarkEntity.Type.TEXTFILE:
                            {
                                @string.Append("<span ").Append(FormatAppend("class", "glyphicon glyphicon-file")).Append("></span>");
                                break;
                            }
                    }
                    //FOLDERNAME
                    @string.Append(root.folder);
                    var indirectFiles = bookmarkEntityRepo.NumberOfIndirectItems(parent.Replace("Root|",""));
                    if (indirectFiles > 0)
                        //Number of indirect Folder badge
                        @string.Append("<span ").Append(FormatAppend("class", "badge badge-pill badge-secondary"))
                            .Append(">")
                            .Append("Indirect Items: ")
                            .Append(indirectFiles);
                    var directFiles = bookmarkEntityRepo.NumberOfDirectItems(parent.Replace("Root|", ""));
                    if (directFiles > 0)
                        @string.Append("</span>")
                        //Number of direct Folder badge
                        .Append("<span ").Append(FormatAppend("class", "badge badge-pill badge-primary"))
                            .Append(">")
                            .Append("Direct Items: ")
                            .Append(directFiles)
                        .Append("</span>");
                    @string.Append("</a>");
                    if (subFolder)
                    {
                        //if there exist subfolders, get there string recursively
                        @string.Append("<div ").Append(FormatAppend("id", id))
                            .Append(FormatAppend("class", "collapse list-group")).Append(">");
                        @string.Append(GetBookMarksTreeStringList(root.subBookmarks, Level + 1));
                        @string.Append("</div>");
                    }

                }
                return @string;
            }
            else return null;
        }
        //return key="value"
        private string FormatAppend(string key, string value)
        {
            return key + "=" + (char)34 + value + (char)34;
        }
        #endregion
    }
}
