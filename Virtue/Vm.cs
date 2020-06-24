#define DEBUG_TRACE_EXECUTION

using System;
using System.Collections.Generic;
using System.Linq;

namespace Virtue
{
    internal class Vm
    {
        private static readonly Lazy<Vm> Lazy = new Lazy<Vm>(() => new Vm());
        public static Vm Instance => Lazy.Value;

        private int _ip;
        private Chunk _chunk;
        private readonly Stack<double> _stack;

        private Vm()
        {
            _stack = new Stack<double>();
        }

        public InterpretResult Interpret(string source)
        {
            var chunk = new Chunk();
            if (!Compiler.Compile(source, chunk))
            {
                return InterpretResult.CompileError;
            }

            _chunk = chunk;
            _ip = 0;
            return Run();
        }

        private InterpretResult Run()
        {
            while (true)
            {
#if DEBUG_TRACE_EXECUTION
                ShowDebugTraceExecution();
#endif
                OpCode instruction;
                switch (instruction = (OpCode)ReadByte())
                {
                    case OpCode.Constant:
                        var constant = ReadConstant();
                        _stack.Push(constant);
                        break;

                    case OpCode.Add:
                        BinaryOp((a, b) => a + b);
                        break;

                    case OpCode.Subtract:
                        BinaryOp((a, b) => a - b);
                        break;

                    case OpCode.Multiply:
                        BinaryOp((a, b) => a * b);
                        break;

                    case OpCode.Divide:
                        BinaryOp((a, b) => a / b);
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

            byte ReadByte() => _chunk.Code[_ip++];
            double ReadConstant() => _chunk.Constants[ReadByte()];
            void BinaryOp(Func<double, double, double> op)
            {
                var b = _stack.Pop();
                var a = _stack.Pop();
                _stack.Push(op(a, b));
            }
        }

        private void ShowDebugTraceExecution()
        {
            Console.Write("          ");
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                Console.Write("[ ");
                Debug.PrintValue(_stack.ElementAt(i));
                Console.Write("]");
            }

            Console.WriteLine();
            Debug.DisassembleInstruction(_chunk, _ip);
        }
    }
}