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
        //shop vars
        private int sanitiserCount;
        private int sanitiserCost;
        private int maskCount;
        private int maskCost;
        private DateTime shopResetTime;
        //player vars
        private int handShakePoints;
        private double playerInfectionChance; //modified by weather/going to shop/work
        //npc vars
        private Random spawnRoll;
        private Random infectionRoll;
        private double valForRandomDouble;
        private double nPCInfectedChance; //affected by regional stats

    public GameService() 
        { 
            player = new Player();
            handShakePoints = 1;
            NPCs = new List<NPC>();
            spawnRoll = new Random((int)DateTime.Now.Ticks);
            infectionRoll = new Random((int)DateTime.Now.Ticks);
            valForRandomDouble = 0.01;
            //shop config
            shopResetTime = new DateTime();
            shopResetTime.AddMinutes(2);
            sanitiserCount = 2;
            sanitiserCost = 10;
            maskCount = 1;
            maskCost = 50;
            //infection
            nPCInfectedChance = 0.5;
            playerInfectionChance = 0.5;

            GenerateNPCs(50);
            TEMPGenerateShop(1);
        }

        public void LoadPlayerData()
        {
            //gets data from DB
        }

        public void SavePlayerData()
        {

        }
        private void GenerateNPCs(int numberToSpawn)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                NPCs.Add(new NPC(i, nPCInfectedChance, "John", "A simple man"));
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

        public void ShakeHand(int npcId)
        {
            player.ScoreTotal += handShakePoints;
            player.ScoreCurrent += handShakePoints;
            if (player.ScoreCurrent >= player.ScorePerLevel)
            {
                player.Level += player.ScoreCurrent / player.ScorePerLevel;
                player.ScoreCurrent = player.ScoreCurrent - player.ScorePerLevel;
            }

            if (NPCs.Find(x => x.ID == npcId).IsInfected)
            {
                if (infectionRoll.NextDouble() < playerInfectionChance)
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

        public void UseMask()
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
            if (shop.SanitiserCount > 0 && player.Gold > shop.SanitiserCost)
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
                shops.Add(new Shop(i,sanitiserCount, sanitiserCost, maskCount, maskCost));
            }
        }

        private double GetRandomDouble()
        {
            return spawnRoll.NextDouble() * (2 * valForRandomDouble) - valForRandomDouble;
        }
    }
}
