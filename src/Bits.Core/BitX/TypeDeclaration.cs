using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public sealed class TypeDeclaration : GraphNode
    {
        public TypeDeclaration(IEnumerable<Token> source, string type)
            : base(source)
        {
            Type = type;

            // TODO: derive propertytype and bitsize from type
        }

        public PropertyType PropertyType { get; }

        public uint BitSize { get; }

        public string Type { get; }
    }
}