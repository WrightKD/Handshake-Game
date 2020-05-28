using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dapper;
using Handshake.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApp.Models;

namespace Handshake.GameLogic
{
    public class GameService
    {
        
        public List<NPC> NPCs { get; set; }
        public List<Shop> shops { get; set; }
        //shop vars
        private int sanitiserCount;
        private int sanitiserCost;
        private int maskCount;
        private int maskCost;
        private DateTime shopResetTime;
        //player vars
        private int handShakePoints;
        private double playerInfectionChance; //modified by weather/going to shop/work
        private bool isMaskOn;
        private DateTime timeMaskUsed;
        private double maskDuration;
        //npc vars
        private Random spawnRoll;
        private Random infectionRoll;
        private double valForRandomDouble;
        private double nPCInfectedChance; //affected by regional stats
        private double nPCRefreshTime;

        public Player player { get; set; }
        public IConfiguration Configuration { get; set; }

        public GameService(IConfiguration configuration) 
        {

            Configuration = configuration;

            
			handShakePoints = 1;
            NPCs = new List<NPC>();
            spawnRoll = new Random((int)DateTime.Now.Ticks);
            infectionRoll = new Random((int)DateTime.Now.Ticks);
            valForRandomDouble = 0.01;
            isMaskOn = false;
            timeMaskUsed = DateTime.MinValue;
            maskDuration = 5;
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
                Gold = 100
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
                    if (infectionRoll.NextDouble() < playerInfectionChance)
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
                if (shop.SanitiserCount > 0 && player.Gold > shop.SanitiserCost)
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
                if (shop.MaskCount > 0 && player.Gold > shop.MaskCost)
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
                [IsInfected] = @{nameof(Player.IsInfected)}
                WHERE [Id] = Id", player);
            }

            return IdentityResult.Success;
        }
    }
}
