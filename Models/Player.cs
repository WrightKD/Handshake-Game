using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class Player
    {
        public Player() {
            ScoreCurrent = 0;
            ScoreTotal = 0;
            ScorePerLevel = 10;
            Level = 1;
            IsInfected = false;
            IsContaminated = false;
            HandshakePoints = 1; 
            SanitiserCount = 2;
            MaskCount = 0;
            Gold = 100;
        }
        
        public int ScoreCurrent { get; set; }
        public int ScoreTotal { get; set; }
        //for future: can level up after a certain score (access better actions etc.)
        public int Level { get; set; }
        public int ScorePerLevel { get; set; }
        public int SanitiserCount { get; set; }
        public int MaskCount { get; set; }
        public int Gold { get; set; }
        public bool IsInfected { get; set; }
        public bool IsContaminated { get; set; }
        public int HandshakePoints { get; set; }
    }
}
