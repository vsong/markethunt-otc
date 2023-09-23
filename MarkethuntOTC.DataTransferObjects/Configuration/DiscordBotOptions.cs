namespace MarkethuntOTC.DataTransferObjects.Configuration;

public class DiscordBotOptions
{
    public string Token { get; private set; }
    public ulong AdminUserId { get; private set; }
    public TimeSpan MessageCollectionInterval { get; private set; }
}