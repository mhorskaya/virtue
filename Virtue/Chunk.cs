using System.Collections.Generic;

namespace Virtue
{
    internal class Chunk
    {
        public List<OpCode> Code { get; }

        public Chunk(List<OpCode> code)
        {
            Code = code;
        }
    }
}