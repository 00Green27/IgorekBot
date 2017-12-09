namespace IgorekBot.BLL.Models
{
    public class ValidatePasswordRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string Email { get; set; }

        public string Password { get; set; }

        public string ChannelId { get; set; }
    }
}