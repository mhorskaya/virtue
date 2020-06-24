using System.Collections.Generic;

namespace Virtue
{
    internal class Chunk
    {
        public readonly List<byte> Code = new List<byte>();
        public readonly List<double> Constants = new List<double>();
        public readonly List<int> Lines = new List<int>();

        public void WriteChunk(byte code, int line)
        {
            Code.Add(code);
            Lines.Add(line);
        }

        public int AddConstant(double constant)
        {
            Constants.Add(constant);
            return Constants.Count - 1;
        }
    }
}