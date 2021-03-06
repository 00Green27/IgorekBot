﻿using System.Collections.Generic;
using NMSService.NMSServiceReference;

namespace IgorekBot.BLL.Models
{
    public class GetUserProjectsResponse
    {
        public int Result { get; set; }

        public IEnumerable<Projects> Projects { get; set; }

        public string ErrorText { get; set; }
    }
}