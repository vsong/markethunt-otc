using MarkethuntOTC.DataTransferObjects.Events;
using MediatR;

namespace MarkethuntOTC.ApplicationServices;

public interface IMessageProcessingEventHandler : INotificationHandler<MessagesCollectedEvent>, INotificationHandler<ReprocessMessagesRequestedEvent>
{
}