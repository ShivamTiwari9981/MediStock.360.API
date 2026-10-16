namespace MediStock360.Application.Common
{
    public class TelegramSettings
    {
        public string BotToken { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }
}

