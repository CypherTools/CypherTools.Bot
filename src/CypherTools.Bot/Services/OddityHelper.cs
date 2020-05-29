using CypherTools.Core.DataAccess.Repos;
using CypherTools.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherTools.Bot.Services
{
    public static class OddityHelper
    {
        public static async Task<List<Oddity>> GetAllOdditysAsync()
        {
            using (var db = new CypherContext(DatabaseHelper.GetDbContextOptions()))
            {
                var oddList = await db.Oddities.ToListAsync();

                return oddList;
            }
        }

        public static async Task<Oddity> GetRandomOddityAsync()
        {
            var oddList = await GetAllOdditysAsync();

            var i = RandomGenerator.GetRandom().Next(0, oddList.Count() - 1);

            return oddList[i];
        }

        public static async Task<List<Oddity>> GetRandomOddityAsync(int numberOfCyphers)
        {
            var ls = new List<Oddity>();
            var rnd = RandomGenerator.GetRandom();

            for (int i = 0; i < numberOfCyphers; i++)
            {
                var OddityList = await GetAllOdditysAsync();

                ls.Add(OddityList[rnd.Next(1, OddityList.Count)]);
            }

            return ls;
        }
    }
}
