namespace MarkethuntOTC.Domain.Roots.DiscordMessage;

public class Message : AggregateRoot<ulong>
{
    private string _text;

    public Message(ulong id, ChannelType originatingChannelType, string text, DateTime createdOn)
    {
        Id = id;
        OriginatingChannelType = originatingChannelType;
        Text = text;
        CreatedOn = createdOn;
    }

    public ChannelType OriginatingChannelType { get; set; }

    public string Text
    {
        get => _text;
        set => _text = value ?? throw new ArgumentNullException(nameof(Text));
    }
    
    public DateTime CreatedOn { get; set; }
}