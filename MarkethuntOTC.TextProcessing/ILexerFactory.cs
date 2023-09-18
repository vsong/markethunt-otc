using MarkethuntOTC.Domain.Roots.DiscordMessage;

namespace MarkethuntOTC.TextProcessing;

public interface ILexerFactory
{
    ILexer Create(ChannelType channelType);
}