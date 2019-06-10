using Bits.Core.BitX.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bits.Core.BitX.AST
{
    class Generator
    {
        static void Main(string[] args)
        {
            var compiler = CSharpCompilation.Create("Bits.Core.BitX.AST")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));

            foreach (int version in Grammar.GrammarFiles.Keys)
            {
                var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Bits.Core.BitX.AST.V" + version))
                    .NormalizeWhitespace()
                    .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Bits.Core.BitX.Parsing")));

                var grammar = new Grammar(version);

                foreach (var ruleSet in grammar)
                {
                    string className = ruleSet.Key.Substring(1, ruleSet.Key.Length - 2);

                    var @class = SyntaxFactory.ClassDeclaration(className)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.SealedKeyword));

                    var methods = CreateRuleMethods(ruleSet.Value);
                    foreach (var method in methods)
                    {
                        @class = @class.AddMembers(method);
                    }

                    @namespace = @namespace.AddMembers(@class);
                }

                string code = @namespace.NormalizeWhitespace().ToFullString();
                compiler.AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
                Console.WriteLine(code);
            }

            compiler.Emit("Bits.Core.BitX.AST.dll");
        }

        private static IEnumerable<MethodDeclarationSyntax> CreateRuleMethods(Parsing.RuleSet ruleSet)
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "Accept")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("tokens")).WithType(SyntaxFactory.ParseTypeName("Queue<Token>")));

            var ruleMethodNames = new List<string>();

            for (int i = 0; i < ruleSet.Count; i++)
            {
                string methodName = "Accept_" + i;
                ruleMethodNames.Add(methodName);

                var ruleMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), methodName)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("tokens")).WithType(SyntaxFactory.ParseTypeName("Queue<Token>")));

                StatementSyntax statement;

                if (ruleSet[i].Count == 0)
                {
                    statement = SyntaxFactory.ParseStatement("return true;");
                }
                else
                {
                    string GetStatement(Queue<Symbol> symbols)
                    {
                        if (symbols.Count == 0)
                        {
                            return "return true;";
                        }
                        else
                        {
                            Symbol symbol = symbols.Dequeue();
                            if (symbol is RuleReferenceSymbol)
                            {
                                string ruleName = (symbol as RuleReferenceSymbol).RuleName;
                                ruleName = ruleName.Substring(1, ruleName.Length - 2);
                                return $"if ({ruleName}.Accept(new Queue<Token>(tokens))) {{ {ruleName}.Accept(tokens); {GetStatement(symbols)} }} else {{ return false; }}";
                            }
                            else
                            {
                                return $"if (tokens.Peek().TokenType == TokenType.{(symbol as TokenTypeSymbol).TokenType}) {{ tokens.Dequeue(); {GetStatement(symbols)} }} else {{ return false; }}";
                            }
                        }
                    }

                    statement = SyntaxFactory.ParseStatement(GetStatement(new Queue<Symbol>(ruleSet[i])));
                }

                ruleMethod = ruleMethod.AddBodyStatements(statement);
                yield return ruleMethod;
            }

            foreach (var ruleMethodName in ruleMethodNames)
            {
                method = method.AddBodyStatements(SyntaxFactory.ParseStatement($"if ({ruleMethodName}(new Queue<Token>(tokens))) {{ {ruleMethodName}(tokens); return true; }}"));
            }
            method = method.AddBodyStatements(SyntaxFactory.ParseStatement("return false;"));

            yield return method;
        }
    }
}
