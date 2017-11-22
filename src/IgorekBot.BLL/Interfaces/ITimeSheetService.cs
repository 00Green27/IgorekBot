using IgorekBot.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgorekBot.BLL.Interfaces
{
    public interface ITimeSheetService
    {
        GetUserByIdResponse GetUserById(GetUserByIdRequest request);

        AddUserByEMailResponse AddUserByEMail(AddUserByEMailRequest request);
    }
}
