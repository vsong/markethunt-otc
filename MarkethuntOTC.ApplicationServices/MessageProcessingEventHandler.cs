using log4net;
using MarkethuntOTC.Common.Extensions;
using MarkethuntOTC.DataTransferObjects.Events;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.ApplicationServices;

public class MessageProcessingEventHandler : IMessageProcessingEventHandler
{
    private readonly IMessageProcessor _messageProcessor;
    private readonly IDomainContextFactory _domainContextFactory;
    
    private static readonly SemaphoreSlim Lock = new (Environment.ProcessorCount);
    private static readonly ILog Log = LogManager.GetLogger(typeof(MessageProcessingEventHandler));

    public MessageProcessingEventHandler(IMessageProcessor messageProcessor, IDomainContextFactory domainContextFactory)
    {
        _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
        _domainContextFactory = domainContextFactory ?? throw new ArgumentNullException(nameof(domainContextFactory));
    }

    public async Task Handle(MessagesCollectedEvent notification, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        Log.Info($"Processing {notification.MessageIds.Count} messages");
        
        await using var db = _domainContextFactory.Create();

        foreach (var messageIdBatch in notification.MessageIds.Chunk(1000))
        {
            var messages = await db.Messages.AsNoTracking().Where(x => messageIdBatch.Contains(x.Id)).ToListAsync(cancellationToken);

            await ProcessMessages(messages, db, cancellationToken);
        }
        
        Lock.Release();
    }

    public async Task Handle(ReprocessMessagesRequestedEvent notification, CancellationToken cancellationToken)
    {
        const int chunkSize = 1000;
        
        await Lock.WaitAsync(cancellationToken);
        await using var db = _domainContextFactory.Create();
        Log.Info($"Reprocessing all messages of type {notification.ChannelType} with chunk size {chunkSize}");

        var messageIdsToReprocess = await db.Messages
            .AsNoTracking()
            .Where(x => x.OriginatingChannelType == notification.ChannelType)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var numChunks = Math.Ceiling((double)messageIdsToReprocess.Count / chunkSize);

        foreach (var (messageIdBatch, chunkIndex) in messageIdsToReprocess.Chunk(chunkSize).WithIndex())
        {
            Log.Debug($"Reprocessing chunk #{chunkIndex + 1} of {numChunks}");
            var messages = await db.Messages
                .AsNoTracking()
                .Where(x => messageIdBatch.Contains(x.Id))
                .ToListAsync(cancellationToken);

            await db.Listings.Where(x => messageIdBatch.Contains(x.MessageId)).ExecuteDeleteAsync(cancellationToken);
            await ProcessMessages(messages, db, cancellationToken, false);
        }

        Lock.Release();
    }

    private async Task ProcessMessages(IEnumerable<Message> messages, DomainContext db, CancellationToken cancellationToken, bool enableLogging = true)
    {
        foreach (var message in messages)
        {
            if (enableLogging) Log.Debug($"Processing message {message.Id}");
            
            var listings = _messageProcessor
                .ExtractListings(message)
                .Select(x => new Listing(
                    x.MatchedParseRule.ItemId,
                    x.SbPrice,
                    x.ListingType,
                    x.IsSelling,
                    x.Amount,
                    message.Id,
                    x.MatchedParseRule.Id))
                .ToList();

            if (!listings.Any()) continue;
                
            await db.Listings.AddRangeAsync(listings, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}