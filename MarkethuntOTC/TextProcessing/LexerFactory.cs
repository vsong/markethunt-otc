using MarkethuntOTC.TextProcessing.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkethuntOTC.TextProcessing;
public static class LexerFactory
{
    public static ILexer Create(ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.SellMapsChests => new MaptainsSellingFillingLexer(),
            _ => throw new NotSupportedException()
        };
    }
}
