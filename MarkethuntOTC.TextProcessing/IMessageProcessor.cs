using MarkethuntOTC.Domain.Roots.DiscordMessage;

namespace MarkethuntOTC.TextProcessing;

public interface IMessageProcessor
{
    IEnumerable<ParseResult> ExtractListings(Message message);
}