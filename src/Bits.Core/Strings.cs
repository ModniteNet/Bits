using System.Collections.Generic;

namespace Bits.Core
{
    public static class Strings
    {
        public static readonly IReadOnlyDictionary<string, string> ErrorMessages =
            new Dictionary<string, string>
            {
                // Grammar
                ["grammar_invalid"] = "This line does not contain a valid grammar rule.",
                ["grammar_invalid_token_type"] = "This rule contains an invalid token.",
                ["grammar_nonexistent_rule"] = "This rule name does not exist in the grammar definition.",
                ["grammar_rule_exists"] = "There is already a rule with this name in the grammar.",
                ["grammar_unsupported_version"] = "There is no support for the specified language version."
            };
    }
}
