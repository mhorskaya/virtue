using System;

namespace Virtue
{
    internal class Compiler
    {
        private static readonly Scanner Scanner = Scanner.Instance;

        public static void Compile(string source)
        {
            Scanner.InitScanner(source);

            var line = -1;
            while (true)
            {
                var token = Scanner.ScanToken();
                if (token.Line != line)
                {
                    Console.Write($"{token.Line:D4} ");
                    line = token.Line;
                }
                else
                {
                    Console.Write("   | ");
                }

                Console.WriteLine($"{(int)token.Type:D2} '{token.Lexeme}'");

                if (token.Type == TokenType.Eof) break;
            }
        }
    }
}