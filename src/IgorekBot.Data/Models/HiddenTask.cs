using System;

namespace IgorekBot.Data.Models
{
    [Serializable]
    public class HiddenTask
    {
        public HiddenTask()
        {
                
        }

        public HiddenTask(UserProfile profile, string taskNo)
        {
            UserProfile = profile;
            TaskNo = taskNo;
        }

        public int Id { get; set; }
        public string TaskNo { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
