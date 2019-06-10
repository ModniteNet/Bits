using Bits.Core.BitX.Parsing.Syntax;

namespace Bits.Core.BitX.Parsing
{
    public static class Parser
    {
        public static bool TryParse(TokenStack tokens, out Script script)
        {
            var scriptSyntax = new ScriptSyntax();

            if (scriptSyntax.TryParse(tokens, out GraphNode scriptNode))
            {
                script = (Script)scriptNode;
                return true;
            }

            script = null;
            return false;
        }
    }
}