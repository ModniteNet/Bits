namespace Bits.Core.BitX.Parsing.Syntax
{
    public abstract class Syntax
    {
        public abstract bool TryParse(TokenStack tokens, out GraphNode node);
    }
}