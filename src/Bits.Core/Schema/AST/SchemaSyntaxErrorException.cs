// Modnite Bits - Copyright (c) 2019 wumbo

using System;

namespace Bits.Core.Schema.AST
{
    public class SchemaSyntaxErrorException : Exception
    {
        public SchemaSyntaxErrorException(SchemaToken token, string message = "")
            : base($"Unexpected symbol '{token.Value}' at line {token.Line} column {token.Column}. {message}")
        {
            Symbol = token.Value;
            Line = token.Line;
            Column = token.Column;
        }

        public string Symbol { get; }
        public int Line { get; }
        public int Column { get; }
    }
}