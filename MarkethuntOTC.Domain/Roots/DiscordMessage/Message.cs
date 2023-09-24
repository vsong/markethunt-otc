using MarkethuntOTC.Common;

namespace MarkethuntOTC.Domain.Roots.DiscordMessage;

public class Message : AggregateRoot<ulong>
{
    private string _text;

    private Message (ulong id, ChannelType originatingChannelType, ulong? originationChannelId, string text, DateTime createdOn)
    {
        Id = id;
        OriginatingChannelType = originatingChannelType;
        OriginationChannelId = originationChannelId;
        Text = text;
        CreatedOn = createdOn;
    }
    
    public static Message Create(ulong id, ChannelType originatingChannelType, ulong? originationChannelId, string text, DateTime createdOn)
    {
        var message = new Message(id, originatingChannelType, originationChannelId, text, createdOn);
        message.SetText(text);
        
        return message;
    }

    public ChannelType OriginatingChannelType { get; set; }
    public ulong? OriginationChannelId { get; private set; }

    public string Text
    {
        get => _text;
        private set => _text = value ?? throw new ArgumentNullException(nameof(Text));
    }
    
    public DateTime CreatedOn { get; private set; }

    public void SetText(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Text = MessageContentHelper.Sanitize(value).ToLowerInvariant();
    }
}