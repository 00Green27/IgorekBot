using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
