using System.Timers;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace MarkethuntOTC.ApplicationServices;

public class MessageCollectionService : IMessageCollectionService
{
    private readonly IDiscordService _discordService;
    private readonly IDomainContextFactory _domainContextFactory;
    private readonly Timer _messageCollectionTimer;
    private readonly TimeSpan _ignoreMessagesNewerThan = TimeSpan.FromSeconds(30);

    public MessageCollectionService(IDiscordService discordService, TimeSpan messageCollectionInterval, IDomainContextFactory domainContextFactory)
    {
        _discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
        _domainContextFactory = domainContextFactory ?? throw new ArgumentNullException(nameof(domainContextFactory));

        _messageCollectionTimer = new Timer(messageCollectionInterval);
        _messageCollectionTimer.Elapsed += CollectMessages;
        _messageCollectionTimer.Enabled = true;                 
    }

    private async void CollectMessages(object o, ElapsedEventArgs e)
    {
        _messageCollectionTimer.Enabled = false;

        await using var db = _domainContextFactory.Create();

        foreach (var channelState in await db.ChannelStates.ToListAsync())
        {
            var messages = (await _discordService.GetChannelMessagesAsync(
                channelState.ServerId,
                channelState.ChannelId,
                channelState.LatestMessageId,
                _ignoreMessagesNewerThan))
                .ToList();

            var messageIds = messages.Select(x => x.Id);
            var existingMessageIds = await db.Messages
                .Where(x => messageIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
            var newMessagesToInsert =
                (from m in messages
                where !existingMessageIds.Contains(m.Id) 
                select new Message(m.Id, channelState.ChannelType, channelState.ChannelId, m.CleanContent, m.Timestamp.DateTime))
                .ToList();

            if (newMessagesToInsert.Any())
            {
                channelState.LatestMessageId = newMessagesToInsert.Max(x => x.Id);
                await db.Messages.AddRangeAsync(newMessagesToInsert);
            }

            await db.SaveChangesAsync();
            
            Console.WriteLine($"Inserted {newMessagesToInsert.Count} new messages");
        }
        
        _messageCollectionTimer.Enabled = true;
    }
}