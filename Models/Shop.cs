using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class Shop
    {
        public Shop(int id, string name, int sanitiserCount, int sanitiserCost, int maskCount, int maskCost, int testCount, int testCost)
        {
            ID = id;
            Name = name;
            SanitiserCount = sanitiserCount;
            SanitiserCost = sanitiserCost;
            MaskCount = maskCount;
            MaskCost = maskCost;
            TestCount = testCount;
            TestCost = testCost;
            RefreshedTime = DateTime.Now;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int SanitiserCount { get; set; }
        public int SanitiserCost { get; set; }
        public int MaskCount { get; set; }
        public int MaskCost { get; set; }
        public int TestCount { get; set; }
        public int TestCost { get; set; }
        public DateTime RefreshedTime { get; set; }
    }
}
