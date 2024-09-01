using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    public class TableDrivenDFA : IDFA
    {

        public static List<TokenStateTrans> transfers = new()
        {
            MakeTrans(0, Consts.CK_LETTER, 1),
            MakeTrans(0, Consts.CK_DIGIT, 2),
            MakeTrans(0, '*', 4),
            MakeTrans(0, '/', 6),
            MakeTrans(0, '-', 7),
            MakeTrans(0, '+', 8),
            MakeTrans(0, ',', 9),
            MakeTrans(0, ';', 10),
            MakeTrans(0, '(', 11),
            MakeTrans(0, ')', 12),

            MakeTrans(1, Consts.CK_LETTER, 1),
            MakeTrans(1, Consts.CK_DIGIT, 1),
            
            MakeTrans(2, Consts.CK_DIGIT, 2),
            MakeTrans(2, '.', 3),

            MakeTrans(3, Consts.CK_DIGIT, 3),

            MakeTrans(4, '*', 5),

            MakeTrans(6, '/', 13),

            MakeTrans(7, '-', 13),

            GetTransEnd(), // 结束标志
        };

        public static List<FinalState> finalStates = new()
        {
            MakeFinalState(1, TokenType.ID), // 识别后进一步确定是哪个关键字

            MakeFinalState(2, TokenType.CONST_ID), // 识别后应得到对应的数值
            MakeFinalState(2, TokenType.CONST_ID),

            MakeFinalState(4, TokenType.MUL),
            MakeFinalState(5, TokenType.POWER),
            MakeFinalState(6, TokenType.DIV),
            MakeFinalState(7, TokenType.MINUS),

            MakeFinalState(8, TokenType.PLUS),
            MakeFinalState(9, TokenType.COMMA),
            MakeFinalState(10, TokenType.SEMICO),
            MakeFinalState(11, TokenType.L_BRACKET),
            MakeFinalState(12, TokenType.R_BRACKET),

            MakeFinalState(13, TokenType.COMMENT),   // 识别后应丢弃直到行尾的字符

            MakeFinalState(-1, TokenType.ERRTOKEN),
        };

        public int GetStartState()
        {
            return 0;
        }

        // 判断指定状态是否为DFA的终态
        // 若是，则返回该终态对应的记号类别，否则返回ERRTOKEN
        public TokenType IsStateFinal(int state)
        {
            foreach(FinalState final in finalStates)
            {
                if(state == final.state)
                {
                    return final.kink;
                }
            }
            return TokenType.ERRTOKEN;
        }
        
        // 参数：
        //  startSrc 当前状态
        //  chr 当前字符
        //  返回值：
        //  -1：此时没有状态转移
        //  >=0: 新的状态
        //  注意：若遇到当前DFA不接受的字符时，则没有状态转移
        public int Move(int startSrc, uint chr)
        {
            // 先计算chr的种类码
            uint CK_OF_CHR = Consts.CK_CHAR;
            if(chr >= '0' && chr <= '9')
            {
                CK_OF_CHR = Consts.CK_DIGIT;
                chr = 0;
            }
            else if((chr >='a' && chr <= 'z') || (chr >= 'A' && chr <= 'Z'))
            {
                CK_OF_CHR = Consts.CK_LETTER;
                chr = 0;
            }
            else
            {
                CK_OF_CHR = Consts.CK_CHAR;
            }

            // 生成查询的关键字
            uint key = MakeQueryIdx(startSrc,CK_OF_CHR | chr);

            // 查找转移
            
            foreach(TokenStateTrans trans in transfers)
            {
                if(trans.idx == GetTransEnd().idx)
                {
                    continue;
                }
                if(trans.idx == key)
                {
                    return trans.stateTransTo;
                }
            }
            return -1;
        }

        private static TokenStateTrans GetTransEnd()
        {
            return MakeTrans(255, Consts.CK_NULL,255);
        }

        private static uint MakeQueryIdx(int fromState,uint chr)
        {
            uint ret = (uint)(fromState << 24);
            ret = ret | chr;
            return ret;
        }

        private static TokenStateTrans MakeTrans(int fromState,uint chr,int toState)
        {
            return new TokenStateTrans
            {
                idx = MakeQueryIdx(fromState, chr),
                stateTransTo = toState
            };
        }

        private static FinalState MakeFinalState(int final,TokenType type)
        {
            return new FinalState
            {
                state = final,
                kink = type
            };
        }
    }

    public struct TokenStateTrans
    {
        public uint idx;    // move(state,chr)的参数合成的查询关键字
        public int stateTransTo;    // 转移的目标状态
    }

    public struct FinalState
    {
        public int state;  // 终态
        public TokenType kink;    // 该终态所识别的记号类别
    }

}
