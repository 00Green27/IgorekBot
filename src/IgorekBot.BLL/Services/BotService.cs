using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IgorekBot.Data;
using IgorekBot.Data.Models;

namespace IgorekBot.BLL.Services
{
    public class BotService : IBotService
    {
        public async Task SaveUserProfile(UserProfile profile)
        {
            using (var ctx = new BotDataContext())
            {
                ctx.UserProfiles.AddOrUpdate(profile);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<UserProfile> GetUserProfileByUserId(string userId)
        {
            using (var ctx = new BotDataContext())
            {
                return await ctx.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
            }
        }
    }
}
