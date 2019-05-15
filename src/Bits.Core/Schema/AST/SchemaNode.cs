// Modnite Bits - Copyright (c) 2019 wumbo

using System.Collections.Generic;

namespace Bits.Core.Schema.AST
{
    public static class SchemaSyntaxTree
    {
        public static SchemaNode Parse(SchemaToken[] tokens)
        {
            var root = new RootNode();
            var stack = new Stack<SchemaNode>();
            stack.Push(root);
            SchemaNode nextNode = null;
            SchemaNode currentNode;
            bool scopeToNext = false;
            bool scopeToPrevious = false;
            for (int i = 0; i < tokens.Length; i++)
            {
                currentNode = stack.Peek();

                if (tokens[i].Value.StartsWith("int") || tokens[i].Value.StartsWith("uint"))
                {
                    bool isUnsigned = tokens[i].Value[0] == 'u';

                    if (!int.TryParse(tokens[i].Value.Substring(isUnsigned ? 4 : 3), out int bitSize))
                        throw new SchemaSyntaxErrorException(tokens[i]);

                    string name = tokens[++i].Value;
                    string castTo = null;
                    if (tokens[i + 1].Value == "as")
                    {
                        castTo = tokens[i + 2].Value;
                        i += 2;
                    }

                    nextNode = new PropertyDefinitionNode
                    {
                        Name = name,
                        PropertyType = isUnsigned ? PropertyType.UnsignedInteger : PropertyType.SignedInteger,
                        IntegerBitSize = bitSize,
                        CastTo = castTo
                    };
                }
                else
                {
                    switch (tokens[i].Value)
                    {
                        case "null":
                            nextNode = new PropertyDefinitionNode
                            {
                                Name = tokens[++i].Value,
                                PropertyType = PropertyType.Null
                            };
                            break;

                        case "open":
                            nextNode = new ImportNode
                            {
                                SchemaName = tokens[++i].Value
                            };
                            break;

                        case "enum":
                            nextNode = new EnumNode
                            {
                                Name = tokens[++i].Value
                            };
                            scopeToNext = true;
                            break;

                        case "block":
                            string name = tokens[++i].Value;
                            bool isMain = (name == "main");
                            if (isMain)
                            {
                                name = tokens[++i].Value;
                            }
                            nextNode = new BlockNode
                            {
                                Name = name,
                                IsMain = isMain
                            };
                            scopeToNext = true;
                            break;

                        case "{":
                            switch (currentNode)
                            {
                                case EnumNode _:
                                case BlockNode _:
                                    continue;
                            }
                            break;

                        case "}":
                            scopeToPrevious = true;
                            nextNode = null;
                            if (stack.Count == 1)
                            {
                                throw new SchemaSyntaxErrorException(tokens[i]);
                            }
                            break;

                        default:
                            if (tokens[i + 1].Value == ":") // Variable declaration
                            {
                                nextNode = new VariableNode
                                {
                                    Name = tokens[i].Value,
                                    Value = tokens[i + 2].Value
                                };
                                i += 2;
                            }
                            else
                            {
                                if (tokens[i + 1].Value != ":")
                                {
                                    nextNode = new PropertyDefinitionNode
                                    {
                                        PropertyType = PropertyType.Block,
                                        BlockName = tokens[i].Value,
                                        Name = tokens[++i].Value
                                    };
                                }
                                else
                                {
                                    throw new SchemaSyntaxErrorException(tokens[i + 1]);
                                }
                            }
                            break;
                    }
                }

                if (nextNode != null)
                {
                    currentNode.AddChild(nextNode);
                    if (scopeToNext)
                    {
                        stack.Push(nextNode);
                        scopeToNext = false;
                    }
                }
                else if (scopeToPrevious)
                {
                    stack.Pop();
                    scopeToPrevious = false;
                }
            }
            return root;
        }
    }
}