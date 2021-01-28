using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Generator.Statements;
using Generator.VisitorPattern;
using Generator.ObserverPattern;
using Generator.Exceptions;
using Generator.Expressions;
using Generator.Utility;
using System.IO;

namespace Generator.CodeProcessors
{
    public class SyntaxChecker : CodeProcessor, ICodeLineVisitor, ISubject
    {
        List<Statement>         _tomatch            = new List<Statement>();
        List<SyntaxErrorException> _errors          = new List<SyntaxErrorException>();
        private bool            _includefile;
        // Unreachable statements checken we verder niet.
        // En we melden alleen de eerste.
        private Boolean         _unreachable        = false;
        private Boolean         _unreachable_info   = false; // is de info al gegeven
        private List<Var>       _globals            = null;

        public SyntaxChecker(bool includefile)
        {
            // Includefiles mogen niks anders hebben op het hoogste niveau dan
            // - commentaar
            // - globale variabelen definitie
            // - functie definities
            _includefile                            = includefile;
            _globals                                = new List<Var>();
        }

        public void AddGlobalVariable(Var v)
        {
            _globals.Add(v);
        }

        public void ProcessAll(List<CodeLine> lines)
        {
            foreach (CodeLine c in lines)
            {
                c.Accept(this);
            }
            foreach (CodeLine c in _tomatch)
            {
                if (c is DoStatement ||
                    c is WhileStatement)
                    _errors.Add(new LoopStatementMissingException(c.SourceCodeContext));
                else if (c is FunctionDeclaration)
                    _errors.Add(new EndFunctionMissingException(c.SourceCodeContext, (c as FunctionDeclaration).Name));
                else if (c is IfStatement ||
                    c is ElseIfStatement ||
                    c is ElseStatement)
                    _errors.Add(new EndIfMissingException(c.SourceCodeContext));
                else
                    System.Diagnostics.Debug.Assert(false, String.Format("Niet-gematchte codeline op de stack gevonden van type: {0} op regel {1}", c.GetType().Name, c.Linenr)); 

            }
            int errorcount = 0;
            foreach (SyntaxErrorException see in _errors)
            {
                if (errorcount++ > 25)
                    break;
                Notify(see.Message, see.InfoOnly ? NotificationType.Info : NotificationType.Erreur, see.Context);
            }
        }

        #region Observer pattern: ISubject
        List<IObserver> m_observers = new List<IObserver>();
        public void AddObserver(IObserver o)
        {
            if (m_observers.IndexOf(o) < 0)
                m_observers.Add(o);
        }
        public void RemoveObserver(IObserver o)
        {
            m_observers.Remove(o);
        }
        public void Notify()
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this);
        }
        public void Notify(string msg, ObserverPattern.NotificationType t, SourceCodeContext scc)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, msg, t, scc);
        }
        public void Notify(string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, firstpart, linkeditem, lastpart, filename, linenr);
        }
        #endregion

        /// <summary>
        /// Doe generieke controles. Als onbereikbaar, geef true terug, anders false.
        /// </summary>
        /// <returns></returns>
        private bool CheckGeneric(CodeLine l)
        {
            if (_unreachable)
            {
                if (!_unreachable_info)
                {
                    _errors.Add(new UnreachableCodeException(l.SourceCodeContext));
                    _unreachable_info               = true;
                }
                return true;
            }

            if (l is Statement && (l as Statement).Unrecognised != String.Empty)
            {
                _errors.Add(new UnexpectedInputException(l.SourceCodeContext, (l as Statement).Unrecognised));
            }
            return false;
        }
        /// <summary>
        /// Dit statement is weer reachable want is het begin van een nieuw blok.
        /// </summary>
        private void Reachable()
        {
            _unreachable                            = false;
            _unreachable_info                       = false;
        }

        /// <summary>
        /// Controleer of, als we een include file aan het doen zijn, we dan wel in een functie zitten.
        /// Voeg anders een errormelding toe, en geef true terug.
        /// </summary>
        /// <returns></returns>
        private bool CheckInclude(CodeLine l)
        {
            if (_includefile &&
                (_tomatch.Count == 0 || !(_tomatch[1] is FunctionDeclaration)))
            {
                _errors.Add(new IncludeFilesCanOnlyContainFunctionsException(l.SourceCodeContext));
                return true;
            }
            return false;
        }

        public void Visit(CodeLine l)
        {
            if (CheckInclude(l))
                return;

            // niks, in fase 2 misschien de expressions checken?
        }

        public void Visit(Comment l)
        {
            // altijd ok.
        }


        /// <summary>
        /// Er moet een loop ergens op de stack staan om te kunnen breaken.
        /// We komen in een onbereikbaar stuk code...
        /// </summary>
        /// <param name="l"></param>
        public void Visit(BreakStatement l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            foreach (Statement s in _tomatch)
            {
                if (s is Statements.DoStatement ||
                    s is Statements.WhileStatement)
                {
                    _unreachable = true;
                    return;
                }
            }
            // Informeer dat we geen Do/While konden vinden, 
            // dus dat de break niet op de juiste plek staat.
            _errors.Add(new BreakShouldBeInLoopException(l.SourceCodeContext));
        }

        /// <summary>
        /// Er moet een functie declaratie op de stack staan om te kunnen returnen,
        /// Als het een void functie is: mag geen parameter expressie hebben
        /// Anders: Moet een parameter hebben (van het juiste type, expressies doen we later)
        /// We komen in een onbereikbaar stuk code...
        /// </summary>
        /// <param name="l"></param>
        public void Visit(ReturnStatement l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            foreach (Statement s in _tomatch)
            {
                if (s is Statements.FunctionDeclaration)
                {
                    FunctionDeclaration f = s as FunctionDeclaration;
                    if (f.ResultType == "Void")
                    {
                        if (l.Param != String.Empty)
                        {
                            _errors.Add(new ReturnExpressionUnexpectedException(l.SourceCodeContext));
                        }
                    }
                    else
                    {
                        if (l.Param == String.Empty)
                        {
                            _errors.Add(new ReturnMissingExpressionException(l.SourceCodeContext));
                        }
                    }
                    _unreachable = true;
                    return;
                }
            }
            // Informeer dat we geen Function konden vinden, 
            // dus dat de 'return' niet op de juiste plek staat.
            _errors.Add(new ReturnNotInFunctionUnexpectedException(l.SourceCodeContext));
        }

        public void Visit(ExitStatement l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // We komen in een onbereikbaar stuk code...
            _unreachable = true;
        }

        public void Visit(VarDeclaration l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // We moeten een variabele hebben en een type, niet perse een initialisatie
            if (l.VarType == String.Empty)
            {
                _errors.Add(new TypenameExpectedException(l.SourceCodeContext));
                return;
            }
            if (l.VarName == String.Empty)
            {
                _errors.Add(new IdentifierExpectedException(l.SourceCodeContext));
                return;
            }
        }
        
        /// <summary>
        /// Deze mag wel in een includefile op het hoogste niveau.
        /// </summary>
        /// <param name="l"></param>
        public void Visit(GlobalVarDeclaration l)
        {
            // We moeten een variabele hebben en een type, niet perse een initialisatie
            if (CheckGeneric(l))
                return;

            // We moeten een variabele hebben en een type, niet perse een initialisatie
            if (l.VarType == String.Empty)
            {
                _errors.Add(new TypenameExpectedException(l.SourceCodeContext));
                return;
            }
            if (l.VarName == String.Empty)
            {
                _errors.Add(new IdentifierExpectedException(l.SourceCodeContext));
                return;
            }
        }

        public void Visit(InfoMessage l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie die naar een string evalueert?
        }

        public void Visit(ErrorMessage l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie die naar een string evalueert?
        }

        public void Visit(IfStatement l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param == String.Empty)
            {
                _errors.Add(new IfMissingConditionException(l.SourceCodeContext));
            }

            // If op de stack zetten. 
            _tomatch.Add(l);
        }

        /// <summary>
        /// Dit is weer reachable
        /// Expressie gevonden?
        /// Staat er een if bovenop de stack of een elseif?
        /// Eraf halen en ElseIf erop zetten
        /// </summary>
        /// <param name="l"></param>
        public void Visit(ElseIfStatement l)
        {
            Reachable();
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param == String.Empty)
            {
                _errors.Add(new IfMissingConditionException(l.SourceCodeContext));
            }

            if (_tomatch.Count > 0 &&
                (_tomatch[_tomatch.Count - 1] is IfStatement ||
                 _tomatch[_tomatch.Count - 1] is ElseIfStatement))
            {
                _tomatch[_tomatch.Count - 1] = l;
            }
            else
            {
                _errors.Add(new ElseIfNotMatchedException(l.SourceCodeContext));
            }
        }

        /// <summary>
        /// Dit is weer reachable
        /// Geen parameter gevonden...?
        /// Staat er een if bovenop de stack of een elseif?
        /// Eraf halen en Else erop zetten
        /// </summary>
        /// <param name="l"></param>
        public void Visit(ElseStatement l)
        {
            Reachable();
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param != String.Empty)
            {
                _errors.Add(new UnexpectedInputException(l.SourceCodeContext, l.Param));
            }

            if (_tomatch.Count > 0 &&
                (_tomatch[_tomatch.Count - 1] is IfStatement ||
                 _tomatch[_tomatch.Count - 1] is ElseIfStatement))
            {
                _tomatch[_tomatch.Count - 1] = l;
            }
            else
            {
                _errors.Add(new ElseNotMatchedException(l.SourceCodeContext));
            }
        }
        /// <summary>
        /// Dit is weer reachable
        /// Staat er een if bovenop de stack of een elseif?
        /// Eraf halen.
        /// </summary>
        /// <param name="l"></param>
        public void Visit(EndIfStatement l)
        {
            Reachable();
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param != String.Empty)
            {
                _errors.Add(new UnexpectedInputException(l.SourceCodeContext, l.Param));
            }

            if (_tomatch.Count > 0 &&
                (_tomatch[_tomatch.Count - 1] is IfStatement ||
                 _tomatch[_tomatch.Count - 1] is ElseIfStatement ||
                 _tomatch[_tomatch.Count - 1] is ElseStatement))
            {
                _tomatch.RemoveAt(_tomatch.Count - 1);
            }
            else
            {
                _errors.Add(new EndIfNotMatchedException(l.SourceCodeContext));
            }
        }

        /// <summary>
        /// Do
        /// </summary>
        /// <param name="l"></param>
        public void Visit(DoStatement l)
        {
            if (CheckInclude(l))
                return;

            // Expressie gevonden? 
            // Do op de stack zetten
            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param == String.Empty)
            {
                _errors.Add(new DoExpressionExpectedException(l.SourceCodeContext));
            }

            _tomatch.Add(l);
        }

        /// <summary>
        /// Expressie gevonden?
        /// While op de stack zetten
        /// </summary>
        /// <param name="l"></param>
        public void Visit(WhileStatement l)
        {
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param == String.Empty)
            {
                _errors.Add(new WhileExpressionExpectedException(l.SourceCodeContext));
            }

            _tomatch.Add(l);
        }

        /// <summary>
        /// Do of While boven op de stack?
        /// Eraf halen
        /// Dit is weer reachable
        /// </summary>
        /// <param name="l"></param>
        public void Visit(LoopStatement l)
        {
            Reachable();
            if (CheckInclude(l))
                return;

            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param != String.Empty)
            {
                _errors.Add(new UnexpectedInputException(l.SourceCodeContext, l.Param));
            }

            if (_tomatch.Count > 0 &&
                (_tomatch[_tomatch.Count - 1] is DoStatement ||
                 _tomatch[_tomatch.Count - 1] is WhileStatement))
            {
                _tomatch.RemoveAt(_tomatch.Count - 1);
            }
            else
            {
                _errors.Add(new LoopNotMatchedException(l.SourceCodeContext));
            }
        }

        /// <summary>
        /// Er moet wel iets meegegeven zijn. Als het meezit zelfs een bestandsnaam die we kunnen vinden?
        /// Er mag ook niks op de stack staan. Include moet op hoogste niveau.
        /// </summary>
        /// <param name="l"></param>
        public void Visit(Include l)
        {
            // Bestaand Bestand gevonden? Expressie gevonden?
            if (_tomatch.Count > 0)
            {
                _errors.Add(new IncludeFileShouldBeTopLevelException(l.SourceCodeContext));
                return;
            }
            string includefilename = l.Param;
            if (includefilename == String.Empty)
            {
                _errors.Add(new IncludeFileNotFoundException(l.SourceCodeContext, ""));
                return;
            }
            String includefilepath = TemplateUtil.Instance().CombineAndCompact(Path.GetDirectoryName(l.SourceCodeContext.Filename), includefilename);
            if (!File.Exists(includefilepath))
            {
                _errors.Add(new IncludeFileNotFoundException(l.SourceCodeContext, includefilepath));
            }
        }
        /// <summary>
        /// Alles ok met de functiedeclaratie?
        /// Nog niks op de stack...?
        /// Op de stack zetten
        /// </summary>
        /// <param name="l"></param>
        public void Visit(FunctionDeclaration l)
        {
            if (CheckGeneric(l))
                return;

            // We moeten een variabele hebben en een type, niet perse een initialisatie
            if (l.ResultType == String.Empty)
            {
                _errors.Add(new TypenameExpectedException(l.SourceCodeContext));
                return;
            }
            if (l.Name == String.Empty)
            {
                _errors.Add(new IdentifierExpectedException(l.SourceCodeContext));
                return;
            }
            if (l.ParListFound)
            {
                foreach (FunctionParameterDeclaration fpd in l.ParList)
                {
                    if (fpd.ParType == String.Empty)
                    {
                        _errors.Add(new TypenameExpectedException(l.SourceCodeContext));
                        continue;
                    }
                    if (fpd.ParName == String.Empty)
                    {
                        _errors.Add(new IdentifierExpectedException(l.SourceCodeContext));
                    }
                }
            }
            _tomatch.Add(l);
        }

        /// <summary>
        /// Hebben we een functiedeclaratie bovenop de stack
        /// Eraf halen
        /// Dit is weer reachable
        /// </summary>
        /// <param name="l"></param>
        public void Visit(EndFunctionDeclaration l)
        {
            Reachable();
            if (CheckGeneric(l))
                return;

            // Expressie gevonden?
            if (l.Param != String.Empty)
            {
                _errors.Add(new UnexpectedInputException(l.SourceCodeContext, l.Param));
            }

            if (_tomatch.Count > 0 &&
                (_tomatch[_tomatch.Count - 1] is FunctionDeclaration))
            {
                _tomatch.RemoveAt(_tomatch.Count - 1);
            }
            else
            {
                _errors.Add(new EndFunctionNotMatchedException(l.SourceCodeContext));
            }

        }

        public void Visit(Assignment l)
        {
            // Altijd ok, in fase 2 de expressie checken
        }

        public void Visit(VoidFunctionCall l)
        {
            // Altijd ok, in fase 2 de expressie (parameters) checken.
        }
    }
}
