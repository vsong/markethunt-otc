using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkethuntOTC.Domain;

namespace MarkethuntOTC.TextProcessing;
public abstract class Token
{
    public TokenTransactionType TransactionType { get; }
    public bool IsSelling { get; }
    public string Text { get; }

    public Token(TokenTransactionType transactionType, bool isSelling, string text)
    {
        TransactionType = transactionType;
        IsSelling = isSelling;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public abstract ParseResults Accept(ITokenVisitor visitor);
}
