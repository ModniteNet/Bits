using Bits.Core.BitX.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Bits.Test.BitX
{
    public class TokenizerTests
    {
        /// <summary>
        /// Passes only when each token type is detected in the sample script.
        /// </summary>
        [Fact]
        public void TokenizeScript_Version1()
        {
            var tokenTypes = new List<TokenType>(Enum.GetValues(typeof(TokenType)).Cast<TokenType>());
            tokenTypes.Remove(TokenType.Undefined);

            string script = File.ReadAllText(@"./Resources/SampleScript-Version1.bitx");

            var tokens = Tokenizer.Tokenize(script);
            foreach (Token token in tokens)
            {
                tokenTypes.Remove(token.TokenType);
            }

            Assert.Empty(tokenTypes);
        }
    }
}
