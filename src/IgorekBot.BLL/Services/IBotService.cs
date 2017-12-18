using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IgorekBot.Data.Models;

namespace IgorekBot.BLL.Services
{
    public interface IBotService
    {
        Task SaveUserProfile(UserProfile profile);
        Task<UserProfile> GetUserProfileByUserId(string userId);
        Task HideTask(HiddenTask hiddenTask);
        List<HiddenTask> GetUserHiddenTasks(UserProfile profile);
        Task ShowTask(HiddenTask hiddenTask);
    }
}