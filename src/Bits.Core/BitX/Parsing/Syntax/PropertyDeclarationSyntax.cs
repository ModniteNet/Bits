namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class PropertyDeclarationSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            var typeDeclarationSyntax = new TypeDeclarationSyntax();

            if (typeDeclarationSyntax.TryParse(tokens, out GraphNode typeDeclaration))
            {
                if (tokens.Expect(TokenType.Identifier))
                {
                    var identifier = tokens.Pop();

                    if (tokens.Expect(TokenType.AsKeyword))
                    {
                        var asKeyword = tokens.Pop();

                        if (typeDeclarationSyntax.TryParse(tokens, out GraphNode asTypeDeclaration))
                        {
                            node = new PropertyDeclaration(new[] { identifier }, identifier.Value, (TypeDeclaration)typeDeclaration, (TypeDeclaration)asTypeDeclaration);
                            return true;
                        }
                        else
                        {
                            // TODO: syntax error
                        }
                    }
                    else
                    {
                        node = new PropertyDeclaration(new[] { identifier }, identifier.Value, (TypeDeclaration)typeDeclaration);
                        return true;
                    }
                }
                else
                {
                    // TODO: syntax error
                }
            }

            // TODO: syntax error
            node = null;
            return false;
        }
    }
}