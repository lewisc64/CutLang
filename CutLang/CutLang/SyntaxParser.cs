using CutLang.Token;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CutLang
{
    public class SyntaxParser
    {
        private static readonly Type[] Precedence = new[] { typeof(ConcatToken), typeof(SpanToken) };

        private IEnumerable<IToken> _tokens;

        public SyntaxParser(IEnumerable<IToken> tokens)
        {
            _tokens = tokens;
        }

        public Node MakeTree()
        {
            return MakeTree(_tokens);
        }

        public Node MakeTree(IEnumerable<IToken> tokens)
        {
            var debracketed = Debracket(tokens);

            if (debracketed.Count() == 1)
            {
                return new Node(debracketed.Single());
            }

            foreach (var tokenType in Precedence)
            {
                var bracketLevel = 0;
                for (var i = 0; i < debracketed.Count(); i++)
                {
                    var token = debracketed.ElementAt(i);

                    if (token is OpenBracketToken)
                    {
                        bracketLevel++;
                    }
                    else if (token is CloseBracketToken)
                    {
                        bracketLevel--;
                    }

                    if (bracketLevel > 0)
                    {
                        continue;
                    }

                    if (Precedence.Contains(token.GetType()) && token.GetType() != tokenType)
                    {
                        continue;
                    }

                    if (token is ConcatToken)
                    {
                        var left = MakeTree(debracketed.Take(i));
                        var right = MakeTree(debracketed.Skip(i + 1));

                        return new Node(token)
                        {
                            Left = left,
                            Right = right,
                        };
                    }

                    if (token is SpanToken)
                    {
                        var left = MakeTree(debracketed.Take(i));
                        var right = MakeTree(debracketed.Skip(i + 1));

                        if (left.Token is not TimestampToken)
                        {
                            throw new SyntaxParserException($"Expected {nameof(TimestampToken)} on the left side of {nameof(SpanToken)}, got {left.Token.GetType().Name}");
                        }
                        
                        if (right.Token is not TimestampToken && right.Token is not EndOfVideoToken)
                        {
                            throw new SyntaxParserException($"Expected {nameof(TimestampToken)} on the right side of {nameof(SpanToken)}, got {right.Token.GetType().Name}");

                        }

                        return new Node(token)
                        {
                            Left = left,
                            Right = right,
                        };
                    }
                }
            }

            throw new SyntaxParserException($"Not enough complexity.");
        }

        public static IEnumerable<IToken> Debracket(IEnumerable<IToken> tokens)
        {
            var output = new List<IToken>(tokens);
            while (output.First() is OpenBracketToken && output.Last() is CloseBracketToken)
            {
                var trimmed = output.Skip(1).Reverse().Skip(1).Reverse();
                var bracketLevel = 0;
                foreach (var token in trimmed)
                {
                    if (token is OpenBracketToken)
                    {
                        bracketLevel++;
                    }
                    else if (token is CloseBracketToken && bracketLevel > 0)
                    {
                        bracketLevel--;
                    }
                }
                if (bracketLevel == 0)
                {
                    output = new List<IToken>(trimmed);
                }
                else
                {
                    break;
                }
            }
            return output;
        }

        public class SyntaxParserException : Exception
        {
            public SyntaxParserException(string message)
                : base(message)
            {
            }
        }

        public class Node
        {
            public Node Left { get; set; }

            public Node Right { get; set; }

            public IToken Token { get; }

            public bool IsLeaf => Left == null && Right == null;

            public Node(IToken token)
            {
                Token = token;
            }

            public override string ToString()
            {
                return $"({Left}{Token.GetType().Name}{Right})";
            }
        }
    }
}
