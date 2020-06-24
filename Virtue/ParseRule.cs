using System;

namespace Virtue
{
    internal class ParseRule
    {
        public ParseRule(Action prefix, Action infix, Precedence precedence)
        {
            Prefix = prefix;
            Infix = infix;
            Precedence = precedence;
        }

        public Action Prefix { get; set; }
        public Action Infix { get; set; }
        public Precedence Precedence { get; set; }
    }
}