using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.Parser;
using MyCompiler.Lexer;

namespace MyCompiler.Semantic
{
    public class DrawSemantic
    {
        private RDParser parser;

        private Matrix_2x1 originOffset;
        private Matrix_2x2 scaleOffset;
        private Matrix_2x2 rotationOffset;

        private double tParam = 0.0f;

        private DrawCoordExecutionHandler drawCoordExecution;

        private void InitSemantic(DrawCoordExecutionHandler externalDrawCoordExecution)
        {
            parser = new RDParser();

            originOffset = new Matrix_2x1(0f, 0f);
            scaleOffset = Matrix_2x2.CreateScaleOffsetMatrix(1f, 1f);
            rotationOffset = Matrix_2x2.CreateRotationOffsetMatrix(0f);

            drawCoordExecution = externalDrawCoordExecution;

        }

        public void StartSemantic(string filePath, string logPath,DrawCoordExecutionHandler externalDrawCoordExecution)
        {
            InitSemantic(externalDrawCoordExecution);
            parser.StartParser(filePath, logPath, SetOrigin, SetRot, SetScale, DrawLoop);
        }


        private void SetOrigin(ExpressionNode x, ExpressionNode y)
        {
            originOffset = new Matrix_2x1(CalcExpression(x), CalcExpression(y));
        }

        private void SetScale(ExpressionNode scaleX, ExpressionNode scaleY)
        {
            scaleOffset = Matrix_2x2.CreateScaleOffsetMatrix(CalcExpression(scaleX), CalcExpression(scaleY));
        }

        private void SetRot(ExpressionNode angle)
        {
            rotationOffset = Matrix_2x2.CreateRotationOffsetMatrix(CalcExpression(angle));
        }

        private void DrawLoop(GraphParams drawParams)
        {
            double start = CalcExpression(drawParams.start);
            double end = CalcExpression(drawParams.end);
            double step = CalcExpression(drawParams.step);

            DrawCoordParams coordParams = new();

            for(double t = start; t <= end;t += step)
            {
                tParam = t;

                double x = CalcExpression(drawParams.x);
                double y = CalcExpression(drawParams.y);

                Matrix_2x1 point = new Matrix_2x1(x, y);
                point = ApplyTransformation(point);

                // 将计算出的点加入到coordParams.points中
                coordParams.points.Add(point);
            }

            // 调用委托来绘制一系列的图像
            drawCoordExecution?.Invoke(coordParams);
        }

        private Matrix_2x1 ApplyTransformation(Matrix_2x1 point)
        {
            point = scaleOffset * point;
            point = rotationOffset * point;
            point = originOffset + point;
            return point;
        }

        private void TestFunc(double x,double y)
        {
            Console.WriteLine($"Point:({x},{y})");
        }

        private double CalcExpression(ExpressionNode root)
        {
            if (root == null) return 0.0f;

            switch(root.optionType)
            {
                case TokenType.CONST_ID:
                    {
                        return root.constValue;
                    }
                case TokenType.T:
                    {
                        return tParam;
                    }
                case TokenType.PLUS:
                    {
                        return CalcExpression(root.operatorLeft) + CalcExpression(root.operatorRight);
                    }
                case TokenType.MINUS:
                    {
                        return CalcExpression(root.operatorLeft) - CalcExpression(root.operatorRight);
                    }
                case TokenType.MUL:
                    {
                        return CalcExpression(root.operatorLeft) * CalcExpression(root.operatorRight);
                    }
                case TokenType.DIV:
                    {
                        return CalcExpression(root.operatorLeft) / CalcExpression(root.operatorRight);
                    }
                case TokenType.POWER:
                    {
                        return Math.Pow(CalcExpression(root.operatorLeft),CalcExpression(root.operatorRight));
                    }
                case TokenType.FUNC:
                    {
                        return root.singleMathFunc.Invoke(CalcExpression(root.funcChild));
                    }
                default:
                    return 0.0f;
            }
        }
    }
}
