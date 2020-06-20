namespace Virtue
{
    internal class Virtue
    {
        private static void Main(string[] args)
        {
            var vm = Vm.Instance;

            var chunk = new Chunk();
            var constantIndex = chunk.AddConstant(1.2);
            chunk.WriteChunk((byte)OpCode.Constant, 123);
            chunk.WriteChunk(constantIndex, 123);
            chunk.WriteChunk((byte)OpCode.Negate, 123);
            chunk.WriteChunk((byte)OpCode.Return, 123);
            Debug.DisassembleChunk(chunk, "Test Chunk");

            vm.Interpret(chunk);
        }
    }
}