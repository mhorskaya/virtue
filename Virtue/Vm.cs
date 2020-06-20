#define DEBUG_TRACE_EXECUTION

using System;

namespace Virtue
{
    internal class Vm
    {
        private static readonly Lazy<Vm> Lazy =
            new Lazy<Vm>(() => new Vm());

        public static Vm Instance => Lazy.Value;

        public Chunk Chunk { get; set; }
        private int _ip;

        private Vm()
        {
        }

        private InterpretResult Interpret(Chunk chunk)
        {
            Chunk = chunk;
            _ip = 0;
            return Run();
        }

        private InterpretResult Run()
        {
            byte ReadByte() => Chunk.GetCodeAt(_ip++);
            double ReadConstant() => Chunk.GetConstantAt(ReadByte());

            while (true)
            {
#if DEBUG_TRACE_EXECUTION
                Debug.DisassembleInstruction(Chunk, _ip);
#endif
                OpCode instruction;
                switch (instruction = (OpCode)ReadByte())
                {
                    case OpCode.Constant:
                        var constant = ReadConstant();
                        Debug.PrintValue(constant);
                        Console.WriteLine();
                        break;

                    case OpCode.Return:
                        return InterpretResult.Ok;
                }
            }
        }
    }
}