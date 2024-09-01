using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.Lexer;

namespace MyCompiler.Parser
{
    public class ErrorLog
    {   

        private FileStream logFs;
        private StreamWriter logSw;

        public void InitErrorLog(string logPath)
        {
            FileMode mode;
            if (!System.IO.File.Exists(logPath))
            {
                mode = FileMode.Create;
            }
            else
            {
                mode = FileMode.Open;
            }

            logFs = new FileStream(logPath, mode, FileAccess.Write);   //创建写入文件
            logSw = new StreamWriter(logFs);
            
        }

        public void CloseErrorLog()
        {
            logSw.Close();
            logFs.Close();
        }

        public void LogErrorMsg(SyntaxError err,Token token)
        {
            switch(err)
            {
                case SyntaxError.IlleagalToken:
                    {
                        WriteErrorLog(token.location.row, token.lexeme, "非法单词");
                        break;
                    }
                case SyntaxError.NonExpectedToken:
                    {
                        WriteErrorLog(token.location.row, token.lexeme, "不是预期记号");
                        break;
                    }
            }

        }

        private void WriteErrorLog(uint row,string errLexeme,string description)
        {
            logSw.WriteLine($"error in line {row}:{errLexeme}  ->  {description}");
        }
    }
}
