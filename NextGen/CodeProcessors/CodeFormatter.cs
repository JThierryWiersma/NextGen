using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Generator.Utility;
using Generator.VisitorPattern;
using Generator.Statements;

namespace Generator.CodeProcessors
{
    public class CodeFormatter : CodeProcessor, ICodeLineVisitor
    {
        private RichTextBox     _txtText;
        private int             _level;
        private int             _templatetabs;

        public CodeFormatter(RichTextBox t)
        {
            _txtText                                = t;
            _templatetabs                           = OptionsSettings.Instance().Templatetabs;
        }

        public void ProcessAll(List<CodeLine> lines)
        {
            _level                                  = 0;
            foreach (CodeLine c in lines)
            {
                c.Accept(this);
            }

        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        private void Format(Statement l)
        {
            if (_level < 0)
            {
                _level                              = 0;
                //_levelwastoolowonline = _linenr;
            }
            if (l.pAt != String.Empty)
            {
                // Spaties aan begin van de regel verwijderen
                _txtText.Select(_txtText.GetFirstCharIndexFromLine(l.Linenr - 1), l.pAt.Length);
                _txtText.SelectedText               = "";
                l.pAt                               = "";
            }
            //int leveltabs = _level * _templatetabs;
            if (l.pCommand.Length > _level)
            {
                // haal er een paar weg
                _txtText.Select(_txtText.GetFirstCharIndexFromLine(l.Linenr - 1) + 1, l.pCommand.Length - _level);
                _txtText.SelectedText               = "";
                l.pCommand                          = new string(' ', _level);
            }
            else if (l.pCommand.Length < _level)
            {
                // voeg er een paar toe
                _txtText.Select(_txtText.GetFirstCharIndexFromLine(l.Linenr - 1) + 1, 0);
                _txtText.SelectedText               = new string(' ', _level - l.pCommand.Length);
                l.pCommand                          = new string(' ', _level);
            }
        }

        public void Visit(CodeLine l)
        {
            // niks
        }

        public void Visit(Comment l)
        {
            Format(l);
        }

        public void Visit(BreakStatement l)
        {
            Format(l);
        }

        public void Visit(ReturnStatement l)
        {
            Format(l);
        }

        public void Visit(ExitStatement l)
        {
            Format(l);
        }

        public void Visit(VarDeclaration l)
        {
            Format(l);
        }

        public void Visit(GlobalVarDeclaration l)
        {
            Format(l);
        }

        public void Visit(InfoMessage l)
        {
            Format(l);
        }

        public void Visit(ErrorMessage l)
        {
            Format(l);
        }

        public void Visit(IfStatement l)
        {
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(ElseIfStatement l)
        {
            _level -= _templatetabs;
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(ElseStatement l)
        {
            _level -= _templatetabs;
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(EndIfStatement l)
        {
            _level -= _templatetabs;
            Format(l);
        }

        public void Visit(DoStatement l)
        {
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(WhileStatement l)
        {
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(LoopStatement l)
        {
            _level -= _templatetabs;
            Format(l);
        }

        public void Visit(Include l)
        {
            Format(l);
        }

        public void Visit(FunctionDeclaration l)
        {
            Format(l);
            _level += _templatetabs;
        }

        public void Visit(EndFunctionDeclaration l)
        {
            _level -= _templatetabs;
            Format(l);
        }

        public void Visit(Assignment l)
        {
            Format(l);
        }

        public void Visit(VoidFunctionCall l)
        {
            Format(l);
        }


    }
}
