using System.Collections.Generic;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class EnumDeclarationSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.ExpectSequence(TokenType.EnumKeyword, TokenType.Identifier, TokenType.OpenCurlyBraceSymbol))
            {
                var source = new Queue<Token>();

                var enumKeyword = tokens.Pop();
                var identifier = tokens.Pop();
                var openBrace = tokens.Pop();

                source.Enqueue(enumKeyword);
                source.Enqueue(identifier);
                source.Enqueue(openBrace);

                var enumValues = new List<EnumValue>();

                var enumBodySyntax = new EnumValueSyntax();
                while (tokens.Peek().TokenType != TokenType.CloseCurlyBraceSymbol)
                {
                    if (enumBodySyntax.TryParse(tokens, out GraphNode enumValue))
                    {
                        enumValues.Add((EnumValue)enumValue);
                    }
                    else
                    {
                        // TODO: Set error message
                        node = null;
                        return false;
                    }
                }

                var closeBrace = tokens.Pop();
                source.Enqueue(closeBrace);

                node = new EnumDeclaration(source, identifier.Value, enumValues);
                return true;
            }

            // TODO: Set error message
            node = null;
            return false;
        }
    }
}