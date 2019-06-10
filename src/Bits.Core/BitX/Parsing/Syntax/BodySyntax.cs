namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class BodySyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            node = null;

            if (tokens.Count == 0)
                return true;

            var possibleSyntaxList = new Syntax[]
            {
                new VariableAssignmentSyntax(),
                new OpenStatementSyntax(),
                new EnumDeclarationSyntax(),
                new BlockDeclarationSyntax(),
                new PropertyDeclarationSyntax()
            };

            foreach (Syntax syntax in possibleSyntaxList)
            {
                if (syntax.TryParse(tokens, out node))
                    return true;
            }

            // TODO: syntax error
            return false;
        }
    }
}