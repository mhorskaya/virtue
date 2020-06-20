namespace Virtue
{
    internal class Virtue
    {
        private static void Main(string[] args)
        {
            var vm = Vm.Instance;

            var chunk = new Chunk();

            var constant = chunk.AddConstant(1.2);
            chunk.WriteChunk((byte)OpCode.Constant, 123);
            chunk.WriteChunk(constant, 123);

            constant = chunk.AddConstant(3.4);
            chunk.WriteChunk((byte)OpCode.Constant, 123);
            chunk.WriteChunk(constant, 123);

            chunk.WriteChunk((byte)OpCode.Add, 123);

            constant = chunk.AddConstant(5.6);
            chunk.WriteChunk((byte)OpCode.Constant, 123);
            chunk.WriteChunk(constant, 123);

            chunk.WriteChunk((byte)OpCode.Divide, 123);

            chunk.WriteChunk((byte)OpCode.Negate, 123);
            chunk.WriteChunk((byte)OpCode.Return, 123);

            Debug.DisassembleChunk(chunk, "Test Chunk");

            vm.Interpret(chunk);
        }
    }
}