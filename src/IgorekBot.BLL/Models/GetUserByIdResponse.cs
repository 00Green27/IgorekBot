using NMSService.NMSServiceReference;

namespace IgorekBot.BLL.Models
{
    public class GetUserByIdResponse
    {
        public int Result { get; set; }

        public Employee1 Employee { get; set; }

        public string ErrorText { get; set; }
    }
}