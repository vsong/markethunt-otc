using System.Runtime.CompilerServices;

namespace MarkethuntOTC.Common.Extensions;

public static class StringExtensions
{
    public static void ThrowIfNullOrWhitespace(this string s, [CallerArgumentExpression("s")] string sName = null)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException($"{sName} cannot be null, empty, or whitespace");
    }
}