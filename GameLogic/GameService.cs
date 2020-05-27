using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Handshake.Models;

namespace Handshake.GameLogic
{
    public class GameService
    {
        public Player _player;
        public List<NPC> NPCs;
        private Random spawnRoll;
        private double minVal;
        private double maxVal;

        public GameService() 
        { 
            _player = new Player();
            spawnRoll = new Random((int)DateTime.Now.Ticks);
            maxVal = 0.005;
            minVal = -0.005;

            GenerateNPCs(10);
        }

        public void ShakeHand(double infectionChance)
        {
            _player.IncreasePoints(1);
            Random infectionRoll = new Random();
            if (infectionRoll.NextDouble() < infectionChance)
                _player.BecomeInfected();
        }

        public void GetPlayerData()
        {
            //gets data from DB
        }

        private void GenerateNPCs(int numberToSpawn)
        {
            NPCs = new List<NPC>();
            for (int i = 0; i < numberToSpawn; i++)
            {
                NPCs.Add(new NPC(true, i));
            }
        }

        public void RandomiseNPCLocations(double playerLatitude, double playerLongitude)
        {
            foreach (var npc in NPCs)
            {
                npc.Latitude = playerLatitude + GetRandomDouble();
                npc.Longitude = playerLongitude + GetRandomDouble();
            }
        }

        private double GetRandomDouble()
        {
            return spawnRoll.NextDouble() * (maxVal - minVal) + minVal;
        }
    }
}
