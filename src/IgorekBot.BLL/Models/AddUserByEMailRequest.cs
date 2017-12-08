using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class AddUserByEmailRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string Email { get; set; }
    }
}
