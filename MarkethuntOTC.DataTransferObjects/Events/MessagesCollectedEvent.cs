using MediatR;

namespace MarkethuntOTC.DataTransferObjects.Events;

// TODO instead of using these events, implement the repository layer and emit domain events from there
public class MessagesCollectedEvent : Event, INotification
{
    public MessagesCollectedEvent(Guid correlationId, IReadOnlyCollection<ulong> messageIds) : base(correlationId)
    {
        MessageIds = messageIds ?? throw new ArgumentNullException(nameof(messageIds));
    }

    public IReadOnlyCollection<ulong> MessageIds { get; }
}