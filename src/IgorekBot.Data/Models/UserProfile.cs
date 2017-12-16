using System;
using System.Collections.Generic;

namespace IgorekBot.Data.Models
{
    [Serializable]
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeNo { get; set; }
        public List<HiddenTask> HiddenTasks { get; set; }
    }
}
