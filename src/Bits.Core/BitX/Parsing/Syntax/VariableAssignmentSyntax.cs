using System.Collections.Generic;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class VariableAssignmentSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.ExpectSequence(TokenType.Identifier, TokenType.ColonSymbol))
            {
                var queue = new Queue<Token>();

                var identifier = tokens.Pop();
                var colon = tokens.Pop();

                queue.Enqueue(identifier);
                queue.Enqueue(colon);

                var valueSyntax = new ValueSyntax();
                var valueToken = tokens.Peek();

                if (valueSyntax.TryParse(tokens, out GraphNode value))
                {
                    queue.Enqueue(valueToken);
                    node = new VariableAssignment(queue, identifier.Value, (Value)value);
                    return true;
                }
                else
                {
                    while (queue.Count > 0)
                        tokens.Push(queue.Dequeue());
                }
            }

            node = null;
            return false;
        }
    }
}