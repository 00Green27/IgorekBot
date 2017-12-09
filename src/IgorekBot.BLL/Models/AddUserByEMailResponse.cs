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