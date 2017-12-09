using System.Threading.Tasks;
using IgorekBot.Data.Models;

namespace IgorekBot.BLL.Services
{
    public interface IBotService
    {
        Task SaveUserProfile(UserProfile profile);
        Task<UserProfile> GetUserProfileByUserId(string userId);
    }
}