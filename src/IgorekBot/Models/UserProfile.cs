using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IgorekBot.Models
{
    [Serializable]
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
        public string EmployeeCode { get; set; }
    }
}