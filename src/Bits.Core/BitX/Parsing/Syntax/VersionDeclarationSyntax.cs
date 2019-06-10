namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class VersionDeclarationSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.ExpectSequence(TokenType.VersionKeyword, TokenType.IntegerLiteral))
            {
                var keyword = tokens.Pop();
                var integerLiteral = tokens.Pop();

                node = new VersionDeclaration(new[] { keyword, integerLiteral }, int.Parse(integerLiteral.Value));
                return true;
            }

            node = null;
            return false;
        }
    }
}