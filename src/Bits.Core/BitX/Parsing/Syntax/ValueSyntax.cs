using System.Collections.Generic;
using System.Linq;

namespace Bits.Core.BitX.Parsing.Syntax
{
    public sealed class ValueSyntax : Syntax
    {
        private static IReadOnlyDictionary<TokenType, ValueType> EnumMapping =
            new Dictionary<TokenType, ValueType>
            {
                [TokenType.StringLiteral] = ValueType.String,
                [TokenType.HexadecimalLiteral] = ValueType.Integer,
                [TokenType.BinaryLiteral] = ValueType.Integer,
                [TokenType.IntegerLiteral] = ValueType.Integer,
                [TokenType.TrueLiteral] = ValueType.Boolean,
                [TokenType.FalseLiteral] = ValueType.Boolean,
                [TokenType.Identifier] = ValueType.Identifier
            };

        public override bool TryParse(TokenStack tokens, out GraphNode node)
        {
            if (tokens.ExpectAny(EnumMapping.Keys.ToArray()))
            {
                var token = tokens.Pop();
                node = new Value(new[] { token }, EnumMapping[token.TokenType], token.Value);
                return true;
            }

            node = null;
            return false;
        }
    }
}