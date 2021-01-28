using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Generator.VisitorPattern;

namespace Generator.Statements
{
    /// <summary>
    /// Heeft in param de expressie.
    /// </summary>
    public class IfStatement : Statement
    {
        public const string         keyword         = "If";
        public IfStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }

    }

    /// <summary>
    /// Moet een lege parameter hebben.
    /// </summary>
    public class ElseStatement : Statement
    {
        public const string         keyword         = "Else";
        public ElseStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            if (_param != String.Empty)
            {
                _unrecognised                       = _param;
                _param                              = String.Empty;
            }
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    /// <summary>
    /// Een extra if met een nieuwe expressie.
    /// </summary>
    public class ElseIfStatement : Statement
    {
        public const string         keyword         = "ElseIf";

        public ElseIfStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    /// <summary>
    /// Einde van de if.
    /// </summary>
    public class EndIfStatement : Statement
    {
        public const string         keyword         = "EndIf";

        public EndIfStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            if (_param != String.Empty)
            {
                _unrecognised                       = _param;
                _param                              = String.Empty;
            }
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
}
