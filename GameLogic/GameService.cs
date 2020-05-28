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
        private double nPCRefreshTime;

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
            nPCRefreshTime = 1;

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
                NPCs.Add(new NPC(i, nPCInfectedChance, GetNPCName(i), GetNPCTraits(i)));
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

        public bool IsNPCReady(int npcId)
        {
            NPC npc = NPCs.Find(x => x.ID == npcId);
            if ((DateTime.Now - npc.LastInteractedTime).TotalMinutes > nPCRefreshTime)
                return true;
            else
                return false;
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

            NPC npc = NPCs.Find(x => x.ID == npcId);
            npc.LastInteractedTime = DateTime.Now;
            if (npc.IsInfected)
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
                shop.SanitiserCount--;
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
                shops.Add(new Shop(i, sanitiserCount, sanitiserCost, maskCount, maskCost));
            }
        }

        private double GetRandomDouble()
        {
            return spawnRoll.NextDouble() * (2 * valForRandomDouble) - valForRandomDouble;
        }
        private string GetNPCName(int id)
        {
            List<string> names = new List<string>()
            {
                "Shoaib Herbert",
                "Aiyla Tran",
                "Archibald Akhtar",
                "Elsie-Mae Greene",
                "Missy Mcleod",
                "Sheila Burrows",
                "Youssef Durham",
                "Eduardo Knox",
                "Austen Bryant",
                "Suleman Goodman",
                "Callen Sharples",
                "Sumayyah Allison",
                "Malaikah Langley",
                "Patrik Koch",
                "Rishi Buxton",
                "Hallam Sumner",
                "Zackary Wade",
                "Montague Robles",
                "Frank Mack",
                "Johan Handley",
                "Nel Chadwick",
                "Xander Patel",
                "Lubna Nunez",
                "Kerry Macleod",
                "Gina Guevara",
                "Phoebe Mcgrath",
                "Simrah Wise",
                "Ali East",
                "Alina Cotton",
                "Todd Wagner",
                "Clark Suarez",
                "Esther Mooney",
                "Ruby-Rose Montoya",
                "Jagdeep Connor",
                "Ida Glass",
                "Riley-James Stark",
                "Tabitha Higgins",
                "Joyce Dougherty",
                "Efe Bruce",
                "Kayleigh Kearns",
                "Shamima Howell",
                "Vanesa Lake",
                "Caspian Clements",
                "Macey Daniel",
                "Elizabeth Peters",
                "Kathleen Witt",
                "Natalya Power",
                "Faizaan Mustafa",
                "Kamron Rice",
                "Callum Gardiner",
                "Nayan Sparks",
                "Nile Hamer",
                "Kiana Hirst",
                "Kaleem Dunn",
                "Dewey Tucker",
                "Alanah Hickman",
                "Zaina Kidd",
                "Kerri Fellows",
                "Maxine Ellison",
                "Abdur-Rahman Worthington",
                "Darrel Curtis",
                "Rosa Beaumont",
                "Jordon Marsh",
                "Corban Finnegan",
                "Ryley Wardle",
                "Ada Oneal",
                "Codie Senior",
                "Blossom Stuart",
                "Jamel Mcdonnell",
                "Wilson Blake",
                "Koa Lim",
                "Darius Mays",
                "Dion Walker",
                "Darin Joyner",
                "Santiago Charles",
                "Salma Avalos",
                "Jaydn Calvert",
                "Gurpreet Summers",
                "Nansi Hanson",
                "Danika Macias",
                "Ignacy Watkins",
                "Juliet Sierra",
                "Cain Chavez",
                "Patsy Mendoza",
                "Anum Prosser",
                "Amelia-Lily Evans",
                "Cristian Chester",
                "Abubakar Maguire",
                "Grover Logan",
                "Rohit Bass",
                "Hajrah Villarreal",
                "Anas Neal",
                "Carolyn Arroyo",
                "Javan Hernandez",
                "Kaan Ballard",
                "Kayan Bonner",
                "Ariel Ferreira",
                "Connar Bowes",
                "Brax Burris",
                "Morwenna Mckee"
            };

            return names[id];
        }

        private string GetNPCTraits(int id)
        {
            List<string> traits = new List<string>()
            {
                "devious, mysterious and decisive",
                "easy-going, generous and cheerful",
                "courageous, overbearing and ambitious",
                "brilliant, dependable and spontaneous",
                "selfish, violent and foolish",
                "sly, materialistic and restless",
                "depressed, wise and rude",
                "independent, judgemental and wise",
                "rude, eccentric and stubborn",
                "secretive, lonely and ambitious",
                "uninhibited, violent and caring",
                "dishonest, charming and uninhibited",
                "fun-loving, imaginative and shy",
                "cruel, courageous and anxious",
                "daring, fun-loving and industrious",
                "wise, caring and courageous",
                "naive, selfish and depressed",
                "materialistic, foolish and conscientious",
                "materialistic, easy-going and inspirational",
                "materialistic, daring and imaginative",
                "rebellious, uninhibited and idealistic",
                "cheerful, compassionate and materialistic",
                "optimistic, carefree and idealistic",
                "naive, kindly and cheerful",
                "coarse, nurturing and courageous",
                "optimistic, restless and reckless",
                "inspirational, lively and shy",
                "conscientious, optimistic and conceited",
                "caring, arrogant and decisive",
                "mysterious, restless and judgemental",
                "fearless, charming and idealistic",
                "depressed, restless and bold",
                "nurturing, caring and devious",
                "charming, caring and eccentric",
                "considerate, cheerful and shy",
                "lonely, nurturing and eccentric",
                "industrious, cruel and naive",
                "cowardly, lively and helpful",
                "conscientious, rebellious and materialistic",
                "cheerful, secretive and stubborn",
                "cowardly, coarse and lively",
                "conscientious, reckless and idealistic",
                "brilliant, rude and caring",
                "rude, mysterious and sensitive",
                "ambitious, cowardly and rebellious",
                "naive, restless and fun-loving",
                "lively, fun-loving and bold",
                "aggressive, compassionate and idealistic",
                "inspirational, devious and idealistic",
                "forceful, overbearing and devious",
                "foolish, industrious and selfish",
                "spontaneous, wise and aggressive",
                "secretive, spontaneous and idealistic",
                "compassionate, courageous and manipulative",
                "spontaneous, independent and manipulative",
                "rude, bold and anxious",
                "sly, bold and cowardly",
                "lonely, rude and idealistic",
                "restless, ambitious and caring",
                "secretive, cheerful and aggressive",
                "unkind, wise and naive",
                "industrious, ambitious and selfish",
                "cheerful, manipulative and stubborn",
                "spontaneous, selfish and shy",
                "inspirational, lonely and cowardly",
                "inspirational, forceful and dishonest",
                "conscientious, timid and compassionate",
                "forceful, courageous and restless",
                "charming, aggressive and cheerful",
                "mysterious, rebellious and shy",
                "spontaneous, naive and aggressive",
                "optimistic, secretive and industrious",
                "optimistic, lively and selfish",
                "dishonest, conscientious and naive",
                "dependable, secretive and decisive",
                "charming, rude and brilliant",
                "conceited, lively and coarse",
                "shy, lonely and naive",
                "unkind, manipulative and compassionate",
                "aggressive, fearless and violent",
                "imaginative, helpful and charming",
                "overbearing, stubborn and violent",
                "wise, judgemental and sensitive",
                "optimistic, inspirational and generous",
                "wise, rude and carefree",
                "brilliant, helpful and daring",
                "conscientious, fearless and foolish",
                "foolish, dependable and naive",
                "reckless, cowardly and compassionate",
                "dependable, spontaneous and brilliant",
                "sly, naive and idealistic",
                "eccentric, rude and aggressive",
                "foolish, bold and independent",
                "generous, overbearing and uninhibited",
                "bold, independent and spontaneous",
                "easy-going, materialistic and dishonest",
                "forceful, lively and cheerful",
                "arrogant, violent and fun-loving",
                "independent, mysterious and reckless",
                "courageous, conscientious and aggressive"
            };
            return traits[id];
        }
    }
}
