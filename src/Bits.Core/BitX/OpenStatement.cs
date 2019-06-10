using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class OpenStatement : GraphNode
    {
        internal OpenStatement(IEnumerable<Token> source, Value value)
            : base(source)
        {
            AddChildNode(value);
        }
    }
}