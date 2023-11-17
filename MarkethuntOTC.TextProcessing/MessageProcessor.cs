using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.Infrastructure.DataServices;
using MarkethuntOTC.TextProcessing.Parser;

namespace MarkethuntOTC.TextProcessing;

public class MessageProcessor : IMessageProcessor
{
    private readonly ILexerFactory _lexerFactory;
    private readonly IParser _parser;
    private readonly IDailyValueRepository _dailyValueRepository;

    public MessageProcessor(ILexerFactory lexerFactory, IParser parser, IDailyValueRepository dailyValueRepository)
    {
        _lexerFactory = lexerFactory;
        _parser = parser;
        _dailyValueRepository = dailyValueRepository;
    }

    /// <summary>
    /// Extracts Listings from the given Message
    /// </summary>
    /// <returns>List of successful Listings</returns>
    public IEnumerable<Listing> ExtractListings(Message message)
    {
        return ExtractListings(new[] { message }).First().Listings;
    }

    /// <summary>
    /// Batch version of <see cref="ExtractListings(Message)"/>
    /// </summary>
    public IEnumerable<(Message Message, IEnumerable<Listing> Listings)> ExtractListings(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            var lexer = _lexerFactory.Create(message.OriginatingChannelType);
            var tokens = lexer.Tokenize(message.Text);

            var parseResults = tokens.Select(x => _parser.Parse(message, x)).Where(x => x.Successful);

            yield return (message, parseResults.Select(x => Postprocess(message, x)).Where(x => x != null));
        }
    }

    // HACK throw away parsed trade listings that have "unreasonable" values based on daily MP price
    private Listing Postprocess(Message message, ParseResult parseResult)
    {
        double newSbPrice = parseResult.SbPrice;

        if (parseResult.ListingType == ListingType.Tradeable)
        {
            var dailyValue = _dailyValueRepository.GetLatestDailyValue(parseResult.MatchedParseRule.ItemId, message.CreatedOn)?.SbPrice;
            if (!dailyValue.HasValue) return null;

            if (parseResult.Amount.HasValue)
            {
                var perUnitPrice = parseResult.SbPrice / parseResult.Amount.Value;
                
                if (SbPriceWithinRange(dailyValue.Value, perUnitPrice)) newSbPrice = perUnitPrice;
            }
            
            if (!SbPriceWithinRange(dailyValue.Value, newSbPrice)) return null;
        }

        return new Listing(
            parseResult.MatchedParseRule.ItemId,
            newSbPrice,
            parseResult.ListingType,
            parseResult.IsSelling,
            parseResult.Amount,
            message.Id,
            parseResult.MatchedParseRule.Id,
            message.CreatedOn);
    }

    private static bool SbPriceWithinRange(double expected, double actual)
    {
        return actual <= expected * 1.75 && actual > expected * 0.65;
    }
}