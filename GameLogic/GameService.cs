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
        public Player player;
        public List<NPC> NPCs;
        public List<Shop> shops;
        private Random spawnRoll;
        //private double minVal;
        //private double maxVal;
        private double valForRandomDouble;
        private DateTime shopResetTime;

        public GameService() 
        { 
            player = new Player();
            spawnRoll = new Random((int)DateTime.Now.Ticks);
            //maxVal = 0.005;
            //minVal = -0.005;
            valForRandomDouble = 0.005;
            shopResetTime = new DateTime();
            shopResetTime.AddMinutes(2);

            GenerateNPCs(10);
            TEMPGenerateShop(1);
        }

        public void ShakeHand(int npcId, double infectionChance)
        {
            player.ScoreTotal += player.HandshakePoints;
            player.ScoreCurrent += player.HandshakePoints;
            if (player.ScoreCurrent >= player.ScorePerLevel)
            {
                player.Level += player.ScoreCurrent / player.ScorePerLevel;
                player.ScoreCurrent = player.ScoreCurrent - player.ScorePerLevel;
            }

            if (NPCs.Find(x => x.ID == npcId).IsInfected)
            {
                Random infectionRoll = new Random();
                if (infectionRoll.NextDouble() < infectionChance)
                    player.IsInfected = true;
            }
        }

        public void UseSanitiser()
        {
            if (player.SanitiserCount > 0)
            {
                player.SanitiserCount--;
                player.IsInfected = false;
            }
        }

        public void LoadPlayerData()
        {
            //gets data from DB
        }

        public void SavePlayerData()
        {

        }

        public object GetShopInventory(int shopId)
        {
            //need to add reset
            return shops.Find(x => x.ID == shopId);
        }

        public Shop BuySanitiser(int shopId)
        {
            Shop shop = shops.Find(x => x.ID == shopId);
            if (shop.SanitiserCount >= 0 && player.Gold > shop.SanitiserCost)
            {
                shop.SanitiserCount --;
                player.SanitiserCount++;
                player.Gold -= shop.SanitiserCost;
            }
            return shop;
        }
        
        private void TEMPGenerateShop(int numberToSpawn)
        {
            shops = new List<Shop>();
            for (int i = 0; i < numberToSpawn; i++)
            {
                shops.Add(new Shop(i));
            }
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
            return spawnRoll.NextDouble() * (2 * valForRandomDouble) - valForRandomDouble;
        }
    }
}
