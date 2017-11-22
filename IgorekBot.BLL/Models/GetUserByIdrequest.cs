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
        public int ChannelType { get; set; }

        public string ChannelId { get; set; }
    }
}
