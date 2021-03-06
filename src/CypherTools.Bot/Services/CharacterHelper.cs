﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore;
using CypherTools.Core.Models;
using CypherTools.Core.DataAccess.Repos;


namespace CypherTools.Bot.Services
{
    public static class CharacterHelper
    {
        //public static async Task<Character> GetCurrentPlayersCharacterAsync(CommandContext ctx)
        //{
        //    var chr = new Character();// Data.CharacterList.Characters.FirstOrDefault(x => x.Player == ctx.Member.Username + ctx.Member.Discriminator);

        //    if (chr == null)
        //    {
        //        await ctx.RespondAsync("Hey!  you don't have any characters!");
        //    }

        //    return chr;
        //}

        public static async Task<List<Character>> GetCurrentPlayersCharactersAsync(CommandContext ctx)
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            var chars = await db.Characters
                .Include(x => x.Cyphers)
                .Include(x => x.Inventory)
                .Include(x => x.RecoveryRolls)
                .Include(x => x.Pools)
                .Include(x => x.Abilities)
                .Where(x => x.Player == ctx.Member.Username + ctx.Member.Discriminator)
                .ToListAsync();

            return chars;
        }

        public static async Task SaveCurrentCharacterAsync(Character charToSave)
        {
            using var db = new CypherContext(DatabaseHelper.GetDbContextOptions());
            var chr = db.Characters
                .Include(x => x.Cyphers)
                .Include(x => x.Inventory)
                .Include(x => x.RecoveryRolls)
                .Include(x => x.Abilities)
                .Include(x => x.Artifacts)
                .FirstOrDefault(x => x.CharacterId == charToSave.CharacterId);

            if (chr == null)
            {
                db.Characters.Add(charToSave);
            }
            else
            {
                db.Entry(chr).CurrentValues.SetValues(charToSave);

                foreach (var cy in chr.Cyphers)
                {
                    if (!charToSave.Cyphers.Any(x => x.CypherId == cy.CypherId))
                    {
                        db.Remove(cy);
                    }
                }

                foreach (var inv in chr.Inventory)
                {
                    if (!charToSave.Inventory.Any(x => x.InventoryId == inv.InventoryId))
                    {
                        db.Remove(inv);
                    }
                }

                foreach (var roll in chr.RecoveryRolls)
                {
                    if (!charToSave.RecoveryRolls.Any(x => x.RecoveryRollId == roll.RecoveryRollId))
                    {
                        db.Remove(roll);
                    }
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
