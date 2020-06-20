namespace Virtue
{
    internal class Virtue
    {
        private static void Main(string[] args)
        {
            var chunk = new Chunk();
            var constantIndex = chunk.AddConstant(1.2);
            chunk.WriteChunk((byte)OpCode.Constant, 123);
            chunk.WriteChunk(constantIndex, 123);
            chunk.WriteChunk((byte)OpCode.Return, 123);
            Debug.DisassembleChunk(chunk, "Test Chunk");
        }
    }
}