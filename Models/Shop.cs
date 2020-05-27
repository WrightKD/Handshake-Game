using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class Shop
    {
        public Shop(int id)
        {
            ID = id;
            SanitiserCount = 2;
            SanitiserCost = 10;
            MaskCount = 1;
            RefreshedTime = DateTime.Now;
        }

        public int ID { get; set; }
        public int SanitiserCount { get; set; }
        public int SanitiserCost { get; set; }
        public int MaskCount { get; set; }
        public int MaskCost { get; set; }
        public DateTime RefreshedTime { get; set; }
    }
}
