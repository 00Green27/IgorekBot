using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class GetUserByIdRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string ChannelId { get; set; }
    }
}
