using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.TextProcessing.Parser;

namespace MarkethuntOTC.TextProcessing;

public class MessageProcessor : IMessageProcessor
{
    private readonly ILexerFactory _lexerFactory;
    private readonly IParser _parser;

    public MessageProcessor(ILexerFactory lexerFactory, IParser parser)
    {
        _lexerFactory = lexerFactory;
        _parser = parser;
    }

    /// <summary>
    /// Tokenize and parse supplied message and return successful parse results
    /// </summary>
    /// <returns>List of successful ParseResults</returns>
    public IEnumerable<ParseResult> ExtractListings(Message message)
    {
        return ExtractListings(new[] { message }).First().Results;
    }

    /// <summary>
    /// Batch version of <see cref="ExtractListings(Message)"/>
    /// </summary>
    public IEnumerable<(Message Message, IEnumerable<ParseResult> Results)> ExtractListings(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            var lexer = _lexerFactory.Create(message.OriginatingChannelType);
        
            var tokens = lexer.Tokenize(message.Text);

            yield return (message, tokens.Select(_parser.Parse).Where(x => x.Successful));
        }
    }
}