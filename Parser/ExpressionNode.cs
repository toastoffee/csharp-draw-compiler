using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.Lexer;

namespace MyCompiler.Parser
{
    public class ExpressionNode
    {
        public TokenType optionType;

        public ExpressionNode operatorLeft, operatorRight;

        public SingleParamFuncHandler singleMathFunc;
        public ExpressionNode funcChild;

        public double constValue;
    }
}
