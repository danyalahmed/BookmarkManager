using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CW03.Models
{
    public class ItemLink : Item
    {

        private string _uri;
        private string _title;

        public ItemLink()
        {
            BookmarkType = Type.LINK;
        }

        public ItemLink(string name, string uri, string title) : base(name)
        {
            BookmarkType = Type.LINK;
            Content = uri;
            _title = title;
        }

        public string Title { get => _title; set => _title = value; }

        [NotMapped]
        [Display(Name="Content")]
        public string Uri { get => Content; set => Content = value; }

        public string GetContent() => Uri;
    }
}