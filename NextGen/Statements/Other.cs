using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Generator.Expressions;
using Generator.VisitorPattern;

namespace Generator.Statements
{
    public class FunctionDeclaration : Statement
    {
        public const string         keyword         = "Function";

        private string              _pname;
        private string              _name;
        private string              _presulttype;
        private string              _resulttype;
        private string              _pparlist; 
        private bool                _parlistfound = false;
        private string              _parlist;           // moeten we nog parsen, voorlopig effe zo.
        private Var                 _resultprototype;
        private StringCollection    _body;
        //private SourceCodeContext   _declarationat;
        private List<FunctionParameterDeclaration> _parameterprototypes;
        private static Regex        _reg_type     = new Regex(@"^(?<resulttype>\S+)\b");       
        private static Regex        _reg_name     = new Regex(@"^(?<pname>\s+)(?<name>\w+)\b");       
        private static Regex        _reg_parlist  = new Regex(@"^(?<pparlist>\s*)\((?<parlist>[^\)]*)\)$");       

        public FunctionDeclaration(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            string                  rest            = _param;
            _presulttype                            = _pparam;
            _param                                  = "";
            _pparam                                 = "";

            m                                       = _reg_type.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = rest;
                return;
            }
            _resulttype                             = m.Groups["resulttype"].Value;
            rest                                    = rest.Substring(m.Length); 

            m                                       = _reg_name.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = rest;
                return;
            }
            _pname                                  = m.Groups["pname"].Value;
            _name                                   = m.Groups["name"].Value;
            rest                                    = rest.Substring(m.Length);

            m                                       = _reg_parlist.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = rest;
                return;
            }
            _pparlist                               = m.Groups["pparlist"].Value;
            _parlistfound                           = true;
            _parlist                                = m.Groups["parlist"].Value;
            _parameterprototypes                    = new List<FunctionParameterDeclaration>();

            // Lege lijst levert geen parameters. Moet kunnen.
            if (_parlist.Trim() != String.Empty)
            {
                foreach (string param in _parlist.Split(','))
                {
                    FunctionParameterDeclaration fpd = new FunctionParameterDeclaration(param);
                    _parameterprototypes.Add(fpd);
                }
            }
            //_body                                   = new StringCollection();
        }

        #region Property accessors
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string ResultType
        {
            get
            {
                return _resulttype;
            }
        }
        public string pResultType
        {
            get
            {
                return _presulttype;
            }
        }
        public string pName
        {
            get
            {
                return _pname;
            }
        }
        public bool ParListFound
        {
            get
            {
                return _parlistfound;
            }
        }
        /// <summary>
        /// Space voor de parlist open haak
        /// </summary>
        public string pParList
        {
            get
            {
                return _pparlist;
            }
        }
        public List<FunctionParameterDeclaration> ParList
        {
            get
            {
                return _parameterprototypes;
            }
        }
        #endregion

        /// <summary>
        /// Geeft een lijst van parameters terug die overeenkomen met de gedeclareerde types van de functie.
        /// Zijn bruikbaar als actuele parameters omdat ze gekloond zijn.
        /// </summary>
        /// <returns></returns>
        public List<Var> GetParameterPrototypes()
        {
            List<Var>               result          = new List<Var>(_parameterprototypes.Count);
            foreach (FunctionParameterDeclaration fpd in _parameterprototypes)
            {
                //FunctionParameterDeclaration p = o as FunctionParameterDeclaration;
                result.Add(fpd.Var.CreateClone());
            }
            return result;   
        }

        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
       
    }
    public class FunctionParameterDeclaration
    {
        private string          _ppartype           = "";
        private string          _partype            = "";
        private string          _pparname           = "";
        private string          _parname            = "";
        private string          _pnext              = "";
        private string          _unrecognised       = "";
        private Var             _var                = null;
        private static Regex    _reg_partype        = new Regex(@"^(?<ppartype>\s*)(?<partype>\w+)");
        private static Regex    _reg_parname        = new Regex(@"^(?<pname>\s+)(?<name>\w+)(?<pnext>\s*)$");

        #region Property accessors
        public string pParType
        {
            get
            {
                return _ppartype;
            }
        }
        public string ParType
        {
            get
            {
                return _partype;
            }
        }
        public string pParName
        {
            get
            {
                return _pparname;
            }
        }
        public string ParName
        {
            get
            {
                return _parname;
            }
        }
        public string pNext
        {
            get
            {
                return _pnext;
            }
        }
        public string Unrecognised
        {
            get
            {
                return _unrecognised;
            }
        }
        public Var Var
        {
            get
            {
                return _var;
            }
        }
        #endregion

        public FunctionParameterDeclaration(string par)
        {
            Match                   m               = _reg_partype.Match(par);
            if (!m.Success)
            {
                _unrecognised                       = par;
                return;
            }
            _ppartype                               = m.Groups["ppartype"].Value;
            _partype                                = m.Groups["partype"].Value;

            // Ga verder met de rest
            string                  rest            = par.Substring(m.Length);
            m                                       = _reg_parname.Match(rest);
            if (!m.Success)
            {
                _unrecognised                       = par;
                return;
            }
            _pparname                               = m.Groups["pname"].Value;
            _parname                                = m.Groups["name"].Value;
            _pnext                                  = m.Groups["pnext"].Value;
            try
            {
                _var                                = Var.ConstructVar(_partype);
                _var.name                           = _parname;
            }
            catch (ApplicationException)
            {
                // Als het type niet bestaat komen we hierin
                _var                                = null;
            }
        }
    }

    public class EndFunctionDeclaration : Statement
    {
        public const string         keyword         = "EndFunction";

        public EndFunctionDeclaration(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            if (_param != String.Empty)
            {
                _unrecognised                       = _param;
                _param                              = "";
            }
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    public class Include : Statement
    {
        public const string         keyword         = "Include";

        public Include(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    public class Assignment : Statement
    {
        private string              _varname        = "";
        private string              _pass           = "";
        private string              _ass            = "";
        private string              _expr           = "";

        #region Property accessors
        public string Varname
        {
            get
            {
                return _varname;
            }
        }
        public string pAssign
        {
            get
            {
                return _pass;
            }
        }
        public string Assign
        {
            get
            {
                return _ass;
            }
        }
        public string Expression
        {
            get
            {
                return _expr;
            }
        }
#endregion

        public Assignment(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            //_pvarname                               = m.Groups["pvarname"].Value;
            _varname                                = m.Groups["varname"].Value;
            _pass                                   = m.Groups["passign"].Value;
            _ass                                    = m.Groups["assign"].Value;
            _expr                                   = line.Substring(m.Length, line.Length - m.Length - _pcomment.Length - _comment.Length);
        }

        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }

    /// <summary>
    /// Expression is de complete functioncall (die dus naar een void functie zou kunnen zijn)
    /// Het is ook mogelijk dat er wel wat uitkomt, en dan is het gewoon een expressie die
    /// in de uitvoer gezet moet worden. Misschien is het zelfs wel een deel van een expressie die
    /// begint met een functieaanroep.
    /// </summary>
    public class VoidFunctionCall : Statement
    {

        private string          _expr               = "";
        #region Property accessors
        public string Expression
        {
            get
            {
                return _expr;
            }
        }
        #endregion

        public VoidFunctionCall(int linenr, Match m, string line)
            : base(linenr, m, line)
        {
            _expr                                   = line.Substring(m.Groups["pcommand"].Index, line.Length - _pcomment.Length - _comment.Length - m.Groups["pcommand"].Index);
        }
        public override void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
}
