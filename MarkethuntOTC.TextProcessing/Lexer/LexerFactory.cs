using MarkethuntOTC.TextProcessing.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
