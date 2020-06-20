using System;
using System.Collections.Generic;

namespace Virtue
{
    internal class Chunk
    {
        private readonly List<byte> _code;
        private readonly List<double> _constants;

        public int CodeCount => _code.Count;

        public byte GetCodeAt(int index) => _code[index];

        public double GetConstantAt(int index) => _constants[index];

        public Chunk()
        {
            _code = new List<byte>();
            _constants = new List<double>();
        }

        public void WriteChunk(byte code)
        {
            _code.Add(code);
        }

        public byte AddConstant(double constant)
        {
            if (_constants.Count > byte.MaxValue)
            {
                throw new Exception("Constant pool is overflowed.");
            }

            _constants.Add(constant);
            return (byte)(_constants.Count - 1);
        }
    }
}