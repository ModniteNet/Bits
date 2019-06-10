namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class TypeDeclarationSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.ExpectAny(TokenType.SignedIntegerTypeKeyword, TokenType.UnsignedIntegerTypeKeyword, TokenType.BitfieldTypeKeyword, TokenType.BitTypeKeyword, TokenType.Identifier))
            {
                var token = tokens.Pop();
                node = new TypeDeclaration(new [] { token }, token.Value);
                return true;
            }

            // TODO: syntax error
            node = null;
            return false;
        }
    }
}