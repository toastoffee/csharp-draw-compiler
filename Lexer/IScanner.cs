using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    public interface IScanner
    {
        // 初始化词法分析器，成功时返回true
        bool InitScanner(string filePath);

        // 识别并返回一个记号(TOKEN)
        // 遇到非法输入时 token.type = ERRTOKEN
        // 文件结束时 token.type = NONTOKEN
        Token GetToken();

        // 关闭词法分析器
        void CloseScanner();

        // 预处理
        int PreProcess(ref Token token);

        //
        int ScanMove(ref Token token,int firstChar);

        //
        bool PostProcess(ref Token token,int lastState);

        // 从输入的源程序中读入一个字符并返回它
        // 若遇到文件结束则返回 -1
        int GetChar();

        // 把预读的字符退回到输入源中
        void BackChar(int chr);

        // 将字符chr追加到记号文本末尾
        void AppendTokenLexeme(ref Token token, char chr);

        // 判断所给的字符串是否在符号表中
        Token JudgeKeyToken(string str);
    }
}
