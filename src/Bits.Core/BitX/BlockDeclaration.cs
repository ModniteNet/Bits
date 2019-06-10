using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class BlockDeclaration : GraphNode
    {
        internal BlockDeclaration(IEnumerable<Token> source, string identifier, bool isMain, params GraphNode[] children)
            : base(source)
        {
            Identifier = identifier;
            IsMain = isMain;

            foreach (var child in children)
                AddChildNode(child);
        }

        public string Identifier { get; }

        public bool IsMain { get; }
    }
}