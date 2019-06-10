namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class ScriptSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            node = new Script();

            while (tokens.Count > 0)
            {
                var versionDeclarationSyntax = new VersionDeclarationSyntax();
                var bodySyntax = new BodySyntax();

                if (versionDeclarationSyntax.TryParse(tokens, out GraphNode versionDeclaration))
                {
                    node.AddChildNode(versionDeclaration);
                }
                else if (bodySyntax.TryParse(tokens, out GraphNode body))
                {
                    node.AddChildNode(body);
                }
                else
                {
                    // TODO: syntax error
                    node = null;
                    return false;
                }
            }

            return true;
        }
    }
}