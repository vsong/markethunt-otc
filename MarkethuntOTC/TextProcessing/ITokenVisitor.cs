using MarkethuntOTC.Domain;
using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing;

public interface ITokenVisitor
{
    ParseResults Visit(LeechToken token);
    ParseResults Visit(FreshMapToken token);
}