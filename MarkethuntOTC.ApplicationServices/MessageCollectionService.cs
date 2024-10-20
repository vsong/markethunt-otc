using System.Timers;
using Discord;
using Discord.Net;
using log4net;
using MarkethuntOTC.DataTransferObjects.Events;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChannelType = MarkethuntOTC.Domain.Roots.DiscordMessage.ChannelType;
using Timer = System.Timers.Timer;

namespace MarkethuntOTC.ApplicationServices;

// TODO: can be made into transient event handler
public class MessageCollectionService : IMessageCollectionService
{
    private readonly IDiscordService _discordService;
    private readonly IDomainContextFactory _domainContextFactory;
    private readonly IMediator _messageBus;
    private readonly Timer _messageCollectionTimer;
    private readonly TimeSpan _ignoreMessagesNewerThan = TimeSpan.FromSeconds(30);
    
    private static readonly ILog Log = LogManager.GetLogger(typeof(MessageCollectionService));

    private const int MaxMessageCollectionSize = 1000;

    public MessageCollectionService(
        IDiscordService discordService, 
        TimeSpan messageCollectionInterval, 
        IDomainContextFactory domainContextFactory, 
        IMediator messageBus)
    {
        _discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
        _domainContextFactory = domainContextFactory ?? throw new ArgumentNullException(nameof(domainContextFactory));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

        _messageCollectionTimer = new Timer(messageCollectionInterval);
        _messageCollectionTimer.Elapsed += CollectMessages;
        _messageCollectionTimer.Enabled = true;                 
    }

    private async void CollectMessages(object o, ElapsedEventArgs e)
    {
        _messageCollectionTimer.Enabled = false;
        Log.Info("Begin message collection");

        try
        {
            await using var db = _domainContextFactory.Create();

            foreach (var channelState in await db.ChannelStates.ToListAsync())
            {
                Log.Info($"Collect messages from channel {channelState.ChannelId}, latest message: {channelState.LatestMessageId}");

                IList<IMessage> messages;

                try
                {
                    messages = (await _discordService.GetChannelMessagesAsync(
                            channelState.ServerId,
                            channelState.ChannelId,
                            channelState.LatestMessageId ?? channelState.StartingFromMessageId,
                            _ignoreMessagesNewerThan,
                            MaxMessageCollectionSize))
                        .ToList();
                }
                catch (Exception ex) when (ex is HttpException or HttpRequestException or TimeoutException)
                {
                    Log.Error("Failed to fetch messages", ex);
                    continue;
                }

                var messageIds = messages.Select(x => x.Id);
                var existingMessageIds = await db.Messages
                    .Where(x => messageIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync();
                var newMessagesToInsert =
                    (from m in messages
                        where !existingMessageIds.Contains(m.Id) && !m.Author.IsBot
                        select Message.Create(m.Id, channelState.ChannelType, channelState.ChannelId, m.Content,
                            m.Timestamp.DateTime))
                    .ToList();
                Log.Info($"Collected {newMessagesToInsert.Count} new messages");

                if (newMessagesToInsert.Any())
                {
                    channelState.LatestMessageId = newMessagesToInsert.Max(x => x.Id);
                    await db.Messages.AddRangeAsync(newMessagesToInsert);
                }

                await db.SaveChangesAsync();

                if (channelState.ChannelType == ChannelType.BuySellSnipes) continue;

                if (newMessagesToInsert.Any())
                {
                    await _messageBus.Publish(new MessagesCollectedEvent(
                        Guid.NewGuid(),
                        newMessagesToInsert.Select(x => x.Id).ToList()));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error occured during message collection", ex);
        }
        finally
        {
            Log.Info("Finished message collection");
            _messageCollectionTimer.Enabled = true;
        }
    }
}