using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class EnumDeclaration : GraphNode
    {
        internal EnumDeclaration(IEnumerable<Token> source, string identifier, IEnumerable<EnumValue> values)
            : base(source)
        {
            Identifier = identifier;

            foreach (var value in values)
                AddChildNode(value);
        }

        public string Identifier { get; }
    }
}