using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bits.Core.BitX.Parsing
{
    /// <summary>
    /// Provides a <see cref="Tokenize(string)"/> method to tokenize Bit-X scripts.
    /// </summary>
    public static class Tokenizer
    {
        /// <summary>
        /// Indicates the current script version.
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Defines the keyword used for the version declaration.
        /// </summary>
        private const string VersionKeyword = "version";

        /// <summary>
        /// Defines the regex pattern for comments.
        /// </summary>
        private const string CommentPattern = @"/\*(.*?)\*/|//(.*?)\r?\n|""((\\[^\n]|[^""\n])*)|@(""[^""]*"")+";

        /// <summary>
        /// Provides a <see cref="MatchEvaluator"/> that returns line breaks in place of line comments.
        /// </summary>
        private static readonly MatchEvaluator CommentEvaluator = 
            new MatchEvaluator(
                match =>
                {
                    if (match.Value.StartsWith("/*") || match.Value.StartsWith("//"))
                        return new string('\n', match.Value.Count(c => c == '\n'));
                    else
                        return match.Value;
                }
            );

        /// <summary>
        /// Defines regex patterns for literals.
        /// </summary>
        private static readonly IReadOnlyDictionary<TokenType, Regex> LiteralPatterns =
            new Dictionary<TokenType, Regex>
            {
                [TokenType.StringLiteral] = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*"""),
                [TokenType.HexadecimalLiteral] = new Regex(@"\b0x[0-9a-fA-F]*\b"),
                [TokenType.BinaryLiteral] = new Regex(@"\b0b[0-1]*\b"),
                [TokenType.IntegerLiteral] = new Regex(@"\b[0-9]+"),
            };

        /// <summary>
        /// Defines keywords.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, TokenType> Keywords =
            new Dictionary<string, TokenType>
            {
                [VersionKeyword] = TokenType.VersionKeyword,
                ["open"] = TokenType.OpenKeyword,
                ["enum"] = TokenType.EnumKeyword,
                ["block"] = TokenType.BlockKeyword,
                ["main"] = TokenType.MainKeyword,
                ["check"] = TokenType.CheckKeyword,
                ["as"] = TokenType.AsKeyword,
                ["if"] = TokenType.IfKeyword,
                ["else"] = TokenType.ElseKeyword,
                ["int"] = TokenType.SignedIntegerTypeKeyword,
                ["uint"] = TokenType.UnsignedIntegerTypeKeyword,
                ["bitfield"] = TokenType.BitfieldTypeKeyword,
                ["bit"] = TokenType.BitTypeKeyword,
                ["true"] = TokenType.TrueLiteral,
                ["false"] = TokenType.FalseLiteral
            };

        /// <summary>
        /// Defines the keywords with an integer suffix (e.g. int16).
        /// </summary>
        private static readonly IReadOnlyCollection<TokenType> RangedKeywords =
            new List<TokenType>
            {
                TokenType.SignedIntegerTypeKeyword,
                TokenType.UnsignedIntegerTypeKeyword,
                TokenType.BitfieldTypeKeyword
            };

        /// <summary>
        /// Defines special characters as symbol tokens.
        /// </summary>
        private static readonly IReadOnlyDictionary<char, TokenType> Symbols =
            new Dictionary<char, TokenType>
            {
                ['('] = TokenType.OpenParenthesesSymbol,
                [')'] = TokenType.CloseParenthesesSymbol,
                ['{'] = TokenType.OpenCurlyBraceSymbol,
                ['}'] = TokenType.CloseCurlyBraceSymbol,
                ['['] = TokenType.OpenBracketSymbol,
                [']'] = TokenType.CloseBracketSymbol,
                [':'] = TokenType.ColonSymbol,
                ['='] = TokenType.EqualsSymbol,
                ['!'] = TokenType.NegationSymbol,
                ['≠'] = TokenType.NotEqualsSymbol,
                ['>'] = TokenType.GreaterThanSymbol,
                ['≥'] = TokenType.GreaterThanOrEqualsSymbol,
                ['<'] = TokenType.LessThanSymbol,
                ['≤'] = TokenType.LessThanOrEqualsSymbol,
                ['+'] = TokenType.PlusSymbol,
                ['-'] = TokenType.MinusSymbol,
                ['*'] = TokenType.StarSymbol,
                ['/'] = TokenType.ForwardSlashSymbol
            };

        /// <summary>
        /// Tokenizes the given script.
        /// </summary>
        /// <param name="script">The script to tokenize.</param>
        /// <returns>A collection of tokens from the script.</returns>
        public static IEnumerable<Token> Tokenize(string script)
        {
            script = RemoveComments(script);

            int version = GetScriptVersion(script);
            if (version < 1 || version > CurrentVersion)
            {
                throw new NotSupportedException($"Unsupported script version {version}");
            }

            var literalRegions = new Dictionary<int, int>();
            var literalTokens = new Dictionary<int, Token>();

            foreach (var literalPatternInfo in LiteralPatterns)
            {
                foreach (Match match in literalPatternInfo.Value.Matches(script))
                {
                    foreach (var (index, length) in literalRegions)
                    {
                        if (match.Index >= index && match.Index < index + length)
                            goto skipMatch;
                    }

                    var (line, column) = ConvertIndexToLineAndColumn(script, match.Index);

                    var token = new Token(literalPatternInfo.Key, match.Value, line, column);
                    literalRegions.Add(match.Index, match.Length);
                    literalTokens.Add(match.Index, token);

                    skipMatch:
                    continue;
                }
            }

            int tokenStart = 0;
            int tokenLength = 0;

            for (int index = 0; index < script.Length; index++)
            {
                Token CreateToken(TokenType? tokenType = null)
                {
                    (int line, int column) = ConvertIndexToLineAndColumn(script, index);
                    string value = script.Substring(tokenStart, tokenLength);
                    return new Token(tokenType ?? GetTokenTypeFromValue(value), value, line, column);
                }

                if (literalRegions.ContainsKey(index))
                {
                    if (tokenLength > 0)
                    {
                        yield return CreateToken();
                        tokenLength = 0;
                    }

                    yield return literalTokens[index];
                    index += literalRegions[index];
                }

                char c = script[index];
                bool isSymbol = Symbols.ContainsKey(c);

                if (char.IsWhiteSpace(c) || isSymbol)
                {
                    if (tokenLength > 0)
                        yield return CreateToken();

                    if (isSymbol)
                    {
                        tokenLength = 1;
                        tokenStart = index;
                        yield return CreateToken(Symbols[c]);
                    }

                    tokenLength = 0;
                }
                else
                {
                    if (tokenLength == 0)
                        tokenStart = index;

                    tokenLength++;
                }
            }
        }

        /// <summary>
        /// Returns the given script that has all line and block comments removed.
        /// </summary>
        /// <param name="script">The script to filter.</param>
        /// <returns>The filtered script.</returns>
        private static string RemoveComments(string script) =>
            Regex.Replace(script, CommentPattern, CommentEvaluator, RegexOptions.Singleline);

        /// <summary>
        /// Selects the best <see cref="TokenType"/> value from the given token value.
        /// </summary>
        /// <param name="value">The value of the token.</param>
        /// <returns>A <see cref="TokenType"/> value.</returns>
        private static TokenType GetTokenTypeFromValue(string value)
        {
            var tokenType = Keywords
                .Where(keywordInfo =>
                {
                    if (RangedKeywords.Contains(keywordInfo.Value))
                    {
                        if (value.StartsWith(keywordInfo.Key) && int.TryParse(value.Substring(keywordInfo.Key.Length), out int suffix))
                            return true;

                        return false;
                    }
                    else
                    {
                        return keywordInfo.Key == value;
                    }
                })
                .Select(keywordInfo => keywordInfo.Value)
                .FirstOrDefault();

            return tokenType == default(TokenType) ? TokenType.Identifier : tokenType;
        }

        /// <summary>
        /// Obtains the line and column from an index.
        /// </summary>
        /// <param name="script">The script to use.</param>
        /// <param name="index">The index in the script.</param>
        /// <returns>A tuple pair representing the line and column.</returns>
        private static (int line, int column) ConvertIndexToLineAndColumn(string script, int index)
        {
            int line = 1;
            int column = 0;

            for (int i = 0; i < index; i++)
            {
                column++;
                if (script[i] == '\n')
                {
                    line++;
                    column = 1;
                }
            }

            return (line, Math.Max(column, 1));
        }

        /// <summary>
        /// Searches for the version declaration in the given script, and returns the declared version.
        /// If no declaration was found, returns the current version.
        /// </summary>
        /// <param name="script">The script to search.</param>
        /// <returns>The declared version or the current version if a version declaration does not exist in the script.</returns>
        private static int GetScriptVersion(string script)
        {
            var versionRegex = new Regex($@"{VersionKeyword} \d*\b");
            var versionDeclaration = versionRegex.Match(script.TrimStart());
            if (versionDeclaration.Success)
            {
                return int.Parse(versionDeclaration.Value.Substring(versionDeclaration.Value.IndexOf(' ') + 1));
            }

            return CurrentVersion;
        }
    }
}