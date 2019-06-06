using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bits.Core.BitX.Parsing
{
    /// <summary>
    /// Represents the formal grammar of the Bit-X language.
    /// </summary>
    public sealed class Grammar : Dictionary<string, Rule>
    {
        /// <summary>
        /// Represents the current grammar version.
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Maps the version number to the embedded grammar file resource.
        /// </summary>
        private static readonly IReadOnlyDictionary<int, string> GrammarFiles =
            new Dictionary<int, string>
            {
                [1] = "Bits.Core.BitX.Parsing.Grammars.grammar-v1"
            };

        /// <summary>
        /// Initializes a new formal grammar representation based on the specified language version.
        /// </summary>
        /// <param name="version">The version of the language.</param>
        public Grammar(int version = CurrentVersion)
        {
            if (!GrammarFiles.ContainsKey(version))
                throw new NotSupportedException(Strings.ErrorMessages["grammar_unsupported_version"]);

            Version = version;

            var assembly = this.GetType().GetTypeInfo().Assembly;
            LoadGrammar(assembly.GetManifestResourceStream(GrammarFiles[Version]));
        }

        /// <summary>
        /// Gets the language version this grammar represents.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Parses the grammar definition in the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the grammar definition.</param>
        private void LoadGrammar(Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                int currentLine = 0;
                int currentColumn;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    currentLine++;
                    currentColumn = 1;

                    if (line.Length == 0 || line.StartsWith('#'))
                        continue;

                    string[] tokens = line.Split(null);

                    if (tokens.Length < 2 || tokens[1] != "::=" || tokens[0][0] != '<' || tokens[0][tokens[0].Length - 1] != '>')
                        throw new ParseException("grammar_invalid", GrammarFiles[Version], currentLine, currentColumn);

                    if (this.ContainsKey(tokens[0]))
                        throw new ParseException("grammar_rule_exists", GrammarFiles[Version], currentLine, currentColumn);

                    var rule = new Rule();
                    var phrase = new Phrase();

                    for (int i = 2; i < tokens.Length; i++)
                    {
                        currentColumn = i;

                        if (i == 2 && tokens[i] == "|")
                        {
                            phrase.Add(null);
                        }

                        if (tokens[i] == "|")
                        {
                            rule.Add(phrase);
                            phrase = new Phrase();
                        }
                        else if (tokens[i][0] == '<' && tokens[i][tokens[i].Length - 1] == '>')
                        {
                            phrase.Add(new RuleReferenceSymbol(tokens[i], currentLine, currentColumn));
                        }
                        else
                        {
                            if (Enum.TryParse(tokens[i], out TokenType tokenType))
                            {
                                phrase.Add(new TokenTypeSymbol(tokenType, currentLine, currentColumn));
                            }
                            else
                            {
                                throw new ParseException("grammar_invalid_token_type", GrammarFiles[Version], currentLine, currentColumn);
                            }
                        }
                    }

                    rule.Add(phrase);
                    this.Add(tokens[0], rule);
                }
            }

            var referenceNodes = from rule in Values
                                 from phrase in rule
                                 from node in phrase
                                 where node is RuleReferenceSymbol
                                 where !ContainsKey((node as RuleReferenceSymbol).RuleName)
                                 select node;

            foreach (var node in referenceNodes)
            {
                if (!ContainsKey((node as RuleReferenceSymbol).RuleName))
                {
                    throw new ParseException("grammar_nonexistent_rule", GrammarFiles[Version], node.Line, node.Column);
                }
            }
        }
    }

    /// <summary>
    /// Represents a grammar rule, which is a set of possible <see cref="Phrase"/>s.
    /// </summary>
    public sealed class Rule : List<Phrase> { }

    /// <summary>
    /// Represents a phrase, which is a sequence of <see cref="Symbol"/>s.
    /// </summary>
    public sealed class Phrase : List<Symbol> { }

    /// <summary>
    /// Represents a symbol, which can be either a <see cref="RuleReferenceSymbol"/> or a <see cref="TokenTypeSymbol"/>.
    /// </summary>
    public abstract class Symbol
    {
        protected Symbol(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }

        public int Column { get; }
    }

    /// <summary>
    /// Represents a symbol that refers to another <see cref="Rule"/>.
    /// </summary>
    public sealed class RuleReferenceSymbol : Symbol
    {
        public RuleReferenceSymbol(string ruleName, int line, int column)
            : base(line, column)
        {
            RuleName = ruleName;
        }

        public string RuleName { get; }
    }

    /// <summary>
    /// Represents a symbol that refers to a <see cref="Parsing.TokenType"/> value.
    /// </summary>
    public sealed class TokenTypeSymbol : Symbol
    {
        public TokenTypeSymbol(TokenType tokenType, int line, int column)
            : base(line, column)
        {
            TokenType = tokenType;
        }

        public TokenType TokenType { get; }
    }
}