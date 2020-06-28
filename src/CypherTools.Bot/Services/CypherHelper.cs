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

            var i = new Random().Next(0, cyList.Count());

            return cyList[i];
        }

        public static async Task<List<Cypher>> GetRandomCyphersAsync(int numberOfCyphers)
        {
            var ls = new List<Cypher>();
            var rnd = RandomGenerator.GetRandom();

            for (int i = 0; i < numberOfCyphers; i++)
            {
                var cypherList = await GetAllCyphersAsync();
                ls.Add(cypherList[rnd.Next(0, cypherList.Count)]);
            }

            return ls;
        }

        public static async Task<CharacterCypher> GetRandomCharacterCypherAsync()
        {
            var cypher = await GetRandomCypherAsync();

            var charCypher = new CharacterCypher();

            var rnd = RandomGenerator.GetRandom();

            charCypher.CypherId = cypher.CypherId;
            charCypher.Effect = cypher.Effect;
            charCypher.LevelBonus = cypher.LevelBonus;
            charCypher.LevelDie = cypher.LevelDie;
            charCypher.Level = cypher.Level;
            charCypher.Name = cypher.Name;
            charCypher.Source = cypher.Source;
            charCypher.Type = cypher.Type;
            charCypher.Form = "";

            if (cypher.Forms.Count > 0)
            {
                var cf = cypher.Forms.ToList()[rnd.Next(0, cypher.Forms.Count())];

                charCypher.Form = cf.Form + " - " + cf.FormDescription;
            }

            return charCypher;
        }

        public static async Task<List<CharacterCypher>> GetRandomCharacterCyphersAsync(int numberOfCyphers)
        {
            var cyList = new List<CharacterCypher>();

            for (int i = 0; i < numberOfCyphers; i++)
            {
                cyList.Add(await GetRandomCharacterCypherAsync());
            }

            return cyList;
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
