namespace CW03.Models
{
    public class Item:BookmarkEntity
    {

        private string _content;
        public Item():base()
        {
        }

        public Item(string path) : base(path)
        {
        }

        public string Content { get => _content; set => _content = value; }
    }
}