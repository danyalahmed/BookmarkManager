using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CW03.Models
{
    public class BookmarkEntity
    {
        public enum Type
        {
            FOLDER, LINK, LOCATION, TEXTFILE
        }

        [Key]
        [Required]
        private int _id;
        private Type _bookmarkType;
        [Required]
        private string _name;
        private bool _readOnly;
        private string _parentPath;

        public int Id { get => _id; set => _id = value; }

        [Display(Name = "Type")]
        public Type BookmarkType { get => _bookmarkType; set => _bookmarkType = value; }
        public string Name { get => _name; set => _name = value; }

        [Display(Name = "Read Only")]
        public bool ReadOnly { get => _readOnly; set => _readOnly = value; }

        [Display(Name = "Path")]
        public string ParentPath { get => _parentPath; set => _parentPath = value; }

        public BookmarkEntity() { }
        public BookmarkEntity(String name) => Name = name;

        public override string ToString()
        {
            if (ParentPath == null || ParentPath.Equals(string.Empty))
            {
                return Name;
            }
            else
            {
                return ParentPath.Replace("|", "/") + "/" + Name;
            }

        }

        public static dynamic Gettypes()
        {
            return Enum.GetValues(typeof(Type)).Cast<Type>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() });
        }

        internal static dynamic GetTypeAsSelectList(Type bookmarkType)
        {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = bookmarkType.ToString(),
                    Value = ((int)bookmarkType).ToString()
                }
            };            
        }
    }
}