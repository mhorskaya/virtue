using System.Collections.Generic;

namespace Virtue
{
    internal class Virtue
    {
        private static void Main(string[] args)
        {
            var code = new List<OpCode> { OpCode.Return };
            var chunk = new Chunk(code);
            Debug.DisassembleChunk(chunk, "Test Chunk");
        }
    }
}