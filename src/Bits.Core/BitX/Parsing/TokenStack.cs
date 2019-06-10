using System;
using System.Collections.Generic;
using System.Linq;

namespace Bits.Core.BitX.Parsing
{
    public sealed class TokenStack
    {
        private Stack<Token> _stack;

        public TokenStack(string file, IEnumerable<Token> tokens)
        {
            File = file;
            _stack = new Stack<Token>(tokens.Reverse());
        }

        public int Count { get => _stack.Count; }

        public string File { get; }

        public bool Expect(TokenType type) => ExpectAny(type);

        public bool ExpectSequence(params TokenType[] sequence)
        {
            var acceptedTokens = new Stack<Token>();

            void Undo()
            {
                while (acceptedTokens.Count > 0)
                    _stack.Push(acceptedTokens.Pop());
            }

            foreach (TokenType type in sequence)
            {
                if (_stack.Count == 0 || _stack.Peek().TokenType != type)
                {
                    Undo();
                    return false;
                }

                acceptedTokens.Push(_stack.Pop());
            }

            Undo();
            return true;
        }

        public bool ExpectAny(params TokenType[] types)
        {
            if (_stack.Count == 0)
                return false;

            foreach (TokenType type in types)
            {
                if (_stack.Peek().TokenType == type)
                    return true;
            }

            return false;
        }

        public Token Pop() => _stack.Pop();

        public Token Peek() => _stack.Peek();

        public void Push(Token token) => _stack.Push(token);
    }
}