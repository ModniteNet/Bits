// Modnite Bits - Copyright (c) 2019 wumbo

using System.Collections.Generic;

namespace Bits.Core.Schema.AST
{
    public abstract class SchemaNode
    {
        private List<SchemaNode> _children = new List<SchemaNode>();

        public SchemaNode Parent { get; private set; }

        public void AddChild(SchemaNode node)
        {
            node.Parent = this;
            _children.Add(node);
        }
    }

    public class RootNode : SchemaNode
    {
    }

    public class ImportNode : SchemaNode
    {
        public string SchemaName { get; set; }
    }

    public class VariableNode : SchemaNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class BlockNode : SchemaNode
    {
        public string Name { get; set; }
        public bool IsMain { get; set; }
    }

    public class EnumNode : SchemaNode
    {
        public string Name { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    public enum PropertyType
    {
        Null,
        SignedInteger,
        UnsignedInteger,
        Block
        //Character,
        //CharacterArray,
        //SignedIntegerArray,
        //UnsignedIntegerArray
    }

    public class PropertyDefinitionNode : SchemaNode
    {
        public string Name { get; set; }
        public PropertyType PropertyType { get; set; }
        public string BlockName { get; set; }
        public int? IntegerBitSize { get; set; }
        public string CastTo { get; set; }
    }
}