using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class VersionDeclaration : GraphNode
    {
        internal VersionDeclaration(IEnumerable<Token> source, int version)
            : base(source)
        {
            Version = version;
        }

        public int Version { get; }
    }
}