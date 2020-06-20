using System;

namespace Virtue
{
    internal class Debug
    {
        public static void DisassembleChunk(Chunk chunk, string name)
        {
            Console.WriteLine($"== {name} ==");

            for (var offset = 0; offset < chunk.Code.Count;)
                offset = DisassembleInstruction(chunk, offset);
        }

        public static int DisassembleInstruction(Chunk chunk, int offset)
        {
            Console.Write($"{offset:D4} ");

            if (offset > 0 && chunk.Lines[offset] == chunk.Lines[offset - 1])
            {
                Console.Write("   | ");
            }
            else
            {
                Console.Write($"{chunk.Lines[offset]:D4} ");
            }

            var instruction = (OpCode)chunk.Code[offset];
            switch (instruction)
            {
                case OpCode.Constant:
                    return ConstantInstruction("CONSTANT", chunk, offset);

                case OpCode.Add:
                    return SimpleInstruction("ADD", offset);

                case OpCode.Subtract:
                    return SimpleInstruction("SUBTRACT", offset);

                case OpCode.Multiply:
                    return SimpleInstruction("MULTIPLY", offset);

                case OpCode.Divide:
                    return SimpleInstruction("DIVIDE", offset);

                case OpCode.Negate:
                    return SimpleInstruction("NEGATE", offset);

                case OpCode.Return:
                    return SimpleInstruction("RETURN", offset);

                default:
                    Console.WriteLine($"Unknown OpCode {instruction}");
                    return offset + 1;
            }
        }

        private static int ConstantInstruction(string name, Chunk chunk, int offset)
        {
            var index = chunk.Code[offset + 1];
            Console.Write($"{name,-16} {index:D4} '");
            PrintValue(chunk.Constants[index]);
            Console.WriteLine();
            return offset + 2;
        }

        private static int SimpleInstruction(string name, int offset)
        {
            Console.WriteLine(name);
            return offset + 1;
        }

        public static void PrintValue(double value)
        {
            Console.Write($"{value:g} ");
        }
    }
}