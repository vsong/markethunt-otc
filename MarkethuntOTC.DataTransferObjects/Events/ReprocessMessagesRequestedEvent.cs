using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MediatR;

namespace MarkethuntOTC.DataTransferObjects.Events;

public class ReprocessMessagesRequestedEvent : Event, INotification
{
    public ReprocessMessagesRequestedEvent(Guid correlationId, ChannelType channelType) : base(correlationId)
    {
        ChannelType = channelType;
    }

    public ChannelType ChannelType { get; }
}