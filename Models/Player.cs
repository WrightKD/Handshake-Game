using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class Player
    {
        public Player() { Score = 0; IsInfected = false; handshakePoints = 1; ItemCount = 2; }
        
        public int Score { get; set; }
        //for future: can level up after a certain score (access better actions etc.)
        public int SocialLevel { get; set; }
        public int ItemCount { get; set; }
        public bool IsInfected { get; set; }
        private int handshakePoints { get; set; }

        public void IncreasePoints(int amount)
        {
            Score += amount;
        }

        public void BecomeInfected()
        {
            IsInfected = true;
        }

        public void Sanitise()
        {
            ItemCount--;
            IsInfected = false;
        }
    }
}
