using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class VariableAssignment : GraphNode
    {
        internal VariableAssignment(IEnumerable<Token> source, string identifier, Value value)
            : base(source)
        {
            Identifier = identifier;
            AddChildNode(value);
        }

        public string Identifier { get; }
    }
}