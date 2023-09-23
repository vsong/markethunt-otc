using MarkethuntOTC.Domain.Roots.DiscordMessage;

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

    public IEnumerable<ParseResult> ExtractListings(Message message)
    {
        var lexer = _lexerFactory.Create(message.OriginatingChannelType);
        
        var tokens = lexer.Tokenize(message.Text);

        return tokens.Select(_parser.Parse);
    }
}