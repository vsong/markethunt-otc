using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing;
public interface ILexer
{
    IEnumerable<Token> Tokenize(string input);
}
