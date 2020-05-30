using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CypherTools.Core.Models;
using CypherTools.Core.DataAccess.Repos;

namespace CypherTools.Bot.Services
{
    public static class CypherHelper
    {
        public static async Task<List<Cypher>> GetAllCyphersAsync()
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            var cyList = await db.Cyphers.Include(x => x.EffectOptions).Include(x => x.Forms).ToListAsync();

            return cyList;
        }

        public static async Task<Cypher> GetRandomCypherAsync()
        {
            var cyList = await GetAllCyphersAsync();

            var i = new Random().Next(0, cyList.Count() - 1);

            return cyList[i];
        }

        public static async Task<List<Cypher>> GetRandomCypherAsync(int numberOfCyphers)
        {
            var ls = new List<Cypher>();
            var rnd = RandomGenerator.GetRandom();

            for (int i = 0; i < numberOfCyphers; i++)
            {
                var cypherList = await GetAllCyphersAsync();
                ls.Add(cypherList[rnd.Next(1, cypherList.Count)]);
            }

            return ls;
        }

        public static async Task RemoveUnidentifiedCypherAsync(int unidentifiedCypherID)
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            var uCypherToRemove = new UnidentifiedCypher() { UnidentifiedCypherId = unidentifiedCypherID };
            db.UnidentifiedCyphers.Remove(uCypherToRemove);
            await db.SaveChangesAsync();
        }

        public static async Task SaveUnidentifiedCypherAsync(UnidentifiedCypher unidentifiedCypher)
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            db.UnidentifiedCyphers.Add(unidentifiedCypher);

            await db.SaveChangesAsync();
        }

        public static async Task<List<UnidentifiedCypher>> GetAllUnidentifiedCyphersAsync()
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            var cyList = await db.UnidentifiedCyphers.ToListAsync();

            return cyList;
        }
    }
}
