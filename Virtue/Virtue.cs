namespace Virtue
{
    internal class Virtue
    {
        private static void Main(string[] args)
        {
            var chunk = new Chunk();
            var constantIndex = chunk.AddConstant(1.2);
            chunk.WriteChunk((byte)OpCode.Constant);
            chunk.WriteChunk(constantIndex);
            chunk.WriteChunk((byte)OpCode.Return);
            Debug.DisassembleChunk(chunk, "Test Chunk");
        }
    }
}