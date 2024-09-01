using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Parser
{
    public delegate void SetOriginCommandHandler(ExpressionNode posX, ExpressionNode pos_y);
    public delegate void SetScaleCommandHandler(ExpressionNode scaleX, ExpressionNode scaleY);
    public delegate void SetRotCommandHandler(ExpressionNode angle);
    public delegate void DrawLoopCommandHandler(GraphParams drawParams);
}
