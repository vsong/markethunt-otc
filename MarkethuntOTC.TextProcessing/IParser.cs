using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing;

public interface IParser
{
    ParseResult Parse(Token token);
}