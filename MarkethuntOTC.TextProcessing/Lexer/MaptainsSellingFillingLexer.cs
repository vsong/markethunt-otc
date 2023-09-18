using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing.Lexer;
public class MaptainsSellingFillingLexer : ILexer
{
    public IEnumerable<Token> Tokenize(string input)
    {
        if (IsSellingLeech(input))
        {
            yield return new LeechToken(true, input);
            yield break;
        }

        while (true)
        {
            var startingLength = input.Length;

            if (FreshMapTokenizer.Eat(input, out var tokenString, out input))
            {
                yield return new FreshMapToken(true, tokenString);
            }

            var endingLength = input.Length;

            if (startingLength == endingLength) break;
        }

        yield break;
    }

    private bool IsSellingLeech(string input)
    {
        var normalizedInput = input.ToLowerInvariant();

        if (normalizedInput.Contains("leech")) return true;

        return false;
    }

    private class FreshMapTokenizer
    {
        public static bool Eat(string input, out string? consumed, out string remaining)
        {
            consumed = null;
            remaining = input;

            var normalizedInput = input.ToLowerInvariant();

            var index = normalizedInput.IndexOf("fresh", StringComparison.InvariantCulture);
            if (index < 0) return false;

            var nextNewLine = normalizedInput.IndexOf("\n", StringComparison.InvariantCulture);
            var consumeTo = nextNewLine > 0 ? nextNewLine : normalizedInput.Length;

            consumed = normalizedInput[index..consumeTo];
            remaining = normalizedInput.Substring(consumeTo);

            return true;
        }
    }
}
