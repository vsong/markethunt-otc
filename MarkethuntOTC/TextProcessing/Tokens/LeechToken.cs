using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkethuntOTC.TextProcessing.Tokens;
public class LeechToken : Token
{
    public LeechToken(TokenTransactionType transactionType, bool isSelling, string text) : base(transactionType, isSelling, text)
    {
    }

    public override ParseResults Accept(ITokenVisitor visitor)
    {
        return visitor.Visit(this);
    }
}
