// Modnite Bits - Copyright (c) 2019 wumbo

namespace Bits.Core.Schema
{
    public struct SchemaToken
    {
        public SchemaToken(string value, int line, int col)
        {
            Value = value;
            Line = line;
            Column = col;
        }

        public string Value { get; }
        public int Line { get; }
        public int Column { get; }
    }
}
