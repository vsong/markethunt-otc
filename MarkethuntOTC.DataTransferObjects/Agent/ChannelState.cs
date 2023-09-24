using MarkethuntOTC.Domain.Roots.DiscordMessage;

namespace MarkethuntOTC.DataTransferObjects.Agent;

public class ChannelState
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ChannelType ChannelType { get; }
    public ulong StartingFromMessageId { get; }
    public ulong? LatestMessageId { get; set; }
}