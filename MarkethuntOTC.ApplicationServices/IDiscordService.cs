using Discord;

namespace MarkethuntOTC.ApplicationServices;

public interface IDiscordService
{
    bool IsReady { get; }
    
    Task StopAsync();

    Task<IEnumerable<IMessage>> GetChannelMessagesAsync(
        ulong serverId, 
        ulong channelId, 
        ulong from, 
        TimeSpan excludeNewerThan = default,
        int messageLimit = 5000);
}