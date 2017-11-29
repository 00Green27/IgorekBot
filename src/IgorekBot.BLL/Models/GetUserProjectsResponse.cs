using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class GetUserProjectsResponse
    {
        public int Result { get; set; }

        public root4 XMLPort { get; set; }

        public string ErrorText { get; set; }
    }
}
