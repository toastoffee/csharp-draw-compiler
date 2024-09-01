using MyCompiler.Lexer;
using System;
using MyCompiler.Parser;
using MyCompiler.Semantic;
using MyCompiler.Executor;

namespace MyCompiler
{
    class Program
    {
        private static IScanner scanner;

        static void Main(string[] args)
        {
            /*            scanner = new Scanner();
                        bool flag = scanner.InitScanner(@"C:\DrawCompiler\test.txt");


                        Token token;

                        while (true)
                        {
                            token = scanner.GetToken();
                            Console.WriteLine($"location:({token.location.row},{token.location.col}),{token.lexeme},{token.type}");

                            if (token.type == TokenType.NONTOKEN)
                            {
                                break;
                            }
                        }
                        Console.WriteLine(flag);*/

            DrawExecutorBase executor = new DrawExecutor();
            executor.DrawExecutorStart();
        }
    }
}
