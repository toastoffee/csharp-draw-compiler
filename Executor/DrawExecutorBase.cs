using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.Semantic;

namespace MyCompiler.Executor
{
    public abstract class DrawExecutorBase
    {
        DrawSemantic semantic;

        public void DrawExecutorInit()
        {
            semantic = new DrawSemantic();
        }

        public void DrawExecutorStart()
        {
            DrawExecutorInit();
            semantic.StartSemantic(@"C:\DrawCompiler\test.txt", @"C:\DrawCompiler\log.txt",DrawCoord);
        }

        protected virtual void DrawCoord(DrawCoordParams coordParams)
        {
            Console.WriteLine("ass");
        }
    }
}
