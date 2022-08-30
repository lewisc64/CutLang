using CutLang.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CutLang
{
    public class Lexer
    {
        private string _input;

        private int _position = 0;

        private char CurrentChar => _input[_position];

        private bool AtEndOfFile => _position >= _input.Length;

        public Lexer(string input)
        {
            _input = input.Trim();
        }

        public IEnumerable<IToken> Tokenise()
        {
            while (!AtEndOfFile)
            {
                yield return GetNextToken();
            }
        }

        public IToken GetNextToken()
        {
            while (true)
            {
                if (IsNumber(CurrentChar))
                {
                    return ParseNumerical();
                }

                if (CurrentChar == '-')
                {
                    NextChar();
                    return new SpanToken();
                }

                if (CurrentChar == '+')
                {
                    NextChar();
                    return new ConcatToken();
                }

                if (CurrentChar == 'E')
                {
                    NextChar();
                    if (CurrentChar == 'N')
                    {
                        NextChar();
                        if (CurrentChar == 'D')
                        {
                            NextChar();
                            return new EndOfVideoToken();
                        }
                        throw new LexerException($"Expected 'D' as part of 'END' at position {_position}, instead got '{CurrentChar}'.");
                    }
                    throw new LexerException($"Expected 'N' as part of 'END' at position {_position}, instead got '{CurrentChar}'.");
                }

                if (CurrentChar == 'S')
                {
                    NextChar();
                    if (CurrentChar == 'T')
                    {
                        NextChar();
                        if (CurrentChar == 'A')
                        {
                            NextChar();
                            if (CurrentChar == 'R')
                            {
                                NextChar();
                                if (CurrentChar == 'T')
                                {
                                    NextChar();
                                    return new TimestampToken(0, 0, 0);
                                }
                                throw new LexerException($"Expected 'T' as part of 'START' at position {_position}, instead got '{CurrentChar}'.");
                            }
                            throw new LexerException($"Expected 'R' as part of 'START' at position {_position}, instead got '{CurrentChar}'.");
                        }
                        throw new LexerException($"Expected 'A' as part of 'START' at position {_position}, instead got '{CurrentChar}'.");
                    }
                    throw new LexerException($"Expected 'T' as part of 'START' at position {_position}, instead got '{CurrentChar}'.");
                }

                if (CurrentChar == '(')
                {
                    NextChar();
                    return new OpenBracketToken();
                }

                if (CurrentChar == ')')
                {
                    NextChar();
                    return new CloseBracketToken();
                }

                if (CurrentChar == '>')
                {
                    NextChar();
                    if (CurrentChar == '>')
                    {
                        NextChar();
                        return new SpeedUpToken();
                    }
                    throw new LexerException($"Expected '>' as part of '>>' at position {_position}, instead got '{CurrentChar}'.");
                }

                if (CurrentChar == '<')
                {
                    NextChar();
                    if (CurrentChar == '<')
                    {
                        NextChar();
                        return new SlowDownToken();
                    }
                    throw new LexerException($"Expected '<' as part of '<<' at position {_position}, instead got '{CurrentChar}'.");
                }

                if (CurrentChar == ' ')
                {
                    NextChar();
                    continue;
                }

                throw new LexerException($"Unexpected character at position {_position}: '{CurrentChar}'");
            }
        }

        private IToken ParseNumerical()
        {
            var builderStack = new Stack<StringBuilder>();
            builderStack.Push(new StringBuilder());
            builderStack.Peek().Append(CurrentChar);

            var seenDecimalPoint = false;

            while (true)
            {
                NextChar();

                if (AtEndOfFile)
                {
                    break;
                }

                if (IsNumber(CurrentChar))
                {
                    builderStack.Peek().Append(CurrentChar);
                }
                else if (CurrentChar == '.')
                {
                    if (seenDecimalPoint)
                    {
                        throw new LexerException($"Repeated decimal point in number at position {_position}");
                    }
                    seenDecimalPoint = true;
                    builderStack.Peek().Append(CurrentChar);
                }
                else if (CurrentChar == ':')
                {
                    builderStack.Push(new StringBuilder());
                    seenDecimalPoint = false;
                }
                else
                {
                    break;
                }
            }

            if (builderStack.Count == 1)
            {
                var text = builderStack.Single().ToString();
                return new NumberToken(double.Parse(text));
            }

            double seconds = double.Parse(builderStack.Pop().ToString());
            int minutes = int.Parse(builderStack.Pop().ToString());
            var hours = 0;

            if (builderStack.Any())
            {
                hours = int.Parse(builderStack.Pop().ToString());
            }

            if (builderStack.Any())
            {
                throw new LexerException($"Too many segments in timestamp at position {_position}");
            }

            return new TimestampToken(hours, minutes, seconds);
        }

        private bool IsNumber(char c)
        {
            return Enumerable.Range(0, 10).Select(x => x.ToString()[0]).Contains(c);
        }

        private void NextChar()
        {
            if (AtEndOfFile)
            {
                throw new LexerException("Unexpected end of input.");
            }
            _position++;
        }

        public class LexerException : Exception
        {
            public LexerException(string message)
                : base(message)
            {
            }
        }
    }
}
