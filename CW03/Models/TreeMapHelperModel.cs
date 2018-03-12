using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW03.Models
{
    public class TreeMapHelperModel
    {
        public string name { get; set; }
        public string parentName { get; set; }
        public int id { get; set; }
        public int parentId { get; set; }
        public int items { get; set; }
    }
}
