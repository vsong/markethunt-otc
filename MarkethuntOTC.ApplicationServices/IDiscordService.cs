using Discord;

namespace MarkethuntOTC.ApplicationServices;

public interface IDiscordService
{
    bool IsReady { get; }
    
    Task StopAsync();

    event EventHandler<IEnumerable<string>> CommandReceived;

    Task<IEnumerable<IMessage>> GetChannelMessagesAsync(
        ulong serverId, 
        ulong channelId, 
        ulong from, 
        TimeSpan excludeNewerThan = default,
        int messageLimit = 100);
}