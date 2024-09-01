using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    public static class Consts
    {
        public static List<Token> tokenTable = new List<Token>()
        {
            // 命名常数
            new Token{
                type = TokenType.CONST_ID,
                lexeme = "PI",
                constValue = 3.1415926,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.CONST_ID,
                lexeme = "E",
                constValue = 2.71828,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },

            // 参数
            new Token{
                type = TokenType.T,
                lexeme = "T",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },

            // 支持的函数名
            new Token{
                type = TokenType.FUNC,
                lexeme = "SIN",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(SinFunc),
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FUNC,
                lexeme = "COS",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(CosFunc),
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FUNC,
                lexeme = "TAN",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(TanFunc),
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FUNC,
                lexeme = "LN",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(LnFunc),
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FUNC,
                lexeme = "EXP",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(ExpFunc),
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FUNC,
                lexeme = "SQRT",
                constValue = 0.0,
                singleParamFunc = new SingleParamFuncHandler(SqrtFunc),
                location = new Position{row = 0,col = 0}
            },

            // 预留关键字
            new Token{
                type = TokenType.ORIGIN,
                lexeme = "ORIGIN",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.SCALE,
                lexeme = "SCALE",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.ROT,
                lexeme = "ROT",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.IS,
                lexeme = "IS",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FOR,
                lexeme = "FOR",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.FROM,
                lexeme = "FROM",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.TO,
                lexeme = "TO",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.STEP,
                lexeme = "STEP",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
            new Token{
                type = TokenType.DRAW,
                lexeme = "DRAW",
                constValue = 0.0,
                singleParamFunc = null,
                location = new Position{row = 0,col = 0}
            },
        };


        private static double SinFunc(double d_param)
        {
            return Math.Sin(d_param);
        }

        private static double CosFunc(double d_param)
        {
            return Math.Cos(d_param);
        }

        private static double TanFunc(double d_param)
        {
            return Math.Tan(d_param);
        }

        private static double LnFunc(double d_param)
        {
            return Math.Log(d_param);
        }

        private static double ExpFunc(double d_param)
        {
            return Math.Exp(d_param);
        }

        private static double SqrtFunc(double d_param)
        {
            return Math.Sqrt(d_param);
        }


        public static uint CK_CHAR = 0 << 16;
        public static uint CK_LETTER = 1U << 16;
        public static uint CK_DIGIT = 2U << 16;
        public static uint CK_NULL = 0x80U << 16;
    }
}
