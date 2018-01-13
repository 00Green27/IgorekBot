using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.Data.Models
{
    [Serializable]
    public class ConversationReference
    {
        [ForeignKey("UserProfile")]
        public int Id { get; set; }
        public string EncodedReference { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}