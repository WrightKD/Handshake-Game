using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class Player
    {
        
        public int ScoreCurrent { get; set; }
        public int ScoreTotal { get; set; }
        public int Level { get; set; }
        public int ScorePerLevel { get; set; }
        public int SanitiserCount { get; set; }
        public int MaskCount { get; set; }
        public int Gold { get; set; }
        public int CovidTests { get; set; }
        public bool IsInfected { get; set; }
        public bool IsContaminated { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
