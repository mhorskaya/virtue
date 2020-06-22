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

        public string Source { get; private set; }
        public int Start { get; set; }
        public int Current { get; set; }
        public int Line { get; set; }

        public void InitScanner(string source)
        {
            Source = source;
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
            if (IsAlpha(c)) return Identifier();
            if (IsDigit(c)) return Number();

            return c switch
            {
                '(' => MakeToken(TokenType.LeftParen),
                ')' => MakeToken(TokenType.RightParen),
                '{' => MakeToken(TokenType.LeftBrace),
                '}' => MakeToken(TokenType.RightBrace),
                ';' => MakeToken(TokenType.Semicolon),
                ',' => MakeToken(TokenType.Comma),
                '.' => MakeToken(TokenType.Dot),
                '-' => MakeToken(TokenType.Minus),
                '+' => MakeToken(TokenType.Plus),
                '/' => MakeToken(TokenType.Slash),
                '*' => MakeToken(TokenType.Star),
                '!' => MakeToken(Match('=') ? TokenType.BangEqual : TokenType.Bang),
                '=' => MakeToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal),
                '<' => MakeToken(Match('=') ? TokenType.LessEqual : TokenType.Less),
                '>' => MakeToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater),
                '"' => String(),
                _ => ErrorToken("Unexpected character.")
            };
        }

        private Token Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            return MakeToken(IdentifierType());
        }

        private TokenType IdentifierType()
        {
            switch (Source[Start])
            {
                case 'a': return CheckKeyword(1, 2, "nd", TokenType.And);
                case 'c': return CheckKeyword(1, 4, "lass", TokenType.Class);
                case 'e': return CheckKeyword(1, 3, "lse", TokenType.Else);
                case 'i': return CheckKeyword(1, 1, "f", TokenType.If);
                case 'n': return CheckKeyword(1, 2, "il", TokenType.Nil);
                case 'o': return CheckKeyword(1, 1, "r", TokenType.Or);
                case 'p': return CheckKeyword(1, 4, "rint", TokenType.Print);
                case 'r': return CheckKeyword(1, 5, "eturn", TokenType.Return);
                case 's': return CheckKeyword(1, 4, "uper", TokenType.Super);
                case 'v': return CheckKeyword(1, 2, "ar", TokenType.Var);
                case 'w': return CheckKeyword(1, 4, "hile", TokenType.While);
                case 'f':
                    if (Current - Start > 1)
                    {
                        switch (Source[Start + 1])
                        {
                            case 'a': return CheckKeyword(2, 3, "lse", TokenType.False);
                            case 'o': return CheckKeyword(2, 1, "r", TokenType.For);
                            case 'u': return CheckKeyword(2, 1, "n", TokenType.Fun);
                        }
                    }
                    break;

                case 't':
                    if (Current - Start > 1)
                    {
                        switch (Source[Start + 1])
                        {
                            case 'h': return CheckKeyword(2, 2, "is", TokenType.This);
                            case 'r': return CheckKeyword(2, 2, "ue", TokenType.True);
                        }
                    }
                    break;
            }

            return TokenType.Identifier;
        }

        private TokenType CheckKeyword(int start, int length, string rest, TokenType type)
        {
            if (Current - Start == start + length && Source.Substring(Start + start, length) == rest)
            {
                return type;
            }

            return TokenType.Identifier;
        }

        private Token Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            return MakeToken(TokenType.Number);
        }

        private Token String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd()) return ErrorToken("Unterminated string.");

            Advance();
            return MakeToken(TokenType.String);
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : Source[Current];
        }

        private char PeekNext()
        {
            return Current + 1 >= Source.Length ? '\0' : Source[Current + 1];
        }

        private char Advance()
        {
            return Source[++Current - 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source[Current] != expected) return false;
            Current++;
            return true;
        }

        private bool IsAtEnd()
        {
            return Current == Source.Length;
        }

        private static bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private Token MakeToken(TokenType type)
        {
            var token = new Token
            {
                Type = type,
                Lexeme = Source[Start..Current],
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
    }
}