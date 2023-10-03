using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing.Lexer;
public class MaptainsSellingFillingLexer : ILexer
{
    public IEnumerable<Token> Tokenize(string input)
    {
        if (IsBuyingLeech(input))
        {
            yield break;
        }
        
        if (IsSellingLeech(input))
        {
            yield return new LeechToken(true, input);
            yield break;
        }

        while (true)
        {
            var startingLength = input.Length;

            if (UnopenedMapTokenizer.Eat(input, out var unopenedTokenString, out input))
            {
                yield return new UnopenedMapToken(true, unopenedTokenString);
            }
            
            var endingLength = input.Length;

            if (startingLength == endingLength) break;
        }
    }

    private bool IsBuyingLeech(string input)
    {
        return input.Contains("leeching");
    }

    private bool IsSellingLeech(string input)
    {
        var normalizedInput = input.ToLowerInvariant();

        if (normalizedInput.Contains("leech")) return true;

        return false;
    }
    
    private class UnopenedMapTokenizer
    {
        public static bool Eat(string input, out string consumed, out string remaining)
        {
            consumed = null;
            remaining = input;

            var index = input.IndexOf("unopened", StringComparison.InvariantCulture);
            if (index < 0) return false;

            var nextNewLine = input.IndexOf("\n", index, StringComparison.InvariantCulture);
            var consumeTo = nextNewLine > 0 ? nextNewLine : input.Length;

            consumed = input[index..consumeTo];
            remaining = input[consumeTo..];

            return true;
        }
    }
}
