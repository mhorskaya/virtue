using System;

namespace Virtue
{
    internal class Compiler
    {
        private static readonly Scanner Scanner = Scanner.Instance;
        private static readonly Parser Parser = Parser.Instance;

        public static bool Compile(string source, Chunk chunk)
        {
            Scanner.InitScanner(source);

            Parser.HadError = false;
            Parser.PanicMode = false;

            Advance();
            //Expression();
            Consume(TokenType.Eof, "Expect end of expression.");
            return !Parser.HadError;
        }

        private static void Advance()
        {
            Parser.Previous = Parser.Current;

            while (true)
            {
                Parser.Current = Scanner.ScanToken();
                if (Parser.Current.Type != TokenType.Error) break;

                ErrorAtCurrent(Parser.Current.Lexeme);
            }
        }

        private static void Consume(TokenType type, string message)
        {
            if (Parser.Current.Type == type)
            {
                Advance();
                return;
            }

            ErrorAtCurrent(message);
        }

        private static void ErrorAtCurrent(string message)
        {
            ErrorAt(Parser.Current, message);
        }

        private static void Error(string message)
        {
            ErrorAt(Parser.Previous, message);
        }

        private static void ErrorAt(Token token, string message)
        {
            if (Parser.PanicMode) return;
            Parser.PanicMode = true;

            Console.Write($"[line {token.Line}] Error");

            if (token.Type == TokenType.Eof)
            {
                Console.Write(" at end");
            }
            else if (token.Type == TokenType.Error)
            {
                // Nothing.
            }
            else
            {
                Console.Write($" at '{token.Lexeme}'");
            }

            Console.WriteLine($": {message}");
            Parser.HadError = true;
        }
    }
}