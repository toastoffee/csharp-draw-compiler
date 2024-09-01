using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    class Scanner : IScanner
    {
        // 编译源代码输入
        private SourceReader sourceReader;

        // 当前记号所在的位置
        private Position currentPos = new Position { row =1,col = 0};

        // DFA
        private IDFA dfa;

        // 初始化词法分析器
        public bool InitScanner(string filePath)
        {
            currentPos.row = 1;
            currentPos.col = 0;

            sourceReader = new SourceReader(filePath);
            dfa = new TableDrivenDFA();

            if (sourceReader.IsAvailable())
            {
                return true;
            }

            return false;
        }

        // 关闭词法分析器
        public void CloseScanner()
        {
            if (sourceReader.IsAvailable())
            {
                sourceReader.CloseReader();
            }
        }

        // 识别并返回一个记号(TOKEN)
        // 遇到非法输入时 token.type = ERRTOKEN
        // 文件结束时 token.type = NONTOKEN
        public Token GetToken()
        {
            int firstChar;  //记号开始的第1个字符
            int lastState = -1;    // 识别记号结束时的状态
            Position tokenPos = new Position {row=1,col=0 }; // 当前记号的起始位置
            Token token = new Token(); // 被识别到的token对象
            bool toBeContinued;

            do
            {
                // 第1步：预处理，跳过空白字符
                firstChar = PreProcess(ref token);

                if(firstChar == -1) // 文件结束了
                {
                    token.type = TokenType.NONTOKEN;
                    return token;
                }
                token.location = tokenPos = currentPos;

                // 第2步：边扫描输入，边转移状态
                lastState = ScanMove(ref token, firstChar);

                // 第3步: 后处理：根据终态所标记的记号种类，进行特殊处理
                toBeContinued = PostProcess(ref token, lastState);

            } while (toBeContinued);

            token.location = tokenPos;
            return token;
        }

        // 从输入的源程序中读入一个字符并返回它
        // 若遇到文件结束则返回 -1
        public int GetChar()
        {
            int nextChar;

            nextChar = sourceReader.GetChar();

            if (nextChar == -1)
            {
                return -1;
            }
            else
            {
                // 换行符
                if(nextChar == '\n')
                {
                    currentPos.row++;
                    currentPos.col = 0;
                    return nextChar;
                }
                else
                {
                    currentPos.col++;
                    return nextChar.ToUpper();
                }
            }
             
        }

        // 识别一个记号的前处理：跳过空白字符，并读取第一个非空白字符
        // 返回值：-1 文件结束，其他值表示字符本身
        public int PreProcess(ref Token token)
        {
            int currentChar;
            token = new Token();

            do
            {
                currentChar = GetChar();

                if(currentChar == -1)
                {
                    return -1;
                }
                if (!currentChar.IsSpace()) break;

            } while (true);

            //此时，currentChar 为记号的第1个字符
            return currentChar;
        }

        public int ScanMove(ref Token token,int firstChar)
        {
            int currentState, nextState; // 当前状态 下一状态
            int currentChar; // 当前字符

            currentChar = firstChar;
            currentState = dfa.GetStartState();

            do
            {
                nextState = dfa.Move(currentState, (uint)currentChar);

                if (nextState < 0)   // 没有转移了
                {
                    // 第一个字符就无效，则丢弃它，否则因为反复读到该字符而陷入死循环
                    if (token.lexeme.Length > 0 && token.lexeme[0] == '\0')
                    {
                        char chr = (char)currentChar;
                        token.lexeme = chr.ToString();
                    }
                    else
                    {
                        BackChar(currentChar);
                    }
                    break;
                }

                AppendTokenLexeme(ref token, (char)currentChar);

                currentState = nextState;
                currentChar = GetChar();
                if (currentChar == -1) // 文件结束了
                    break;
            } while (true);

            return currentState;
        }

        // 根据终态所标记的记号种类信息，进行特殊处理
        // 若返回非0，则表示当前刚处理完了“注释”，需要调用者接着获取下一个记号
        public bool PostProcess(ref Token token,int lastState)
        {
            bool toBeContinued = false;
            TokenType tokenKey = dfa.IsStateFinal(lastState);
            switch(tokenKey)
            {
                case TokenType.ID:
                    {
                        Token id = JudgeKeyToken(token.lexeme);
                        
                        if(TokenType.ERRTOKEN == id.type)
                        {
                            token.type = TokenType.ERRTOKEN;
                        }
                        else
                        {
                            token = id;
                        }
                    }
                    break;
                case TokenType.CONST_ID:
                    {
                        token.type = tokenKey;
                        token.constValue = Convert.ToDouble(token.lexeme);
                        break;
                    }
                case TokenType.COMMENT: // 行注释：忽略直到行尾的文本，并获取下一个记号
                    {
                        int chr;
                        while(true)
                        {
                            chr = GetChar();
                            if (chr == '\n' || chr == -1)
                                break;
                        }

                        toBeContinued = true;
                        break;
                    }
                default:
                    {
                        token.type = tokenKey;
                        break;
                    }
            }
            return toBeContinued;
        }

        // 把预读的字符退回到输入源中
        public void BackChar(int chr)
        {
            // 文件结束标志、换行不回退
            if (chr == -1 || chr == '\n')
                return;
            sourceReader.UngetChar(chr);
            currentPos.col--;
        }

        // 将字符chr追加到记号文本末尾
        public void AppendTokenLexeme(ref Token token, char chr)
        {
            token.lexeme += chr;
        }

        // 判断所给的字符串是否在符号表中
        public Token JudgeKeyToken(string str)
        {
            Token errToken = new Token {type = TokenType.ERRTOKEN };

            foreach(Token t in Consts.tokenTable)
            {
                if(t.lexeme == str)
                {
                    return t;
                }
            }
            return errToken;
        }
    
    }



    // 扩展方法
    public static class CharExtensionMethods
    {
        // 将char返回大写形式
        public static int ToUpper(this int chr)
        {
            // 如果为小写字母则返回大写形式
            // 其他则原样返回
            if (chr >= 'a' && chr <= 'z')
            {
                return chr + ('A' - 'a');
            }
            else
            {
                return chr;
            }
        }

        // 处理非ASCII字符，如(半个)中文字符
        public static bool IsSpace(this int chr)
        {
            if(chr < 0 || chr > 0x7e)
            {
                return false;
            }
            return Char.IsWhiteSpace((char)chr);
        }
    }
}
