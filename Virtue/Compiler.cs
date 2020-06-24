using System;

namespace Virtue
{
    internal class Compiler
    {
        private static readonly Scanner Scanner = Scanner.Instance;
        private static readonly Parser Parser = Parser.Instance;
        private static Chunk _compilingChunk;

        private static readonly ParseRule[] Rules = {
            new ParseRule(Grouping, null  , Precedence.None),   // LeftParen
            new ParseRule(null    , null  , Precedence.None),   // RightParen
            new ParseRule(null    , null  , Precedence.None),   // LeftBrace
            new ParseRule(null    , null  , Precedence.None),   // RightBrace
            new ParseRule(null    , null  , Precedence.None),   // Comma
            new ParseRule(null    , null  , Precedence.None),   // Dot
            new ParseRule(Unary   , Binary, Precedence.Term),   // Minus
            new ParseRule(null    , Binary, Precedence.Term),   // Plus
            new ParseRule(null    , null,   Precedence.None),   // Semicolon
            new ParseRule(null    , Binary, Precedence.Factor), // Slash
            new ParseRule(null    , Binary, Precedence.Factor), // Star
            new ParseRule(null    , null,   Precedence.None),   // Bang
            new ParseRule(null    , null,   Precedence.None),   // BangEqual
            new ParseRule(null    , null,   Precedence.None),   // Equal
            new ParseRule(null    , null,   Precedence.None),   // EqualEqual
            new ParseRule(null    , null,   Precedence.None),   // Greater
            new ParseRule(null    , null,   Precedence.None),   // GreaterEqual
            new ParseRule(null    , null,   Precedence.None),   // Less
            new ParseRule(null    , null,   Precedence.None),   // LessEqual
            new ParseRule(null    , null,   Precedence.None),   // Identifier
            new ParseRule(null    , null,   Precedence.None),   // String
            new ParseRule(Number  , null,   Precedence.None),   // Number
            new ParseRule(null    , null,   Precedence.None),   // And
            new ParseRule(null    , null,   Precedence.None),   // Class
            new ParseRule(null    , null,   Precedence.None),   // Else
            new ParseRule(null    , null,   Precedence.None),   // False
            new ParseRule(null    , null,   Precedence.None),   // For
            new ParseRule(null    , null,   Precedence.None),   // Fun
            new ParseRule(null    , null,   Precedence.None),   // If
            new ParseRule(null    , null,   Precedence.None),   // Nil
            new ParseRule(null    , null,   Precedence.None),   // Or
            new ParseRule(null    , null,   Precedence.None),   // Print
            new ParseRule(null    , null,   Precedence.None),   // Return
            new ParseRule(null    , null,   Precedence.None),   // Super
            new ParseRule(null    , null,   Precedence.None),   // This
            new ParseRule(null    , null,   Precedence.None),   // True
            new ParseRule(null    , null,   Precedence.None),   // Var
            new ParseRule(null    , null,   Precedence.None),   // While
            new ParseRule(null    , null,   Precedence.None),   // Error
            new ParseRule(null    , null,   Precedence.None),   // Eof
        };

        public static bool Compile(string source, Chunk chunk)
        {
            Scanner.InitScanner(source);
            _compilingChunk = chunk;
            Parser.HadError = false;
            Parser.PanicMode = false;

            Advance();
            Expression();
            Consume(TokenType.Eof, "Expect end of expression.");
            EndCompiler();
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

        private static Chunk CurrentChunk()
        {
            return _compilingChunk;
        }

        private static void EmitByte(byte @byte)
        {
            CurrentChunk().WriteChunk(@byte, Parser.Previous.Line);
        }

        private static void EmitBytes(byte byte1, byte byte2)
        {
            EmitByte(byte1);
            EmitByte(byte2);
        }

        private static void EndCompiler()
        {
            EmitReturn();
        }

        private static void Grouping()
        {
            Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
        }

        private static void Number()
        {
            var value = double.Parse(Parser.Previous.Lexeme);
            EmitConstant(value);
        }

        private static void Binary()
        {
            var operatorType = Parser.Previous.Type;

            var rule = GetRule(operatorType);
            ParsePrecedence(rule.Precedence + 1);

            switch (operatorType)
            {
                case TokenType.Plus: EmitByte((byte)OpCode.Add); break;
                case TokenType.Minus: EmitByte((byte)OpCode.Subtract); break;
                case TokenType.Star: EmitByte((byte)OpCode.Multiply); break;
                case TokenType.Slash: EmitByte((byte)OpCode.Divide); break;
                default:
                    return; // Unreachable.
            }
        }

        private static void Unary()
        {
            var operatorType = Parser.Previous.Type;

            Expression();
            ParsePrecedence(Precedence.Unary);
            switch (operatorType)
            {
                case TokenType.Minus: EmitByte((byte)OpCode.Negate); break;
                default: return; // Unreachable.
            }
        }

        private static void Expression()
        {
            ParsePrecedence(Precedence.Assignment);
        }

        private static void ParsePrecedence(Precedence precedence)
        {
            Advance();
            var prefixRule = GetRule(Parser.Previous.Type).Prefix;
            if (prefixRule == null)
            {
                Error("Expect expression.");
                return;
            }

            prefixRule();

            while (precedence <= GetRule(Parser.Current.Type).Precedence)
            {
                Advance();
                var infixRule = GetRule(Parser.Previous.Type).Infix;
                infixRule();
            }
        }

        private static ParseRule GetRule(TokenType type)
        {
            return Rules[(int)type];
        }

        private static void EmitReturn()
        {
            EmitByte((byte)OpCode.Return);
        }

        private static byte MakeConstant(double value)
        {
            var constant = CurrentChunk().AddConstant(value);
            if (constant > byte.MaxValue)
            {
                Error("Too many constants in one chunk.");
                return 0;
            }

            return (byte)constant;
        }

        private static void EmitConstant(double value)
        {
            EmitBytes((byte)OpCode.Constant, MakeConstant(value));
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