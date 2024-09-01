using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Parser
{
    public struct GraphParams
    {
        public ExpressionNode start;
        public ExpressionNode end;
        public ExpressionNode step;
        public ExpressionNode x;
        public ExpressionNode y;
        public ExpressionNode angle;
    }
}
