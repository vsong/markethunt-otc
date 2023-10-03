using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MarkethuntOTC.Common.Extensions;

public static class StringExtensions
{
    public static void ThrowIfNullOrWhitespace(this string s, [CallerArgumentExpression("s")] string sName = null)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException($"{sName} cannot be null, empty, or whitespace");
    }

    public static int IndexOf(this string s, Regex regex)
    {
        var m = regex.Match(s);
        return m.Success ? m.Index : -1;
    }
}