// Modnite Bits - Copyright (c) 2019 wumbo

using Bits.Core.Schema.AST;
using System.Linq;

namespace Bits.Core.Schema
{
    public sealed class Schema
    {
        private Schema() { }

        public static Schema Parse(string schema)
        {
            var tokens = SchemaTokenizer.Tokenize(schema);
            var syntaxTree = SchemaSyntaxTree.Parse(tokens.ToArray());

            // TODO: Build state machine from syntaxTree

            return new Schema();
        }
    }
}