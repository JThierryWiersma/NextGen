using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Generator.VisitorPattern;

namespace Generator.Statements
{
    /// <summary>
    /// Do loop over een concept, of een lijst attributen of sets of zo.
    /// **TODO** Later graag met een extra 'where' expressie
    /// </summary>
    public class DoStatement : Statement
    {
        public const string         keyword         = "Do";

        public DoStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    /// <summary>
    /// While een expressie true oplevert.
    /// </summary>
    public class WhileStatement : Statement
    {
        public const string         keyword         = "While";

        public WhileStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    /// <summary>
    /// Het einde van de loop.
    /// </summary>
    public class LoopStatement : Statement
    {
        public const string         keyword         = "Loop";

        public LoopStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

}
