using Discord;
using Discord.WebSocket;
using log4net;

namespace MarkethuntOTC.ApplicationServices.Discord;

public class DiscordService : IDiscordService
{
    private readonly ulong _adminUserId;
    private readonly DiscordSocketClient _client;
    private static readonly ILog Log = LogManager.GetLogger(typeof(DiscordService));

    public bool IsReady { get; private set; }
    
    public DiscordService(string token, ulong adminUserId)
    {
        _adminUserId = adminUserId;
        _client = new DiscordSocketClient();
        _client.Log += OnClientLog;
        _client.MessageReceived += OnMessageReceived;
        _ = StartAsync(token);
    }

    private Task OnClientLog(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Verbose:
                break;
            case LogSeverity.Debug:
                Log.Debug(message);
                break;
            case LogSeverity.Info:
                Log.Info(message);
                break;
            case LogSeverity.Warning:
                Log.Warn(message);
                break;
            case LogSeverity.Error:
                Log.Error(message, message.Exception);
                break;
            case LogSeverity.Critical:
                Log.Fatal(message, message.Exception);
                break;
        }
        
        return Task.CompletedTask;
    }
    
    private async Task StartAsync(string token)
    {
        Log.Info("Starting Discord client");
        // LoginAsync returns before actual login, so we have to wait for Ready event
        var readyTask = new TaskCompletionSource();
        
        var readyHandler = () =>
        {
            Log.Info("Discord client ready");
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
        Log.Info("Stopping Discord client");
        await _client.LogoutAsync();
        IsReady = false;
    }

    public event EventHandler<IEnumerable<string>> CommandReceived;

    public async Task<IEnumerable<IMessage>> GetChannelMessagesAsync(
        ulong serverId,
        ulong channelId,
        ulong from,
        TimeSpan excludeNewerThan = default,
        int messageLimit = 100)
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
    
    private Task OnMessageReceived(SocketMessage message)
    {
        if (message.Author.Id != _adminUserId || !message.Content.StartsWith("otc")) return Task.CompletedTask;

        var args = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
        
        Log.Info("Admin command received: " + message.Content);
        CommandReceived?.Invoke(null, args);

        return Task.CompletedTask;
    }
}