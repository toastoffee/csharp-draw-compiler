namespace MyCompiler.Lexer
{
    public delegate double SingleParamFuncHandler(double d_param);

    public struct Position
    {
        public uint row;
        public uint col;
    }

    public struct Token
    {
        public TokenType type; // 记号的类型
        public string lexeme; // 构成记号的字符串
        public double constValue; // 若为常数，则为常数的值
        public SingleParamFuncHandler singleParamFunc; // 若为一参数函数，则为函数的委托 

        public Position location; // 记号在源代码中的位置
    }
}