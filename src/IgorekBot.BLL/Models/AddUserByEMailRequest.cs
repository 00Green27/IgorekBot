namespace IgorekBot.BLL.Models
{
    public class AddUserByEmailRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string Email { get; set; }
    }
}