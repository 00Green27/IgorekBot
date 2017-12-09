namespace IgorekBot.BLL.Models
{
    public class GetUserByIdRequest
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Telegram;

        public string ChannelId { get; set; }
    }
}