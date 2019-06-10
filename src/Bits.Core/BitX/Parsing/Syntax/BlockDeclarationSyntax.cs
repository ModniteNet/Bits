using System.Collections.Generic;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class BlockDeclarationSyntax : Syntax
    {
        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            var source = new Queue<Token>();

            string blockIdentifier = null;
            bool isMain = false;

            if (tokens.ExpectSequence(TokenType.BlockKeyword, TokenType.MainKeyword, TokenType.Identifier, TokenType.OpenCurlyBraceSymbol))
            {
                var blockKeyword = tokens.Pop();
                var mainKeyword = tokens.Pop();
                var identifier = tokens.Pop();
                var openBrace = tokens.Pop();

                source.Enqueue(blockKeyword);
                source.Enqueue(mainKeyword);
                source.Enqueue(identifier);
                source.Enqueue(openBrace);

                blockIdentifier = identifier.Value;
                isMain = true;
            }
            else if (tokens.ExpectSequence(TokenType.BlockKeyword, TokenType.Identifier, TokenType.OpenCurlyBraceSymbol))
            {
                var blockKeyword = tokens.Pop();
                var identifier = tokens.Pop();
                var openBrace = tokens.Pop();

                source.Enqueue(blockKeyword);
                source.Enqueue(identifier);
                source.Enqueue(openBrace);

                blockIdentifier = identifier.Value;
            }
            else
            {
                while (source.Count > 0)
                    tokens.Push(source.Dequeue());

                // TODO: syntax error
                node = null;
                return false;
            }

            var blockChildren = new List<GraphNode>();

            while (tokens.Peek().TokenType != TokenType.CloseCurlyBraceSymbol)
            {
                var bodySyntax = new BodySyntax();
                if (bodySyntax.TryParse(tokens, out GraphNode body))
                {
                    blockChildren.Add(body);
                }
                else
                {
                    while (source.Count > 0)
                        tokens.Push(source.Dequeue());

                    // TODO: syntax error
                    node = null;
                    return false;
                }
            }

            source.Enqueue(tokens.Pop());

            node = new BlockDeclaration(source, blockIdentifier, isMain, blockChildren.ToArray());
            return true;
        }
    }
}