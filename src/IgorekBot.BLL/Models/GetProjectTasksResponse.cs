﻿using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class GetProjectTasksResponse
    {
        public int Result { get; set; }

        public root5 XMLPort { get; set; }

        public string ErrorText { get; set; }
    }
}