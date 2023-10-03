using MarkethuntOTC.Domain.Roots.DiscordMessage;

namespace MarkethuntOTC.TextProcessing.Lexer;
public class LexerFactory : ILexerFactory
{
    public ILexer Create(ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.SellMapsChests => new MaptainsSellingFillingLexer(),
            ChannelType.BuySellTradeables => new TradeablesLexer(),
            _ => throw new NotSupportedException()
        };
    }
}