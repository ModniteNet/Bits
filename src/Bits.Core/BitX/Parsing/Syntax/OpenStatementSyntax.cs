using System.Collections.Generic;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class OpenStatementSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.Expect(TokenType.OpenKeyword))
            {
                var source = new List<Token>();
                source.Add(tokens.Pop());

                if (tokens.ExpectAny(TokenType.StringLiteral, TokenType.Identifier))
                {
                    var valueToken = tokens.Pop();
                    source.Add(valueToken);

                    var value = new Value(new [] { valueToken }, valueToken.TokenType == TokenType.Identifier ? ValueType.Identifier : ValueType.String, valueToken.Value);
                    node = new OpenStatement(source, value);
                    return true;
                }
            }

            // TODO: syntax error
            node = null;
            return false;
        }
    }
}