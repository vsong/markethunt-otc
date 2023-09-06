using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkethuntOTC.TextProcessing;
public interface ILexer
{
    IEnumerable<Token> Tokenize(string input);
}
