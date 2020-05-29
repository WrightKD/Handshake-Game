using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dapper;
using Handshake.Models;
using Handshake.Wrappers.CovidStats;
using Handshake.Wrappers.Mapbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApp.Models;

namespace Handshake.GameLogic
{
    public class GameService
    {
        public Player player { get; set; }
        public IConfiguration Configuration { get; set; }
        public List<NPC> NPCs { get; set; }
        public List<Shop> shops { get; set; }
        //player vars
        private readonly int handShakePoints;
        private readonly double playerBaseInfectionChance; //modified by weather/going to shop/work
        public readonly double maskDuration;
        private bool isMaskOn;
        private DateTime timeMaskUsed;
        //shop vars
        private readonly int sanitiserInitialCount;
        private readonly int sanitiserCost;
        private readonly int maskInitialCount;
        private readonly int maskCost;
        private readonly int testInitialCount;
        private readonly int testCost;
        private readonly double shopResetMinutes;
        //npc vars
        private readonly Random spawnRoll;
        private readonly Random infectionRoll;
        private readonly double valForRandomDouble;
        private readonly double nPCBaseInfectedChance; //affected by regional stats
        private readonly double nPCInfectedChanceOffset;
        private readonly double nPCRefreshTime;

        public GameService(IConfiguration configuration) 
        {
            Configuration = configuration;
            NPCs = new List<NPC>();
            shops = new List<Shop>();
            //player config
            handShakePoints = 1;
            playerBaseInfectionChance = 0.5;
            maskDuration = 5;
            isMaskOn = false;
            timeMaskUsed = DateTime.MinValue;
            //shop config
            sanitiserInitialCount = 2;
            sanitiserCost = 10;
            maskInitialCount = 1;
            maskCost = 50;
            testInitialCount = 1;
            testCost = 20;
            shopResetMinutes = 2;
            //npc config
            spawnRoll = new Random((int)DateTime.Now.Ticks);
            infectionRoll = new Random((int)DateTime.Now.Ticks);
            valForRandomDouble = 0.01;
            nPCBaseInfectedChance = 0.5;
            nPCInfectedChanceOffset = 0.3;
            nPCRefreshTime = 1;
            //GenerateNPCs(100);
            //TEMPGenerateShop(1);
        }

        public Player GetDefaultPlayer()
        {
            return new Player
            {
                ScoreCurrent = 0,
                ScoreTotal = 0,
                ScorePerLevel = 10,
                Level = 1,
                IsInfected = false,
                IsContaminated = false,
                SanitiserCount = 2,
                MaskCount = 0,
                Gold = 100,
                CovidTests = 0
            };
        }

        public async Task LoadPlayerDataAsync(int userId)
        {
            //make it static ?
            player = await GetPlayerAsync(userId);
        }

        public async Task SavePlayerData()
        {
            //player.Gold = 10000000;
            await UpdatePlayerAsync(player);
        }
        private void GenerateNPCs(int numberToSpawn, double lat, double lon)
        {
            //var lat = "-26.1796856";
            //var lon = "28.0509079";
            double nPCAdjustedInfectedChance;
            var location = MapboxService.GetLocationDetails($"{lon}", $"{lat}");

            var province = MapboxService.GetProvince(location);
            if (province == "Gauteng")
            {
                var a = CovidStatsService.GetStats();
                var b = a.RSA.GP[0].Cases[a.RSA.GP[0].Cases.Count - 1];
                int x = (int)Math.Log10(b);
                nPCAdjustedInfectedChance = 0.1 * x + nPCBaseInfectedChance - nPCInfectedChanceOffset;
            }
            else
                nPCAdjustedInfectedChance = nPCBaseInfectedChance;

            for (int i = 0; i < numberToSpawn; i++)
            {
                NPCs.Add(new NPC(i, nPCAdjustedInfectedChance, GetNPCName(i), GetNPCTraits(i)));
            }
        }

        public void RandomiseNPCLocations(double playerLatitude, double playerLongitude)
        {
            GenerateNPCs(50, playerLatitude, playerLongitude);
            foreach (var npc in NPCs)
            {
                npc.Latitude = playerLatitude + GetRandomDouble();
                npc.Longitude = playerLongitude + GetRandomDouble();
            }
        }

        public bool IsNPCReady(int npcId)
        {
            NPC npc = NPCs.Find(x => x.ID == npcId);
            if (npc != null)
            {
                if ((DateTime.Now - npc.LastInteractedTime).TotalMinutes > nPCRefreshTime)
                    return true;
                else
                    return false;
            }
            else
            {
                throw new NullReferenceException("Invalid NPC ID");
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

            NPC npc = NPCs.Find(x => x.ID == npcId);
            if (npc != null)
            {
                npc.LastInteractedTime = DateTime.Now;
                if (npc.IsInfected && !CheckMaskOn())
                {
                    if (infectionRoll.NextDouble() < playerBaseInfectionChance)
                        player.IsInfected = true;
                }
            }
            else
            {
                throw new NullReferenceException("Invalid NPC ID");
            }
        }

        private bool CheckMaskOn()
        {
            if (isMaskOn)
            {
                if ((DateTime.Now - timeMaskUsed).TotalMinutes > maskDuration)
                {
                    isMaskOn = false;
                    return false;
                }
                return true;
            }
            return false;
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
            if (player.MaskCount > 0)
            {
                player.MaskCount--;
                isMaskOn = true;
                timeMaskUsed = DateTime.Now;
            }
        }

        public void UseTest()
        {
            if (player.CovidTests > 0)
            {
                player.CovidTests--;
            }
        }

        public Shop GetShopInventory(int shopId)
        {
            Shop shop = shops.Find(x => x.ID == shopId);
            if (shop != null)
            {
                return shops.Find(x => x.ID == shopId);
            }
            else
            {
                throw new NullReferenceException("Invalid shop ID");
            }
        }

        public Shop BuySanitiser(int shopId)
        {
            Shop shop = shops.Find(x => x.ID == shopId);
            if (shop != null)
            {
                if (shop.SanitiserCount > 0 && player.Gold >= shop.SanitiserCost)
                {
                    shop.SanitiserCount--;
                    player.SanitiserCount++;
                    player.Gold -= shop.SanitiserCost;
                }
                return shop;
            }
            else
            {
                throw new NullReferenceException("Invalid shop ID");
            }
        }

        public Shop BuyMask(int shopId)
        {
            Shop shop = shops.Find(x => x.ID == shopId);
            if (shop != null)
            {
                if (shop.MaskCount > 0 && player.Gold >= shop.MaskCost)
                {
                    shop.MaskCount--;
                    player.MaskCount++;
                    player.Gold -= shop.MaskCost;
                }
                return shop;
            }
            else
            {
                throw new NullReferenceException("Invalid shop ID");
            }
        }

        public Shop BuyTest(int shopId)
        {
            Shop shop = shops.Find(x => x.ID == shopId);
            if (shop != null)
            {
                if (shop.TestCount > 0 && player.Gold >= shop.TestCost)
                {
                    shop.TestCount--;
                    player.CovidTests++;
                    player.Gold -= shop.TestCost;
                }
                return shop;
            }
            else
            {
                throw new NullReferenceException("Invalid shop ID");
            }
        }

        public void AddShop(int id, string name)
        {
            shops.Add(new Shop(id, name, sanitiserInitialCount, sanitiserCost, maskInitialCount, maskCost, testInitialCount, testCost));
        }

        private double GetRandomDouble()
        {
            return spawnRoll.NextDouble() * (2 * valForRandomDouble) - valForRandomDouble;
        }

        public async Task<Player> GetPlayerAsync(int userId)
        {

            using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                return await connection.QuerySingleOrDefaultAsync<Player>($@"SELECT * FROM [ApplicationPlayer]
                WHERE [Id] = @{nameof(userId)}", new { userId });
            }
        }

        public async Task<IdentityResult> UpdatePlayerAsync(Player player)
        {

            using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync($@"UPDATE [ApplicationPlayer] SET
                [ScoreCurrent] = @{nameof(Player.ScoreCurrent)},
                [ScoreTotal] = @{nameof(Player.ScoreTotal)},
                [Level] = @{nameof(Player.Level)},
                [ScorePerLevel] = @{nameof(Player.ScorePerLevel)},
                [SanitiserCount] = @{nameof(Player.SanitiserCount)},
                [Gold] = @{nameof(Player.Gold)},
                [MaskCount] = @{nameof(Player.MaskCount)},
                [IsContaminated] = @{nameof(Player.IsContaminated)},
                [IsInfected] = @{nameof(Player.IsInfected)},
                [CovidTests] = @{nameof(Player.CovidTests)}
                WHERE [Id] = Id", player);
            }

            return IdentityResult.Success;
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
