using System.Text.RegularExpressions;

namespace MarkethuntOTC.Common;

public static class MessageContentHelper
{
    private static readonly Regex EmojiRegex = new (@"<(?<emoji>:\w+:)\d+>", RegexOptions.Compiled);
    private static readonly Regex MentionsRegex = new (@"<[@#][^>]+>", RegexOptions.Compiled);

    public static string Sanitize(string content)
    {
        return StripMentions(SanitizeEmojis(content));
    }
    
    private static string SanitizeEmojis(string content)
    {
        return content == null ? null : EmojiRegex.Replace(content, x => x.Groups["emoji"].Value);
    }

    private static string StripMentions(string content)
    {
        return content == null ? null : MentionsRegex.Replace(content, "");
    }
}