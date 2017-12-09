using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IgorekBot.Data.Models;

namespace IgorekBot.Data
{
    public class BotDataContext : DbContext
    {
        public BotDataContext()
            : base("BotDataContextConnectionString")
        {
        }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
