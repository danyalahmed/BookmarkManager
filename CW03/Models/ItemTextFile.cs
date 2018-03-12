namespace CW03.Models
{
    public class ItemTextFile : Item
    {


        public ItemTextFile() : base()
        {
            BookmarkType = Type.TEXTFILE;
        }

        public ItemTextFile(string title, string text) : base(title)
        {
            BookmarkType = Type.TEXTFILE;
            Content = text;
        }
    }
}