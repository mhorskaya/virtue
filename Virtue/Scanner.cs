using System;

namespace Virtue
{
    internal class Scanner
    {
        private static readonly Lazy<Scanner> Lazy = new Lazy<Scanner>(() => new Scanner());
        public static Scanner Instance => Lazy.Value;

        private Scanner()
        {
        }

        private string _source;
        public int Start { get; set; }
        public int Current { get; set; }
        public int Line { get; set; }

        public void InitScanner(string source)
        {
            _source = source;
            Start = 0;
            Current = 0;
            Line = 1;
        }

        public Token ScanToken()
        {
            SkipWhitespace();

            Start = Current;

            if (IsAtEnd()) return MakeToken(TokenType.Eof);

            var c = Advance();

            switch (c)
            {
                case '(': return MakeToken(TokenType.LeftParen);
                case ')': return MakeToken(TokenType.RightParen);
                case '{': return MakeToken(TokenType.LeftBrace);
                case '}': return MakeToken(TokenType.RightBrace);
                case ';': return MakeToken(TokenType.Semicolon);
                case ',': return MakeToken(TokenType.Comma);
                case '.': return MakeToken(TokenType.Dot);
                case '-': return MakeToken(TokenType.Minus);
                case '+': return MakeToken(TokenType.Plus);
                case '/': return MakeToken(TokenType.Slash);
                case '*': return MakeToken(TokenType.Star);
                case '!': return MakeToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                case '=': return MakeToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                case '<': return MakeToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                case '>': return MakeToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
            }

            return ErrorToken("Unexpected character.");
        }

        private bool IsAtEnd()
        {
            return Current == _source.Length;
        }

        private char Advance()
        {
            return _source[Current++];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[Current] != expected) return false;
            Current++;
            return true;
        }

        private Token MakeToken(TokenType type)
        {
            var token = new Token
            {
                Type = type,
                Lexeme = _source[Start..Current],
                Line = Line
            };

            return token;
        }

        private Token ErrorToken(string message)
        {
            var token = new Token
            {
                Type = TokenType.Error,
                Lexeme = message,
                Line = Line
            };

            return token;
        }

        private void SkipWhitespace()
        {
            while (true)
            {
                var c = Peek();
                switch (c)
                {
                    case ' ':
                    case '\r':
                    case '\t':
                        Advance();
                        break;

                    case '\n':
                        Line++;
                        Advance();
                        break;

                    case '/':
                        if (PeekNext() == '/')
                        {
                            while (Peek() != '\n' && !IsAtEnd()) Advance();
                        }
                        else
                        {
                            return;
                        }
                        break;

                    default:
                        return;
                }
            }
        }

        private char Peek()
        {
            return _source[Current];
        }

        private char PeekNext()
        {
            return IsAtEnd() ? '\0' : _source[Current + 1];
        }
    }
}