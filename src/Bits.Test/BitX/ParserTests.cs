using Bits.Core.BitX;
using Bits.Core.BitX.Parsing;
using System.IO;
using Xunit;

namespace Bits.Test.BitX
{
    public class ParserTests
    {
        [Fact]
        public void ParseScript()
        {
            const string scriptPath = @"./Resources/SampleScript-Version1.bitx";
            string scriptText = File.ReadAllText(scriptPath);

            var tokens = new TokenStack(scriptPath, Tokenizer.Tokenize(scriptText));

            bool isSyntaxValid = Parser.TryParse(tokens, out Script script);
            Assert.True(isSyntaxValid);
        }
    }
}