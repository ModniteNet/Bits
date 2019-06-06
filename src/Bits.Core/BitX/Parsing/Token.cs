namespace Bits.Core.BitX.Parsing
{
    /// <summary>
    /// Represents a token in a Bit-X script.
    /// </summary>
    public sealed class Token
    {
        public Token(TokenType tokenType, string value, int line, int column)
        {
            TokenType = tokenType;
            Value = value;
            Line = line;
            Column = column;
        }

        public TokenType TokenType { get; }

        public string Value { get; }

        public int Line { get; }

        public int Column { get; }
    }
}