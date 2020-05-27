using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class NPC
    {
        public NPC(bool randomiseInfection, int id, string name="John", string story="A simple man") 
        {
            if (randomiseInfection)
                RandomiseInfection(0.5);
            else
                IsInfected = false;
            ID = id;
            Name = name;
            Story = story;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Story { get; set; }
        public bool IsInfected { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        private bool IsFriend { get; set; }

        private void RandomiseInfection(double infectionChance)
        {
            Random infectionRoll = new Random();
            if (infectionRoll.NextDouble() < infectionChance)
                IsInfected = true;
            else
                IsInfected = false;
        }
    }
}
