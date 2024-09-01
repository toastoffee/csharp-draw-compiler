using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.Lexer;

namespace MyCompiler.Parser
{
    public class RDParser
    {
        private Token currentToken;
        private IScanner scanner;
        private ErrorLog errorLog;

        private int indentLen = 0;

        private SetOriginCommandHandler setOriginCommand;
        private SetRotCommandHandler setRotCommand;
        private SetScaleCommandHandler setScaleCommand;
        private DrawLoopCommandHandler drawLoopCommand;

        // 初始化语法分析器，成功时返回true
        private bool InitParser(string filePath,string logPath,
            SetOriginCommandHandler externalSetOriginCommand,
            SetRotCommandHandler externalSetRotCommand,
            SetScaleCommandHandler externalSetScaleCommand,
            DrawLoopCommandHandler externalDrawLoopCommand)
        {
            indentLen = 0;

            setOriginCommand = externalSetOriginCommand;
            setRotCommand = externalSetRotCommand;
            setScaleCommand = externalSetScaleCommand;
            drawLoopCommand = externalDrawLoopCommand;

            errorLog = new ErrorLog();
            errorLog.InitErrorLog(logPath);

            scanner = new Scanner();
            return scanner.InitScanner(filePath);

        }

        public void StartParser(string filePath, string logPath,
            SetOriginCommandHandler externalSetOriginCommand,
            SetRotCommandHandler externalSetRotCommand,
            SetScaleCommandHandler externalSetScaleCommand,
            DrawLoopCommandHandler externalDrawLoopCommand)
        {
            if(!InitParser(filePath,logPath,externalSetOriginCommand,externalSetRotCommand,externalSetScaleCommand,externalDrawLoopCommand))
            {
                return;
            }

            // 获取第一个记号
            FetchToken();
            // 递归下降分析
            Program();

            CloseParser();
        }
        

        // 关闭语法分析器
        public void CloseParser()
        {
            scanner.CloseScanner();
            errorLog.CloseErrorLog();
        }

        // 获取记号
        private void FetchToken()
        {
            currentToken = scanner.GetToken();
            if(currentToken.type == TokenType.ERRTOKEN)
            {
                errorLog.LogErrorMsg(SyntaxError.IlleagalToken,currentToken);
            }
        }

        // 匹配记号
        private void MatchToken(TokenType expectedTokenType)
        {
            if(currentToken.type != expectedTokenType)
            {
                errorLog.LogErrorMsg(SyntaxError.NonExpectedToken, currentToken);
            }
            else
            {
                // 匹配上了记号则打印记号文本 接着读取下一记号
                for (int i = 0; i < indentLen; i++) Console.Write(" ");
                Console.WriteLine($"Match Token: {currentToken.lexeme}");

                FetchToken();
            }
        }

        private void Program()
        {
            Enter("Program");
            while(currentToken.type != TokenType.NONTOKEN)
            {
                Statement();
                MatchToken(TokenType.SEMICO);
            }    

            Back("Program");
        }

        private void Statement()
        {
            Enter("Statement");

            switch(currentToken.type)
            {
                case TokenType.ORIGIN: OriginStatement();break;
                case TokenType.SCALE:ScaleStatement();break;
                case TokenType.ROT:RotStatement();break;
                case TokenType.FOR:ForStatement();break;
                default:errorLog.LogErrorMsg(SyntaxError.NonExpectedToken, currentToken);break;
            }

            Back("Statement");
        }



        private void OriginStatement()
        {
            GraphParams forParams = new();
            Enter("Origin Statement");

            MatchToken(TokenType.ORIGIN);
            MatchToken(TokenType.IS);
            MatchToken(TokenType.L_BRACKET);

            forParams.x = Expression();
            TreeTrace(forParams.x);

            MatchToken(TokenType.COMMA);

            forParams.y = Expression();
            TreeTrace(forParams.y);

            MatchToken(TokenType.R_BRACKET);

            Back("Origin Statement");
            SetOrigin(forParams.x, forParams.y);
        }

        private void ScaleStatement()
        {
            GraphParams forParams = new();
            Enter("Scale Statement");

            MatchToken(TokenType.SCALE);
            MatchToken(TokenType.IS);
            MatchToken(TokenType.L_BRACKET);

            forParams.x = Expression();
            TreeTrace(forParams.x);

            MatchToken(TokenType.COMMA);

            forParams.y = Expression();
            TreeTrace(forParams.y);

            MatchToken(TokenType.R_BRACKET);

            Back("Scale Statement");
            SetScale(forParams.x, forParams.y);
        }

        private void RotStatement()
        {
            GraphParams forParams = new();
            Enter("Rot Statement");

            MatchToken(TokenType.ROT);
            MatchToken(TokenType.IS);

            forParams.angle = Expression();
            TreeTrace(forParams.angle);

            Back("Rot Statement");
            SetRot(forParams.angle);
        }


        private void ForStatement()
        {
            GraphParams forParams = new();
            Enter("for statement");

            MatchToken(TokenType.FOR);
            MatchToken(TokenType.T);
            MatchToken(TokenType.FROM);
            
            forParams.start = Expression();
            TreeTrace(forParams.start);

            MatchToken(TokenType.TO);

            forParams.end = Expression();
            TreeTrace(forParams.end);

            MatchToken(TokenType.STEP);

            forParams.step = Expression();
            TreeTrace(forParams.step);

            MatchToken(TokenType.DRAW);
            MatchToken(TokenType.L_BRACKET);
            
            forParams.x = Expression();
            TreeTrace(forParams.x);

            MatchToken(TokenType.COMMA);

            forParams.y = Expression();
            TreeTrace(forParams.y);

            MatchToken(TokenType.R_BRACKET);

            Back("For Statement");

            DrawLoop(forParams);
        }


        private ExpressionNode Expression()
        {
            ExpressionNode leftNode = default;
            ExpressionNode rightNode = default;
            TokenType lastType;

            Enter("expression");
            leftNode = Term();

            while(currentToken.type == TokenType.PLUS || currentToken.type == TokenType.MINUS)
            {
                lastType = currentToken.type;
                MatchToken(lastType);
                rightNode = Term();
                leftNode = MakeBinaryOperatorExpressionNode(lastType, leftNode, rightNode);
            }
            Back("expression");
            return leftNode;
        }


        private ExpressionNode Term()
        {
            ExpressionNode leftNode = default;
            ExpressionNode rightNode = default;
            TokenType lastType;

            leftNode = Factor();

            while(currentToken.type == TokenType.MUL || currentToken.type == TokenType.DIV)
            {
                lastType = currentToken.type;
                MatchToken(lastType);
                rightNode = Factor();
                leftNode = MakeBinaryOperatorExpressionNode(lastType, leftNode, rightNode);
            }

            return leftNode;
        }

        private ExpressionNode Factor()
        {
            ExpressionNode leftNode = default;
            ExpressionNode rightNode = default;

            if(currentToken.type == TokenType.PLUS)
            {
                MatchToken(TokenType.PLUS);
                rightNode = Factor();
            }
            else if(currentToken.type == TokenType.MINUS)
            {
                MatchToken(TokenType.MINUS);
                rightNode = Factor();
                leftNode = new ExpressionNode()
                {
                    optionType = TokenType.CONST_ID,
                    constValue = 0.0f,
                };
                rightNode = MakeBinaryOperatorExpressionNode(TokenType.MINUS, leftNode, rightNode);
            }
            else
            {
                rightNode = Component();
            }
            return rightNode;
        }


        private ExpressionNode Component()
        {
            ExpressionNode leftNode = default;
            ExpressionNode rightNode = default;

            leftNode = Atom();

            if(currentToken.type == TokenType.POWER)
            {
                MatchToken(TokenType.POWER);
                rightNode = Component();
                leftNode = MakeBinaryOperatorExpressionNode(TokenType.POWER, leftNode, rightNode);
            }

            return leftNode;
        }

        private ExpressionNode Atom()
        {
            ExpressionNode selfNode = default;
            ExpressionNode childNode = default;

            Token t = DeepCopyToken(currentToken);

            switch (currentToken.type)
            {
                case TokenType.CONST_ID:
                    {
                        MatchToken(TokenType.CONST_ID);
                        selfNode = MakeConstValueExpressionNode(t.constValue);
                        break;
                    }
                case TokenType.T:
                    {
                        MatchToken(TokenType.T);
                        selfNode = MakeParamTExpressionNode();
                        break;
                    }
                case TokenType.FUNC:
                    {
                        MatchToken(TokenType.FUNC);
                        MatchToken(TokenType.L_BRACKET);
                        childNode = Expression();
                        selfNode = MakeSingleFuncExpressionNode(t.singleParamFunc, childNode);
                        MatchToken(TokenType.R_BRACKET);
                        break;
                    }
                case TokenType.L_BRACKET:
                    {
                        MatchToken(TokenType.L_BRACKET);
                        selfNode = Expression();
                        MatchToken(TokenType.R_BRACKET);
                        break;
                    }
                default:
                    errorLog.LogErrorMsg(SyntaxError.NonExpectedToken, currentToken);
                    break;
            }
            return selfNode;
        }


        #region  Make Expression Node
        // 生成语法树中表达式Expression节点

        // 生成常数节点
        private ExpressionNode MakeConstValueExpressionNode(double value)
        {
            ExpressionNode expr = new()
            {
                optionType = TokenType.CONST_ID,
                constValue = value,
            };
            return expr;
        }

        // 生成参数节点
        private ExpressionNode MakeParamTExpressionNode()
        {
            ExpressionNode expr = new()
            {
                optionType = TokenType.T,
            };
            return expr;
        }

        // 生成函数调用节点
        private ExpressionNode MakeSingleFuncExpressionNode(SingleParamFuncHandler func,ExpressionNode childNode)
        {
            ExpressionNode expr = new()
            {
                optionType = TokenType.FUNC,
                singleMathFunc = func,
                funcChild = childNode,
            };
            return expr;
        }

        // 生成二元运算节点
        private ExpressionNode MakeBinaryOperatorExpressionNode(TokenType type,ExpressionNode left,ExpressionNode right)
        {
            ExpressionNode expr = new()
            {
                optionType = type,
                operatorLeft = left,
                operatorRight = right
            };
            return expr;
        }

        #endregion

        #region log

        private void Enter(string x)
        {
            for (int i = 0; i < indentLen; i++) Console.Write(" ");
            Console.WriteLine($"enter in {x}");
            indentLen += 2;
        }

        private void Back(string x)
        {
            indentLen -= 2;
            for (int i = 0; i < indentLen; i++) Console.Write(" ");
            Console.WriteLine($"exit from {x}");
        }

        private void TreeTrace(ExpressionNode x)
        {
            int L = indentLen;
            for (; L > 0; --L) Console.Write(" ");
            Console.WriteLine("TREE:");

            PrintSyntaxTree(x, 0);
        }

        private void PrintSyntaxTree(ExpressionNode root,int indent)
        {
            int L;

            if (root == null) return;

            L = indent + indentLen;

            for (; L > 0; --L) Console.Write(" ");

            switch(root.optionType)
            {
                case TokenType.PLUS:
                    Console.WriteLine("+"); break;
                case TokenType.MINUS:
                    Console.WriteLine("+"); break;
                case TokenType.MUL:
                    Console.WriteLine("+"); break;
                case TokenType.DIV:
                    Console.WriteLine("+"); break;
                case TokenType.POWER:
                    Console.WriteLine("+"); break;
                case TokenType.T:
                    Console.WriteLine("+");
                    return;
                case TokenType.FUNC:
                    Console.WriteLine($"{root.singleMathFunc}");
                    PrintSyntaxTree(root.funcChild, indent + 2);
                    return;
                case TokenType.CONST_ID:
                    Console.WriteLine($"{root.constValue}");
                    return;
                default:
                    Console.WriteLine("非法的树节点");
                    return;
            }
        }

        #endregion

        private void SetOrigin(ExpressionNode x, ExpressionNode y)
        {
            setOriginCommand?.Invoke(x, y);
        }

        private void SetScale(ExpressionNode scaleX,ExpressionNode scaleY)
        {
            setScaleCommand?.Invoke(scaleX,scaleY);
        }

        private void SetRot(ExpressionNode angle)
        {
            setRotCommand?.Invoke(angle);
        }

        private void DrawLoop(GraphParams graphParams)
        {
            drawLoopCommand?.Invoke(graphParams);
        }

        private Token DeepCopyToken(Token copyFrom)
        {
            return new Token()
            {
                type = copyFrom.type,
                lexeme = copyFrom.lexeme,
                constValue = copyFrom.constValue,
                singleParamFunc = copyFrom.singleParamFunc,
                location = copyFrom.location,
            };
        }
    }

}
