using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Generator.Exceptions;
using Generator.Statements;

namespace Generator.Expressions
{
    public class FunctionInfo
    {
        private string              _header;
        private string              _name;
        private string              _resulttype;
        private string              _parameterlist; // moeten we nog parsen, voorlopig effe zo.
        private Var                 _resultprototype;
        private StringCollection    _body;
        private SourceCodeContext   _declarationat;
        private ArrayList           _parameterprototypes;
        
        public FunctionInfo(SourceCodeContext linenr, string header)
        {
            _declarationat = linenr;
            _header = header;

            Match m = Regex.Match(_header, @"^\s?@\s*Function\s+(?<resulttype>\S+)\s+(?<name>\w+)(?<parameterlist>.*)$");
            if (!m.Success)
            {
                throw new FunctionDeclarationException(linenr, header);
            }
            _resulttype = m.Groups["resulttype"].Value;
            _name = m.Groups["name"].Value;
            _parameterlist = m.Groups["parameterlist"].Value;
            _body = new StringCollection();
            
            BuildParameterPrototypes();

        }

        public void Add(string bodyline)
        {
            _body.Add(bodyline);
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
        public int DeclaredAtLine
        {
            get
            {
                return _declarationat.Linenr;
            }
        }
        public SourceCodeContext DeclaredAt
        {
            get
            {
                return _declarationat;
            }
        }
        public StringCollection BodyCopy()
        {
            StringCollection copy = new StringCollection();
            foreach(string s in _body)
                copy.Add(s);
            return copy;
        }
        public string ResultType()
        {
            return _resulttype;
        }
        public Var xxResultVar()
        {
            switch (_resulttype)
            {
                case "Decimal":
                    return new DecimalVar();
                case "String":
                    return new StringVar();
                case "Boolean":
                    return new BooleanVar();
                case "Void":
                    return null;
                default:
                    return null;
            }
        }
        private void BuildParameterPrototypes()
        {
            Match m = Regex.Match(_parameterlist, @"^\s*\(\s*(?<parameters>[^\)]*)\)\s*$");
            if (!m.Success)
                throw new Exceptions.FunctionParameterListFormatException(_declarationat, _parameterlist);

            string parameterlist = m.Groups["parameters"].Value;
            _parameterprototypes = new ArrayList();

            // Lege lijst levert geen parameters. Moet kunnen.
            if (parameterlist.Trim() != String.Empty)
            {
                foreach (string param in parameterlist.Split(','))
                {
                    m = Regex.Match(param, @"^\s*(?<type>\w+)\s+(?<name>\w+)\s*");
                    if (!m.Success)
                        throw new Exceptions.FunctionParameterListFormatException(_declarationat, param);
                    string ptype = m.Groups["type"].Value;
                    string pname = m.Groups["name"].Value;
                    Var p = Var.ConstructVar(ptype);
                    p.name = pname;
                    _parameterprototypes.Add(p);
                }
            }
        }

        /// <summary>
        /// Geeft een lijst van parameters terug die overeenkomen met de gedeclareerde types van de functie.
        /// Zijn bruikbaar als actuele parameters omdat ze gekloond zijn.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetParameterPrototypes()
        {
            ArrayList result = new ArrayList(_parameterprototypes.Count);
            foreach (object o in _parameterprototypes)
            {
                Var p = o as Var;
                result.Add(p.CreateClone());
            }
            return result;   
        }
    }
}
