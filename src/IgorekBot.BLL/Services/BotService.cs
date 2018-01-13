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
                ctx.UserProfiles.Add(profile);
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

        public async Task HideTask(HiddenTask task)
        {
            using (var ctx = new BotDataContext())
            {
                ctx.UserProfiles.Attach(task.UserProfile);
                ctx.HiddenTasks.Add(task);
                await ctx.SaveChangesAsync();
            }
        }

        public List<HiddenTask> GetUserHiddenTasks(UserProfile profile)
        {
            using (var ctx = new BotDataContext())
            {
                return ctx.HiddenTasks.Where(t => t.UserProfile.Id == profile.Id).ToList();
            }
        }

        public async Task ShowTask(HiddenTask task)
        {
            using (var ctx = new BotDataContext())
            {
                var taskForRemove = ctx.HiddenTasks.FirstOrDefault(t => t.ProjectNo == task.ProjectNo && t.TaskNo == task.TaskNo && t.UserProfile.Id == task.UserProfile.Id);
                if (taskForRemove != null)
                {
                    ctx.HiddenTasks.Remove(taskForRemove);
                    await ctx.SaveChangesAsync();
                }
            }
        }

        public async Task SaveConversationReference(UserProfile profile, string encodedReference)
        {
            using (var ctx = new BotDataContext())
            {
                ctx.UserProfiles.Attach(profile);
                ctx.ConversationReferences.Add(new ConversationReference { UserProfile = profile, EncodedReference = encodedReference});
                await ctx.SaveChangesAsync();
            }
        }

        public async Task RemoveConversationReference(UserProfile profile)
        {
            using (var ctx = new BotDataContext())
            {
                var conversationRef = ctx.ConversationReferences.FirstOrDefault(r => r.UserProfile.Id == profile.Id);
                if (conversationRef != null)
                {
                    ctx.ConversationReferences.Remove(conversationRef);
                    await ctx.SaveChangesAsync();
                }
        }
        }

        public string GetConversationReference(UserProfile profile)
        {
            using (var ctx = new BotDataContext())
            {
                return ctx.ConversationReferences.FirstOrDefault(t => t.Id == profile.Id)?.EncodedReference;
            }
        }

        public List<string> GetConversationReferences()
        {
            using (var ctx = new BotDataContext())
            {
                return ctx.ConversationReferences.Select(r => r.EncodedReference).ToList();
            }
        }
    }
}
