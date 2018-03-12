namespace CW03.Models
{
    public class OrgChartHelpModel
    {
        public string name { get; set; }
        public string parentName { get; set; }
        public int id { get; set; }
        public int parentId { get; set; }
    }
}
