using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    public interface IDFA
    {
        int GetStartState();

        int Move(int startSrc,uint chr);

        TokenType IsStateFinal(int state);
    }
}
