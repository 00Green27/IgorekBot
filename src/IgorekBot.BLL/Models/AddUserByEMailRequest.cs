using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class AddUserByEMailRequest
    {
        public int ChannelType { get; set; }

        public string EMail { get; set; }
    }
}
