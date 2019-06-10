using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class PropertyDeclaration : GraphNode
    {
        internal PropertyDeclaration(IEnumerable<Token> source, string identifier, TypeDeclaration type, TypeDeclaration castAsType = null)
            : base(source)
        {
            Identifier = identifier;
            AddChildNode(type);
            AddChildNode(castAsType);
        }

        public string Identifier { get; }
    }
}