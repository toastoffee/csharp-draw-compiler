using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Lexer
{
    public class SourceReader
    {
        // 编译源代码输入流
        private StreamReader sourceFileStream = null;

        // 源文件是否可用
        private bool isSourceAvailable = false;

        // 前置的输入栈 用以实现 UngetChar
        private Stack<int> preChars = new();

        public SourceReader(string filePath)
        {
            sourceFileStream = new StreamReader(filePath);

            if (sourceFileStream != null)
            {
                isSourceAvailable = true;
            }
            else
            {
                isSourceAvailable = false;
            }
        }

        public void CloseReader()
        {
            if (sourceFileStream != null)
            {
                sourceFileStream.Close();
            }
        }

        public int GetChar()
        {
            // 如果有unget传入的char 则先返回这些char
            if(preChars.Count != 0)
            {
                return preChars.Pop();
            }

            // 如果无unget传入且文件已读到结尾，则返回-1
            if (sourceFileStream.EndOfStream)
            {
                return -1;
            }
            else
            {
                return sourceFileStream.Read();
            }
        }

        public void UngetChar(int chr)
        {
            preChars.Push(chr);
        }


        // 获取是否可用
        public bool IsAvailable()
        {
            return isSourceAvailable;
        }
    }
}
