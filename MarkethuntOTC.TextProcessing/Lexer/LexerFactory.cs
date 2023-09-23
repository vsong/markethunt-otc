using MarkethuntOTC.Domain.Roots.DiscordMessage;

namespace MarkethuntOTC.TextProcessing.Lexer;
public class LexerFactory : ILexerFactory
{
    public ILexer Create(ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.SellMapsChests => new MaptainsSellingFillingLexer(),
            _ => throw new NotSupportedException()
        };
    }
}
