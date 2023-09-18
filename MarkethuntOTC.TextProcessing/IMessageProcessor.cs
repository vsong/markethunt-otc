using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Domain.Roots.Listing;

namespace MarkethuntOTC.TextProcessing;

public interface IMessageProcessor
{
    IEnumerable<ParseResult> ExtractListings(Message message);
}