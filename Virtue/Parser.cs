using System;

namespace Virtue
{
    internal class Parser
    {
        private static readonly Lazy<Parser> Lazy = new Lazy<Parser>(() => new Parser());
        public static Parser Instance => Lazy.Value;
        public Token Current { get; set; }
        public Token Previous { get; set; }
        public bool HadError { get; set; }
        public bool PanicMode { get; set; }

        private Parser()
        {
        }
    }
}