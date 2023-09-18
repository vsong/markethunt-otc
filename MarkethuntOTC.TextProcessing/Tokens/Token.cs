namespace MarkethuntOTC.TextProcessing.Tokens;
public abstract class Token
{
    public bool IsSelling { get; }
    public string Text { get; }

    public Token(bool isSelling, string text)
    {
        IsSelling = isSelling;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }
}
