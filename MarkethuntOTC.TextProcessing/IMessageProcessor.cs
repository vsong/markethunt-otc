using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.TextProcessing.Parser;

namespace MarkethuntOTC.TextProcessing;

public interface IMessageProcessor
{
    IEnumerable<Listing> ExtractListings(Message message);
    IEnumerable<(Message Message, IEnumerable<Listing> Listings)> ExtractListings(IEnumerable<Message> messages);
}