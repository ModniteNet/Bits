using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class Value : GraphNode
    {
        internal Value(IEnumerable<Token> source, ValueType valueType, string value)
            : base(source)
        {
            ValueType = valueType;
            ValueString = value;
        }

        public ValueType ValueType { get; }

        public string ValueString { get; }
    }
}