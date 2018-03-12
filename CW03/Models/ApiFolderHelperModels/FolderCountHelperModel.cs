namespace CW03.Models.ApiFolderHelperModels
{
    public class FolderCountHelperModel
    {
        public FolderCountHelperModel()
        {
        }

        public FolderCountHelperModel(int direct, int indirect)
        {
            this.direct = direct;
            this.indirect = indirect;
        }

        public int direct { get; set; }
        public int indirect { get; set; }
    }
}
