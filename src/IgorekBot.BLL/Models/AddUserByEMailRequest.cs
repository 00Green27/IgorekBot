﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class AddUserByEMailRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string EMail { get; set; }
    }
}
