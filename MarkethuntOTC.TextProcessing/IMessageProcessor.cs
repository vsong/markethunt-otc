using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.TextProcessing.Parser;

namespace MarkethuntOTC.TextProcessing;

public interface IMessageProcessor
{
    IEnumerable<ParseResult> ExtractListings(Message message);
    IEnumerable<(Message Message, IEnumerable<ParseResult> Results)> ExtractListings(IEnumerable<Message> messages);
}