using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; 
using Generator.VisitorPattern;

namespace Generator.Statements
{

    public abstract class Statement : CodeLine
    {
        // Als 0, dan is er geen comment
        protected string        _pat                = "";
        protected string        _pcommand           = "";
        protected string        _command            = "";

        // Een aantal statements hebben een expressie, bv If, ElseIf, While
        // De param geeft die aan. Als er geen param is
        protected string        _pparam             = "";
        protected string        _param              = "";

        // pcomment kan ook de space aan het einde van de regel zijn als er geen commentaar is.
        protected string        _pcomment           = "";
        protected string        _comment            = "";
        protected string        _unrecognised       = "";

        #region Property accessors
        public String pAt
        {
            get
            {
                return _pat;
            }
            set
            {
                _pat = value;
            }
        }
        public String pCommand
        {
            get
            {
                return _pcommand;
            }
            set
            {
                _pcommand = value;
            }
        }
        public String Command
        {
            get
            {
                return _command;
            }
        }
        public String pComment
        {
            get
            {
                return _pcomment;
            }
            set
            {
                _pcomment = value;
            }
        }
        public String Comment
        {
            get
            {
                return _comment;
            }
        }
        public String pParam
        {
            get
            {
                return _pparam;
            }
        }
        public String Param
        {
            get
            {
                return _param;
            }
        }
        public String Unrecognised
        {
            get
            {
                return _unrecognised;
            }
        }
        #endregion

        /// <summary>
        /// Maak het statement zonder de Match op command, param en comment te doen.
        /// Aanroeper doet het lekker zelf (bv Comment)
        /// </summary>
        /// <param name="linenr"></param>
        /// <param name="line"></param>
        public Statement(int linenr, string line)
            : base(linenr, line)
        {
        }
        /// <summary>
        /// Maak het statement, check of de regel niet beeindigd wordt door commentaar.
        /// Noteer allerlei onderdelen van het statement
        /// </summary>
        /// <param name="linenr"></param>
        /// <param name="line"></param>
        public Statement(int linenr, Match m, string line)
            : base(linenr, line)
        {
            int                  pos                = 0;
            int                  commentpos         = 0;
            _pat                                    = m.Groups["pat"].Value;
            _pcommand                               = m.Groups["pcommand"].Value;
            _command                                = m.Groups["command"].Value;
            
            while ((pos = line.IndexOf("--", pos + 1)) > -1)
            {
                if (EvenNumberNonEscapedDoubleQuotes(line.Substring(0, pos)))
                {
                    commentpos                      = pos;
                    _comment                        = line.Substring(pos);
                    break;
                }
            }

            pos                                     = m.Length;
            while (pos < line.Length && line[pos] == ' ')
                pos++;
            _pparam                                 = line.Substring(m.Length, pos - m.Length);
            if (commentpos > 0)
            {
                _param                              = line.Substring(pos, commentpos - pos);
            }
            else if (pos < line.Length)
            {
                _param                              = line.Substring(pos);
            }

            // Verplaats eindspaties van de param naar de pcomment
            while (_param != String.Empty && _param[_param.Length - 1] == ' ')
            {
                _param                              = _param.Substring(0, _param.Length - 1);
                _pcomment                          += ' ';
            }

        }

        private bool EvenNumberNonEscapedDoubleQuotes(string s)
        {
            // We beginnen op false. Elk onderdeel van het gesplitte deel
            // zorgt voor een omkering hiervan. Tenzij deze geescaped was.
            bool                    result          = false;
            foreach (string sub in s.Split('"'))
            {
                if (!sub.EndsWith("\\"))
                    result                          = !result;
            }
            return result;
        }
    }
    
    /// <summary>
    /// Breakt de loop af.
    /// </summary>
    public class BreakStatement : Statement
    {
        public const string         keyword         = "Break";
        public BreakStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    /// <summary>
    /// Al of niet met een expressie. Hangt af van de soort functie waar deze in staat.
    /// </summary>
    public class ReturnStatement : Statement
    {
        public const string         keyword         = "Return";
        public ReturnStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    public class ExitStatement : Statement
    {
        public const string         keyword         = "Exit";
        public ExitStatement(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    public class VarDeclaration : Statement
    {
        public const string         keyword         = "Var";
        private static Regex        _reg_type       = new Regex(@"^(?<pvartype>\s*)(?<vartype>[-\w]+)\s*", RegexOptions.Compiled);
        private static Regex        _reg_naam       = new Regex(@"^(?<pvarname>\s*)(?<varname>\w+)\b", RegexOptions.Compiled);
        private static Regex        _reg_expr       = new Regex(@"^(?<passign>\s*=)(?<initexpr>.+)$", RegexOptions.Compiled);
        private string              _pvartype       = "";
        private string              _vartype        = "";
        private string              _pvarname       = "";
        private string              _varname        = "";
        private string              _passign        = "";
        private string              _initexpr       = "";
        #region Property accessors
        public string VarType
        {
            get
            {
                return _vartype;
            }
        }
        public string VarName
        {
            get
            {
                return _varname;
            }
        }
        public String InitialisationExpression
        {
            get
            {
                return _initexpr;
            }
        }
        public string pVarType
        {
            get
            {
                return _pvartype;
            }
        }
        public string pVarName
        {
            get
            {
                return _pvarname;
            }
        }
        public String pAssign
        {
            get
            {
                return _passign;
            }
        }
        #endregion

        public VarDeclaration(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            string                  rest            = _pparam + _param;
            _pparam                                 = String.Empty;
            _param                                  = String.Empty;

            // Type opsnorren
            m                                       = _reg_type.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = rest;
                return;
            }

            _pvartype                               = m.Groups["pvartype"].Value;
            _vartype                                = m.Groups["vartype"].Value;

            // Naam opsnorren
            rest                                    = rest.Substring(m.Groups["vartype"].Index + m.Groups["vartype"].Length);
            m                                       = _reg_naam.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = rest;
                return;
            }

            _pvarname                               = m.Groups["pvarname"].Value;
            _varname                                = m.Groups["varname"].Value;

            // Evt expressie opsnorren
            rest                                    = rest.Substring(m.Length);
            m                                       = _reg_expr.Match(rest);
            if (m.Success)
            {
                _passign                            = m.Groups["passign"].Value;
                _initexpr                           = m.Groups["initexpr"].Value;
            }
            else if (rest.Trim() != "")
            {
                _unrecognised                       = rest;
            }
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    /// <summary>
    /// Verwacht een Type Naam (optioneel:) = Expressie die naar iets van het Type evalueert. 
    /// Is een subclass van VarDeclaration om daarvan gelijkend gedrag te erven.
    /// </summary>
    public class GlobalVarDeclaration : VarDeclaration
    {
        new public const string     keyword         = "Global";
        public string               vartype;
        public string               varnaam;
        public string               expression;
        public GlobalVarDeclaration(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    /// <summary>
    /// Verwacht een expressie die naar een String evalueert. 
    /// Standaard parameter moet een expressie zijn
    /// </summary>
    public class InfoMessage : Statement
    {
        public const string         keyword         = "Info";
        public InfoMessage(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    /// <summary>
    /// Verwacht een expressie die naar een String evalueert
    /// </summary>
    public class ErrorMessage : Statement
    {
        public const string         keyword         = "Error";
        public ErrorMessage(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
    public class Comment : Statement
    {
        /// <summary>
        /// Commentpos is dus de positie van het echte werk
        /// </summary>
        /// <param name="linenr"></param>
        /// <param name="pos"></param>
        /// <param name="line"></param>
        public Comment(int linenr, Match m, string line)
            : base(linenr, line)
        {
            _pat                                    = m.Groups["pat"].Value;
            _pcommand                               = m.Groups["pcommand"].Value;
            _comment                                = line.Substring(m.Groups["pcommand"].Index + _pcommand.Length); 
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

}
