﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Models
{
    public class NPC
    {
        public NPC(bool randomiseInfection, int id, string name="John", string description="A simple man") 
        {
            if (randomiseInfection)
                RandomiseInfection(0.5);
            else
                IsInfected = false;
            ID = id;
            Name = name;
            Description = description;
            LastInteractedTime = new DateTime(DateTime.MinValue.Ticks);
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsInfected { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime LastInteractedTime { get; set; }
        //for future
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
