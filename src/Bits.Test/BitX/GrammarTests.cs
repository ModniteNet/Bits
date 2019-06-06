using Bits.Core.BitX.Parsing;
using Xunit;

namespace Bits.Test.BitX
{
    public class GrammarTests
    {
        [Fact]
        public void LoadGrammar_Version1()
        {
            var grammar = new Grammar(version: 1);
        }
    }
}
