using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Models
{
    public class AddUserByEMailResponse
    {
        public int Result { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ErrorText { get; set; }
    }
}
