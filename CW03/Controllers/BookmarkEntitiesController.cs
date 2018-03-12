using CW03.Data;
using CW03.IRepositories;
using CW03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW03.Controllers
{
    public class BookmarkEntitiesController : Controller
    {
        private readonly CW03Context _context;
        private readonly IBookmarkEntityRepo bookmarkEntityRepo;
        private readonly IFolderRepo folderRepo;
        private readonly IItemLinkRepo itemLinkRepo;
        private readonly IItemLocationRepo itemLocationRepo;
        private readonly IItemTextfileRepo itemTextfileRepo;

        public BookmarkEntitiesController(CW03Context context,
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

        // GET: BookmarkEntities
        public IActionResult Index()
        {
            ViewBag.Parent = "Root";
            return View(bookmarkEntityRepo.GetRootBookmarks());
        }

        public async Task<IActionResult> Viewcontent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folder = await _context.BookmarkEntity.FindAsync(id);
            if (folder == null) return View("Error", new ErrorViewModel("Folder Not Found!", "The folder with id " + id + " was not found!"));

            List<BookmarkEntity> Children = null;
            if (folder.ParentPath == null || folder.ParentPath == "Root")
            {
                ViewBag.Parent = folder.Name;
                Children = bookmarkEntityRepo.GetListOfSubBookMarks(folder.Name);
            }
            else
            {
                ViewBag.Parent = (folder.ParentPath + "|" + folder.Name);
                Children = bookmarkEntityRepo.GetListOfSubBookMarks(folder.ParentPath + "|" + folder.Name);
            }
            ViewBag.ParentId = folder.Id;
            return View("Index", Children);
        }


        public async Task<IActionResult> Lock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmarkEntity = await _context.BookmarkEntity.SingleOrDefaultAsync(m => m.Id == id);
            if (bookmarkEntity == null)
            {
                return NotFound();
            }

            Tuple<string, string> tuple = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
            var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (parent == null)
            {
                return NotFound();
            }

            if (!bookmarkEntity.Equals(parent))
                if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly) return View("Error", new ErrorViewModel());

            bookmarkEntity.ReadOnly = true;
            _context.Update(bookmarkEntity);

            LockChildren((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);


            await _context.SaveChangesAsync();
            return Redirect("/BookmarkEntities/ViewContent/" + parent.Id);
        }

        public async Task<IActionResult> Unlock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmarkEntity = await _context.BookmarkEntity.SingleOrDefaultAsync(m => m.Id == id);
            if (bookmarkEntity == null)
            {
                return NotFound();
            }

            Tuple<string, string> tuple = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
            var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (parent == null)
            {
                return NotFound();
            }
            if (!bookmarkEntity.Equals(parent))
                if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly)
                    return View("Error", new ErrorViewModel("Parent folder Locked!", "This folder cannot be unlocked, as the parent folder is locked."));

            bookmarkEntity.ReadOnly = false;
            UnlockChildren((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);

            _context.Update(bookmarkEntity);
            await _context.SaveChangesAsync();
            return Redirect("/BookmarkEntities/ViewContent/" + parent.Id);
        }

        public async Task<IActionResult> LockFromPath(string path)
        {
            if (path == null)
            {
                return NotFound();
            }

            Tuple<string, string> tuple = GetFolderAndParentFolder(path.Replace("item-", "").Replace("-", "|"));

            var bookmarkEntity = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (bookmarkEntity == null) return NotFound();

            Tuple<string, string> tuple2 = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
            var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (parent == null)
            {
                return NotFound();
            }
            if (!bookmarkEntity.Equals(parent))
                if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly)
                    return View("Error", new ErrorViewModel("Parent folder Locked!", "This folder cannot be unlocked, as the parent folder is locked."));

            bookmarkEntity.ReadOnly = true;
            LockChildren((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);

            _context.Update(bookmarkEntity);
            await _context.SaveChangesAsync();
            return Redirect("/BookmarkEntities/ViewContent/" + parent.Id);
        }

        public async Task<IActionResult> UnlockFromPath(string path)
        {
            if (path == null)
            {
                return NotFound();
            }

            Tuple<string, string> tuple = GetFolderAndParentFolder(path.Replace("item-", "").Replace("-", "|"));

            var bookmarkEntity = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (bookmarkEntity == null) return NotFound();

            Tuple<string, string> tuple2 = GetFolderAndParentFolder((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath);
            var parent = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (parent == null)
            {
                return NotFound();
            }
            if (!bookmarkEntity.Equals(parent))
                if ((bookmarkEntity.ParentPath != "Root" || bookmarkEntity.ParentPath != "") && parent.ReadOnly)
                    return View("Error", new ErrorViewModel("Parent folder Locked!", "This folder cannot be unlocked, as the parent folder is locked."));

            bookmarkEntity.ReadOnly = false;
            UnlockChildren((bookmarkEntity.ParentPath == "Root" || bookmarkEntity.ParentPath == "")
                ? bookmarkEntity.Name : bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);

            _context.Update(bookmarkEntity);
            await _context.SaveChangesAsync();
            return Redirect("/BookmarkEntities/ViewContent/" + parent.Id);
        }
        // GET: BookmarkEntities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmarkEntity = await _context.BookmarkEntity.FindAsync(id);
            if (bookmarkEntity == null)
            {
                return NotFound();
            }

            return toDetails(bookmarkEntity);
        }

        public IActionResult DetailsFromPath(string Parent, string returnUrl)
        {
            if (Parent == null) return NotFound();

            Tuple<string, string> tuple = GetFolderAndParentFolder(Parent.Replace("item-", "").Replace("-", "|"));
            var bookmark = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (bookmark == null) return NotFound();
            ViewBag.returnUrl = returnUrl;
            return toDetails(bookmark);
        }

        // GET: BookmarkEntities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmarkEntity = await _context.BookmarkEntity.SingleOrDefaultAsync(m => m.Id == id);
            if (bookmarkEntity == null)
            {
                return NotFound();
            }
            ViewBag.Types = BookmarkEntity.GetTypeAsSelectList(bookmarkEntity.BookmarkType);
            return toEdit(bookmarkEntity);
        }

        public IActionResult EditFromPath(string Parent, string returnUrl)
        {
            if (Parent == null) return NotFound();

            Tuple<string, string> tuple = GetFolderAndParentFolder(Parent.Replace("item-", "").Replace("-", "|"));
            var bookmark = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (bookmark == null) return NotFound();
            ViewBag.returnUrl = returnUrl;
            ViewBag.Types = BookmarkEntity.GetTypeAsSelectList(bookmark.BookmarkType);
            return toEdit(bookmark);
        }

        // POST: BookmarkEntities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Content, BookmarkType,Name,ReadOnly,ParentPath")] BookmarkEntity bookmarkEntity)
        {
            if (id != bookmarkEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookmarkEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookmarkEntityExists(bookmarkEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookmarkEntity);
        }

        // GET: BookmarkEntities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmarkEntity = await _context.BookmarkEntity
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bookmarkEntity == null)
            {
                return NotFound();
            }

            return toDelete(bookmarkEntity);
        }

        public IActionResult DeleteFromPath(string Parent, string returnUrl)
        {
            if (Parent == null) return NotFound();

            Tuple<string, string> tuple = GetFolderAndParentFolder(Parent.Replace("item-", "").Replace("-", "|"));
            var bookmark = bookmarkEntityRepo.GetBookMarkByNameAndParentPath(tuple.Item1, tuple.Item2);
            if (bookmark == null) return NotFound();

            ViewBag.returnUrl = returnUrl;
            return toDelete(bookmark);
        }

        // POST: BookmarkEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookmarkEntity = await _context.BookmarkEntity.SingleOrDefaultAsync(m => m.Id == id);
            if (bookmarkEntity.BookmarkType == BookmarkEntity.Type.FOLDER)
            {
                bookmarkEntityRepo.RemoveBookmarkEntitiesByParentPath(bookmarkEntity.ToString().Replace("/", "|"));
            }
            _context.BookmarkEntity.Remove(bookmarkEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: BookmarkEntities/CreateType
        public IActionResult CreateType(string ParentPath, string returnUrl)
        {
            if (ParentPath == null || ParentPath.Equals(""))
            {
                ViewBag.Path = GetAllPossiblePath();
            }
            else
            {
                ViewBag.Path = GetCurrentPath(ParentPath);
            }
            ViewBag.Types = BookmarkEntity.Gettypes();
            ViewBag.returnUrl = returnUrl;
            return View("Create");
        }

        //this method is a helper that directs the request based on bookmark type and action required!
        [HttpGet]
        public IActionResult Create(BookmarkEntity.Type BookmarkType, string ParentPath, string returnUrl)
        {
            if (ParentPath == null) return View("Error", new ErrorViewModel());

            ViewBag.returnUrl = returnUrl;
            ViewBag.Types = BookmarkEntity.Gettypes();

            switch (BookmarkType)
            {
                case BookmarkEntity.Type.FOLDER:
                    {
                        return View("Folder/CreateFolder", new Folder { ParentPath = ParentPath });
                    }
                case BookmarkEntity.Type.LINK:
                    {
                        return View("Link/CreateLink", new ItemLink { ParentPath = ParentPath });
                    }
                case BookmarkEntity.Type.LOCATION:
                    {
                        return View("Location/CreateLocation", new ItemLocation { ParentPath = ParentPath });
                    }
                case BookmarkEntity.Type.TEXTFILE:
                    {
                        return View("Textfile/CreateTextfile", new ItemTextFile { ParentPath = ParentPath });
                    }
                default:
                    {
                        ViewBag.Path = GetAllPossiblePath();
                        return View("Create");
                    }
            }
        }

        #region Folder
        // POST: BookmarkEntities/CreateFolder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder([Bind("Id,Name,ReadOnly,ParentPath")]
        Folder folder, string returnUrl)
        {
            BookmarkEntity parent = folderRepo.GetById((Convert.ToInt32(folder.ParentPath)));
            if (folder.ParentPath.Equals("-1"))
            {
                folder.ParentPath = "Root";
            }
            else
            {
                folder.ParentPath = parent.ToString().Replace("Root/", "").Replace("/", "|");
            }

            if (ModelState.IsValid)
            {
                if (parent.ReadOnly)
                {
                    return View("Error", new ErrorViewModel("Parent folder is not Editable!", "\'" + folder.ParentPath + "\' is readonly"));
                }

                if (bookmarkEntityRepo.NameExistById(folder.Name, folder.ParentPath, folder.Id))
                {
                    return View("Error", new ErrorViewModel("Folder already exist!", "A folder with name \'" + folder.Name + "\' already exist at \'" + folder.ParentPath + "\'"));
                }
                folderRepo.InsertAsync(folder);
                await _context.SaveChangesAsync();

                if (returnUrl != null && returnUrl.Equals("Home"))
                {
                    return Redirect("/Home");
                }
                else if (returnUrl == null)
                {
                    return Redirect("/BookmarkEntities");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            return View(folder);
        }

        #endregion

        #region Link
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLink([Bind("Id,Title, Uri, Content, Name, ReadOnly,ParentPath")]
        ItemLink link, string returnUrl)
        {
            BookmarkEntity parent = folderRepo.GetById((Convert.ToInt32(link.ParentPath)));
            if (link.ParentPath.Equals("-1") || link.ParentPath == "Root")
            {
                link.ParentPath = "Root";
            }
            else
            {
                if (parent == null) return NotFound();
                link.ParentPath = parent.ToString().Replace("/", "|");
            }

            if (ModelState.IsValid)
            {
                if (parent != null && parent.ReadOnly)
                {
                    return View("Error", new ErrorViewModel("Parent folder is not Editable!", "\'" + link.ParentPath + "\' is readonly"));
                }
                if (bookmarkEntityRepo.NameExistById(link.Name, link.ParentPath, link.Id))
                {
                    return View("Error", new ErrorViewModel());
                }
                itemLinkRepo.InsertAsync(link);
                await _context.SaveChangesAsync();

                if (returnUrl != null && returnUrl.Equals("Home"))
                {
                    return Redirect("/Home");
                }
                else
                {
                    return Redirect("/BookmarkEntities");
                }
            }
            return View(link);
        }

        #endregion

        #region Location
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation([Bind("Id,Title, Uri, Content, Name, ReadOnly,ParentPath")]
        ItemLocation location, string returnUrl)
        {
            BookmarkEntity parent = folderRepo.GetById((Convert.ToInt32(location.ParentPath)));
            if (location.ParentPath.Equals("-1"))
            {
                location.ParentPath = "Root";
            }
            else
            {
                
                if (parent == null) return NotFound();
                location.ParentPath = parent.ToString().Replace("/", "|");
            }

            if (ModelState.IsValid)
            {
                if (parent.ReadOnly)
                {
                    return View("Error", new ErrorViewModel("Parent folder is not Editable!", "\'" + location.ParentPath + "\' is readonly"));
                }
                if (bookmarkEntityRepo.NameExistById(location.Name, location.ParentPath, location.Id))
                {
                    return View("Error", new ErrorViewModel());
                }
                itemLocationRepo.InsertAsync(location);
                await _context.SaveChangesAsync();

                if (returnUrl != null && returnUrl.Equals("Home"))
                {
                    return Redirect("/Home");
                }
                else
                {
                    return Redirect("/BookmarkEntities");
                }
            }
            return View(location);
        }


        #endregion

        #region TextFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTextfile([Bind("Id,Title, Uri, Content, Name, ReadOnly,ParentPath")]
        ItemTextFile file, string returnUrl)
        {
            BookmarkEntity parent = folderRepo.GetById((Convert.ToInt32(file.ParentPath)));
            if (file.ParentPath.Equals("-1"))
            {
                file.ParentPath = "Root";
            }
            else
            {
                
                if (parent == null) return NotFound();
                file.ParentPath = parent.ToString().Replace("/", "|");
            }

            if (ModelState.IsValid)
            {
                if (parent.ReadOnly)
                {
                    return View("Error", new ErrorViewModel("Parent folder is not Editable!", "\'" + file.ParentPath + "\' is readonly"));
                }
                if (bookmarkEntityRepo.NameExistById(file.Name, file.ParentPath, file.Id))
                {
                    return View("Error", new ErrorViewModel());
                }
                itemTextfileRepo.InsertAsync(file);
                await _context.SaveChangesAsync();

                if (returnUrl != null && returnUrl.Equals("Home"))
                {
                    return Redirect("/Home");
                }
                else
                {
                    return Redirect("/BookmarkEntities");
                }
            }
            return View(file);
        }

        #endregion

        #region HelperMethods
        private bool BookmarkEntityExists(int id)
        {
            return _context.BookmarkEntity.Any(e => e.Id == id);
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
        private dynamic GetAllPossiblePath()
        {
            List<SelectListItem> Path = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Root",
                    Value = "-1"
                }
            };
            foreach (Folder folder in folderRepo.GetAll())
            {
                Path.Add(new SelectListItem
                {
                    Text = folder.ToString(),
                    Value = ((folder.Id).ToString())
                });
            }
            return Path;
        }
        private dynamic GetCurrentPath(string parentPath)
        {
            if (parentPath.Equals("Root"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Root",
                        Value = "-1"
                    }
                };
            }
            else
            {
                Tuple<string, string> tuple = GetFolderAndParentFolder(
                    parentPath.Replace("item-", "").Replace("-", "|"));
                var parent = folderRepo.GetFolderByNameAndParentPath(tuple.Item1, tuple.Item2);
                if (parent == null)
                {
                    return new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Root",
                        Value = "-1"
                    }
                };
                }
                return new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = parent.ToString(),
                        Value = ((parent.Id).ToString())
                    }
                };
            }
        }
        private IActionResult toDetails(BookmarkEntity bookmarkEntity)
        {
            switch (bookmarkEntity.BookmarkType)
            {
                case BookmarkEntity.Type.FOLDER:
                    {
                        if (bookmarkEntity.ParentPath == null || bookmarkEntity.ParentPath.Equals("") || bookmarkEntity.ParentPath.Equals("Root"))
                        {
                            ViewBag.DirectChild = bookmarkEntityRepo.NumberOfDirectItems(bookmarkEntity.Name);
                            ViewBag.IndirectChild = bookmarkEntityRepo.NumberOfIndirectItems(bookmarkEntity.Name);
                        }
                        else
                        {
                            ViewBag.DirectChild = bookmarkEntityRepo.NumberOfDirectItems(bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);
                            ViewBag.IndirectChild = bookmarkEntityRepo.NumberOfIndirectItems(bookmarkEntity.ParentPath + "|" + bookmarkEntity.Name);
                        }

                        return View("Folder/Details", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LINK:
                    {
                        return View("Link/Details", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LOCATION:
                    {
                        return View("Location/Details", bookmarkEntity);
                    }
                case BookmarkEntity.Type.TEXTFILE:
                    {
                        return View("Textfile/Details", bookmarkEntity);
                    }
                default:
                    {
                        return View("Details");
                    }
            }
        }
        private IActionResult toEdit(BookmarkEntity bookmarkEntity)
        {
            if (bookmarkEntity.ReadOnly) return View("Error", new ErrorViewModel("Item Readonly!", "\'" + bookmarkEntity.Name + "\' is readonly, therefor it cannot be edit."));
            switch (bookmarkEntity.BookmarkType)
            {
                case BookmarkEntity.Type.FOLDER:
                    {
                        return View("Folder/Edit", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LINK:
                    {
                        return View("Link/Edit", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LOCATION:
                    {
                        return View("Location/Edit", bookmarkEntity);
                    }
                case BookmarkEntity.Type.TEXTFILE:
                    {
                        return View("Textfile/Edit", bookmarkEntity);
                    }
                default:
                    {
                        return View("Edit");
                    }
            }
        }
        private IActionResult toDelete(BookmarkEntity bookmarkEntity)
        {
            if (bookmarkEntity.ReadOnly) return View("Error", new ErrorViewModel("Item Readonly!", "\'" + bookmarkEntity.Name + "\' is readonly, therefor it cannot be deleted."));
            switch (bookmarkEntity.BookmarkType)
            {
                case BookmarkEntity.Type.FOLDER:
                    {
                        ViewBag.DirectChild = bookmarkEntityRepo.NumberOfDirectItems(bookmarkEntity.ToString().Replace("/", "|"));
                        ViewBag.IndirectChild = bookmarkEntityRepo.NumberOfIndirectItems(bookmarkEntity.ToString().Replace("/", "|"));
                        return View("Folder/Delete", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LINK:
                    {
                        return View("Link/Delete", bookmarkEntity);
                    }
                case BookmarkEntity.Type.LOCATION:
                    {
                        return View("Location/Delete", bookmarkEntity);
                    }
                case BookmarkEntity.Type.TEXTFILE:
                    {
                        return View("Textfile/Delete", bookmarkEntity);
                    }
                default:
                    {
                        return View("Edit");
                    }
            }
        }
        private void LockChildren(string v)
        {
            _context.BookmarkEntity.Where(b => b.ParentPath.StartsWith(v))
                .ToList()
                .ForEach(i => i.ReadOnly = true);
        }
        private void UnlockChildren(string v)
        {
            _context.BookmarkEntity.Where(b => b.ParentPath.StartsWith(v))
                .ToList()
                .ForEach(i => i.ReadOnly = false);
        }
        #endregion
    }
}
