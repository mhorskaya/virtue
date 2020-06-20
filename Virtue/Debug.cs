using System;

namespace Virtue
{
    internal class Debug
    {
        public static void DisassembleChunk(Chunk chunk, string name)
        {
            Console.WriteLine($"== {name} ==");

            for (var offset = 0; offset < chunk.Code.Count;)
            {
                offset = DisassembleInstruction(chunk, offset);
            }
        }

        private static int DisassembleInstruction(Chunk chunk, int offset)
        {
            Console.Write($"{offset:D4} ");

            var instruction = chunk.Code[offset];
            switch (instruction)
            {
                case OpCode.Return:
                    return SimpleInstruction("Return", offset);

                default:
                    Console.WriteLine($"Unknown OpCode {instruction}");
                    return offset + 1;
            }
        }

        private static int SimpleInstruction(string name, int offset)
        {
            Console.WriteLine(name);
            return offset + 1;
        }
    }
}