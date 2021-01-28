using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Generator.VisitorPattern;

namespace Generator.Statements
{
    public class CodeLine
    {
        string                      _filename;
        int                         _linenr;
        string                      _content;

        public CodeLine(int linenr, string line)
        {
            _linenr                                 = linenr;
            _content                                = line;
        }
        public SourceCodeContext SourceCodeContext
        {
            get
            {
                return new SourceCodeContext("", _filename, _linenr);
            }
        }
        public int Linenr
        {
            get
            {
                return _linenr;
            }
            set
            {
                _linenr = value;
            }
        }
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        private static Regex            line_regex  = new Regex(@"^(?<pat>\s*)@(?<pcommand>\s*)(?<command>\w*)\b", RegexOptions.Compiled);
        private static Regex            cmmt_regex  = new Regex(@"^(?<pat>\s*)@(?<pcommand>\s*)--", RegexOptions.Compiled);
        private static Regex            cmm2_regex  = new Regex(@"^(?<pat>\s*)@(?<pcommand>\s*)$", RegexOptions.Compiled);
        private static Regex            ass_regex   = new Regex(@"^(?<pat>\s*)@(?<pcommand>\s*)(?<varname>\w+)(?<passign>\s*)(?<assign>[-/+*]?=)", RegexOptions.Compiled);
        private static Regex            vfc_regex   = new Regex(@"^(?<pat>\s*)@(?<pcommand>\s*)(?<functionname>\w+\s*)[(]", RegexOptions.Compiled);

        public static CodeLine BuildCodeLine(string filename, int linenr, string codeline)
        {
            CodeLine                    result      = null;
            Match                       m           = line_regex.Match(codeline);

            if (m.Success)
            {
                string                  sCommand    = m.Groups["command"].Value;
                int                     pos         = m.Length;
                switch (sCommand)
                {
                    case DoStatement.keyword:
                        result                      = new DoStatement(linenr, m, codeline);
                        break;

                    case WhileStatement.keyword:
                        result                      = new WhileStatement(linenr, m, codeline);
                        break;

                    case IfStatement.keyword:
                        result                      = new IfStatement(linenr, m, codeline);
                        break;

                    case ElseStatement.keyword:
                        result                      = new ElseStatement(linenr, m, codeline);
                        break;

                    case ElseIfStatement.keyword:
                        result                      = new ElseIfStatement(linenr, m, codeline);
                        break;

                    case EndIfStatement.keyword:
                        result                      = new EndIfStatement(linenr, m, codeline);
                        break;

                    case ErrorMessage.keyword:
                        result                      = new ErrorMessage(linenr, m, codeline);
                        break;

                    case InfoMessage.keyword:
                        result                      = new InfoMessage(linenr, m, codeline);
                        break;

                    case LoopStatement.keyword:
                        result                      = new LoopStatement(linenr, m, codeline);
                        break;

                    case BreakStatement.keyword:
                        result                      = new BreakStatement(linenr, m, codeline);
                        break;

                    case ReturnStatement.keyword:
                        result                      = new ReturnStatement(linenr, m, codeline);
                        break;

                    case ExitStatement.keyword:
                        result                      = new ExitStatement(linenr, m, codeline);
                        break;

                    case VarDeclaration.keyword:
                        result                      = new VarDeclaration(linenr, m, codeline);
                        break;

                    case GlobalVarDeclaration.keyword:
                        result                      = new GlobalVarDeclaration(linenr, m, codeline);
                        break;

                    case FunctionDeclaration.keyword:
                        result                      = new FunctionDeclaration(linenr, m, codeline);
                        break;

                    case EndFunctionDeclaration.keyword:
                        result                      = new EndFunctionDeclaration(linenr, m, codeline);
                        break;

                    case Include.keyword:
                        result                      = new Include(linenr, m, codeline);
                        break;
                }
            }
            // Misschien is het een comment?
            if (result == null)
            {
                m                                   = cmmt_regex.Match(codeline);
                if (m.Success)
                {
                    result                          = new Comment(linenr, m, codeline);
                }
            }
            if (result == null)
            {
                m                                   = ass_regex.Match(codeline);
                if (m.Success)
                {
                    result                          = new Assignment(linenr, m, codeline);
                }
            }
            if (result == null)
            {
                m                                   = vfc_regex.Match(codeline);
                if (m.Success)
                {
                    result                          = new VoidFunctionCall(linenr, m, codeline);
                }
            }
            if (result == null && codeline.Trim() == "@")
            {
                m                                   = cmm2_regex.Match(codeline);
                if (m.Success)
                {
                    result                          = new Comment(linenr, m, codeline);
                }
            }
            if (result == null)
            {
                // Anders zal het een gewone tekst regel zijn? Met evt een paar expressies erin
                result                              = new CodeLine(linenr, codeline);
            }
            result._filename                        = filename;
            return result;
        }

        public virtual void Accept(ICodeLineVisitor v)
        {
            v.Visit(this);
        }
    }
}
