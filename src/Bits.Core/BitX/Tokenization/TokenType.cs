namespace Bits.Core.BitX.Tokenization
{
    /// <summary>
    /// Indicates the type of a token.
    /// </summary>
    public enum TokenType
    {
        Undefined,
        Identifier,

        OpenKeyword,
        EnumKeyword,
        BlockKeyword,
        MainKeyword,
        CheckKeyword,
        AsKeyword,
        IfKeyword,
        ElseKeyword,
        VersionKeyword,

        SignedIntegerTypeKeyword,
        UnsignedIntegerTypeKeyword,
        BitfieldTypeKeyword,
        BitTypeKeyword,

        StringLiteral,
        HexadecimalLiteral,
        IntegerLiteral,
        BinaryLiteral,
        TrueLiteral,
        FalseLiteral,

        OpenParenthesesSymbol,
        CloseParenthesesSymbol,
        OpenCurlyBraceSymbol,
        CloseCurlyBraceSymbol,
        OpenBracketSymbol,
        CloseBracketSymbol,
        ColonSymbol,
        EqualsSymbol,
        NotEqualsSymbol,
        GreaterThanSymbol,
        GreaterThanOrEqualsSymbol,
        LessThanSymbol,
        LessThanOrEqualsSymbol,
        PlusSymbol,
        MinusSymbol,
        StarSymbol,
        ForwardSlashSymbol,
        NegationSymbol
    }
}