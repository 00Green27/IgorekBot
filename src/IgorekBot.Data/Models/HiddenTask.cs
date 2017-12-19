using System;

namespace IgorekBot.Data.Models
{
    [Serializable]
    public class HiddenTask
    {
        public HiddenTask()
        {
                
        }

        public HiddenTask(UserProfile profile, string projectNo, string taskNo)
        {
            UserProfile = profile;
            ProjectNo = projectNo;
            TaskNo = taskNo;
        }

        public int Id { get; set; }
        public string TaskNo { get; set; }
        public UserProfile UserProfile { get; set; }
        public string ProjectNo { get; set; }
    }
}
