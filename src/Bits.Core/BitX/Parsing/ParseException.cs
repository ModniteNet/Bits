﻿using System;

namespace Bits.Core.BitX.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string errorCode, string file, int line, int column)
            : base($"Parse Error in file {file} at line {line} col {column} ({errorCode}): {GetErrorMessageFromCode(errorCode)}")
        {
            File = file;
            Line = line;
            Column = column;
        }

        private static string GetErrorMessageFromCode(string errorCode)
        {
            if (Strings.ErrorMessages.TryGetValue(errorCode, out string message))
                return message;

            return $"Missing error description in {nameof(Strings.ErrorMessages)} table";
        }

        public string File { get; }

        public int Line { get; }

        public int Column { get; }
    }
}