using System;
using System.ComponentModel.DataAnnotations;

namespace IgorekBot.Data.Models
{
    [Serializable]
    public class Cookie
    {
        public int Id { get; set; }
        [Required]
        public UserProfile User { get; set; }
        public string ResumptionCookie { get; set; }
    }
}
