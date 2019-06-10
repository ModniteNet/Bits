using System.Collections.Generic;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class EnumValueSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            var source = new Queue<Token>();

            if (tokens.Expect(TokenType.Identifier))
            {
                var identifier = tokens.Pop();
                source.Enqueue(identifier);

                if (tokens.Expect(TokenType.ColonSymbol))
                {
                    var colon = tokens.Pop();
                    source.Enqueue(colon);

                    var valueSyntax = new ValueSyntax();
                    if (valueSyntax.TryParse(tokens, out GraphNode value))
                    {
                        node = new EnumValue(source, identifier.Value, (Value)value);
                        return true;
                    }
                    else
                    {
                        // TODO: syntax error
                        node = null;
                        return false;
                    }
                }
                else
                {
                    node = new EnumValue(source, identifier.Value, null);
                    return true;
                }
            }

            // TODO: syntax error
            node = null;
            return false;
        }
    }
}