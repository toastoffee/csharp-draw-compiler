namespace MyCompiler.Lexer
{
    public enum TokenType
    {
        // 正规式设计
        ID,
        COMMENT,
        
        // 预留字
        ORIGIN,
        SCALE,
        ROT,
        IS,
        TO,
        STEP,
        DRAW,
        FOR,
        FROM,
        
        // 参数
        T,
        
        // 分隔符
        SEMICO,
        L_BRACKET,
        R_BRACKET,
        COMMA,

        // 运算符
        PLUS,
        MINUS,
        MUL,
        DIV,
        POWER,

        // 函数调用
        FUNC,

        // 常数
        CONST_ID,
        
        // 空记号（源程序结束）
        NONTOKEN,
        // 出错记号（非法输入）
        ERRTOKEN,
    }
}
