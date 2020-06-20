#define DEBUG_TRACE_EXECUTION

using System;
using System.Collections.Generic;

namespace Virtue
{
    internal class Vm
    {
        private static readonly Lazy<Vm> Lazy =
            new Lazy<Vm>(() => new Vm());

        public static Vm Instance => Lazy.Value;

        public Chunk Chunk { get; private set; }
        private int _ip;
        private readonly Stack<double> _stack;

        private Vm()
        {
            _stack = new Stack<double>();
        }

        public InterpretResult Interpret(Chunk chunk)
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
                Console.Write("          ");
                foreach (var value in _stack)
                {
                    Console.Write("[ ");
                    Debug.PrintValue(value);
                    Console.Write(" ]");
                }
                Console.WriteLine();
                Debug.DisassembleInstruction(Chunk, _ip);
#endif
                OpCode instruction;
                switch (instruction = (OpCode)ReadByte())
                {
                    case OpCode.Constant:
                        var constant = ReadConstant();
                        _stack.Push(constant);
                        break;

                    case OpCode.Negate:
                        _stack.Push(-_stack.Pop());
                        break;

                    case OpCode.Return:
                        Debug.PrintValue(_stack.Pop());
                        Console.WriteLine();
                        return InterpretResult.Ok;
                }
            }
        }
    }
}