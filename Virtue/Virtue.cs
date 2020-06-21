using System;
using System.IO;

namespace Virtue
{
    internal class Virtue
    {
        private static readonly Vm Vm = Vm.Instance;

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Repl();
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                Console.WriteLine("Usage: virtue [path]");
                Environment.Exit(64);
            }
        }

        private static void Repl()
        {
            while (true)
            {
                Console.Write("> ");
                Vm.Interpret(Console.ReadLine());
            }
        }

        private static void RunFile(string path)
        {
            var result = Vm.Interpret(File.ReadAllText(path));
            if (result == InterpretResult.CompileError) Environment.Exit(65);
            if (result == InterpretResult.RuntimeError) Environment.Exit(70);
        }
    }
}