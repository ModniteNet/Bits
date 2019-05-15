// Modnite Bits - Copyright (c) 2019 wumbo

using System.Collections.Generic;

namespace Bits.Core.Schema
{
    public sealed class SchemaTokenizer
    {
        public static IEnumerable<SchemaToken> Tokenize(string schema)
        {
            bool isComment = false;
            bool isString = false;
            char[] tokenBuffer = new char[256];
            int tokenIndex = 0;
            int tokenLength = 0;
            int line = 1;
            int col = 1;
            for (int i = 0; i < schema.Length; i++)
            {
                char c = schema[i];
                if (isComment && c == '\n')
                {
                    isComment = false;
                    line++;
                    col = 1;
                }
                else if (schema[i] == '#' && !isComment)
                {
                    isComment = true;
                }
                else if (!isComment && isString)
                {
                    tokenBuffer[tokenIndex++] = schema[i];
                    tokenLength++;

                    if (schema[i] == '"')
                        isString = false;
                }
                else if (!isComment && !isString)
                {
                    if (c == '"')
                    {
                        isString = true;
                        tokenBuffer[tokenIndex++] = schema[i];
                        tokenLength++;
                    }
                    else
                    {
                        bool isWhiteSpace = char.IsWhiteSpace(c);
                        if (c == '(' || c == ')' || c == '{' || c == '}' || c == ':' || c == '>' || c == '<' || c == '!' || c == '=' || isWhiteSpace)
                        {
                            if (tokenLength > 0)
                            {
                                yield return new SchemaToken(new string(tokenBuffer, 0, tokenLength), line, col);
                                tokenIndex = tokenLength = 0;
                            }

                            if (!isWhiteSpace)
                                yield return new SchemaToken(c + "", line, col);

                            if (c == '\n')
                            {
                                line++;
                                col = 1;
                            }
                        }
                        else
                        {
                            tokenBuffer[tokenIndex++] = schema[i];
                            tokenLength++;
                        }
                    }
                }
            }
        }
    }
}