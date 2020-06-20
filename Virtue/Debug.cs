using System;

namespace Virtue
{
    internal class Debug
    {
        public static void DisassembleChunk(Chunk chunk, string name)
        {
            Console.WriteLine($"== {name} ==");

            for (var offset = 0; offset < chunk.CodeCount;)
            {
                offset = DisassembleInstruction(chunk, offset);
            }
        }

        private static int DisassembleInstruction(Chunk chunk, int offset)
        {
            Console.Write($"{offset:D4} ");

            if (offset > 0 && chunk.GetLineAt(offset) == chunk.GetLineAt(offset - 1))
            {
                Console.Write("   | ");
            }
            else
            {
                Console.Write($"{chunk.GetLineAt(offset):D4} ");
            }

            var instruction = (OpCode)chunk.GetCodeAt(offset);
            switch (instruction)
            {
                case OpCode.Constant:
                    return ConstantInstruction("CONSTANT", chunk, offset);

                case OpCode.Return:
                    return SimpleInstruction("RETURN", offset);

                default:
                    Console.WriteLine($"Unknown OpCode {instruction}");
                    return offset + 1;
            }
        }

        private static int ConstantInstruction(string name, Chunk chunk, int offset)
        {
            var constantIndex = chunk.GetCodeAt(offset + 1);
            Console.Write($"{name,-16} {constantIndex:D4} '");
            PrintValue(chunk.GetConstantAt(constantIndex));
            Console.WriteLine();
            return offset + 2;
        }

        private static int SimpleInstruction(string name, int offset)
        {
            Console.WriteLine(name);
            return offset + 1;
        }

        private static void PrintValue(double value)
        {
            Console.Write($"{value:g} ");
        }
    }
}