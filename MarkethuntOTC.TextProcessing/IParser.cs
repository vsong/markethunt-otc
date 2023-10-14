using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.TextProcessing.Parser;
using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing;

public interface IParser
{
    ParseResult Parse(Message message, Token token);
}