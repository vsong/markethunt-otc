using Discord;
using Discord.WebSocket;

namespace MarkethuntOTC.ApplicationServices.Discord;

public class DiscordService : IDiscordService
{
    private readonly DiscordSocketClient _client;

    public bool IsReady { get; private set; }
    
    public DiscordService(string token)
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;
        StartAsync(token);
    }
    
    private static Task Log(LogMessage message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
    
    private async Task StartAsync(string token)
    {
        // LoginAsync returns before actual login, so we have to wait for Ready event
        var readyTask = new TaskCompletionSource();
        
        var readyHandler = () =>
        {
            readyTask.SetResult();
            return Task.CompletedTask;
        };

        _client.Ready += readyHandler;
    
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    
        await readyTask.Task;
        _client.Ready -= readyHandler;

        IsReady = true;
    }

    public async Task StopAsync()
    {
        await _client.LogoutAsync();
        IsReady = false;
    }

    public async Task<IEnumerable<IMessage>> GetChannelMessagesAsync(
        ulong serverId,
        ulong channelId,
        ulong from,
        TimeSpan excludeNewerThan = default,
        int messageLimit = 5000)
    {
        var messages = new List<IMessage>();
        var to = DateTime.UtcNow - excludeNewerThan;

        var channel = _client.GetGuild(serverId).GetTextChannel(channelId);

        var messageCursor = from;

        while (true)
        {
            var batchResult = await channel.GetMessagesAsync(messageCursor, Direction.After).FlattenAsync();
            var messageBatch = batchResult.ToList();

            var messagesToAdd = messageBatch.Where(x => x.Id != messageCursor && x.Timestamp.DateTime < to).ToList();
            if (!messagesToAdd.Any()) break;

            messages.AddRange(messagesToAdd);
            messageCursor = messageBatch.Max(x => x.Id);

            if (messages.Count >= messageLimit) break;

            Thread.Sleep(100);
        }

        return messages.DistinctBy(x => x.Id);
    }
}