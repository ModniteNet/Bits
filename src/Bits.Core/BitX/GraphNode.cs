using Bits.Core.BitX.Parsing;
using System.Collections.Generic;

namespace Bits.Core.BitX
{
    public abstract class GraphNode
    {
        private List<GraphNode> _children = new List<GraphNode>();

        protected GraphNode(IEnumerable<Token> source)
        {
            Source = source;
        }

        public IEnumerable<Token> Source { get; }

        public IReadOnlyList<GraphNode> Children { get => _children; }

        public void AddChildNode(GraphNode node) => _children.Add(node);
    }
}