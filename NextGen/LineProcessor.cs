using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Generator.Expressions;
using Generator.Utility;
using Generator.ObserverPattern;
using Generator.Exceptions;
using Generator.Statements;

namespace Generator
{
    public class LineProcessor : ISubject, IExternalTokenResolver, IObserver
    {
        private Stack               m_Stack;
        private Hashtable           m_VariableInfo;
        private Hashtable           m_GlobalVariableInfo;
        private Hashtable           m_FunctionInfo;
        private Log                 m_log;
        public  int                 m_errors;
        private CurrentInfo         m_initialcurrent; // zijn we mee opgestart, moeten we doorgeven naar (andere) functies.
        private CurrentInfo         m_current;
        private SourceCodeContext   m_currentlinenr;
        private String              m_currenttemplatename;
        
        private StringCollection    m_source;
        private StringCollection    m_target;
        private ExpressionEvaluator m_evaluator;

        // De functie 'm_currentfunction' zijn we nu aan het uitvoeren, als we tenminste een functie aan het uitvoeren zijn.
        // Dit kan dus informatie leveren voor een Return statement.
        // De m_functionresult vullen we eventueel met een Return statement en geldt dan als het resultaat van het geheel.
        private FunctionInfo        m_currentfunction;
        private Var                 m_functionresult;

        // Keep informatie
        private Hashtable           m_KeepInfo;
        private const int           KEEP_START_LEN  = 6;
        private const string        KEEP_START      = "@Keep:";
        private const string        KEEP_END        = "@EndKeep@";

        public LineProcessor(
            CurrentInfo             current, 
            Hashtable               globalvariables, 
            Hashtable               localvariables,
            StringCollection        source, 
            StringCollection        target, 
            Log                     log, 
            Hashtable               keepInfo,
            Hashtable               functioninfo,
            String                  currenttemplatename,
            FunctionInfo            currentfunction)
        {
            m_initialcurrent                        = current;
            m_current                               = new CurrentInfo(m_initialcurrent);
            m_current.state                         = InterpretationStatus.Active;
            m_source                                = source;
            m_target                                = target;
            m_GlobalVariableInfo                    = globalvariables;
            m_VariableInfo                          = localvariables;
            m_log                                   = log;
            m_KeepInfo                              = keepInfo;
            m_FunctionInfo                          = functioninfo;
            m_currenttemplatename                   = currenttemplatename;
            m_currentfunction                       = currentfunction;

            m_Stack                                 = new Stack();
            m_errors                                = 0;
            if (m_currentfunction == null)
                m_currentlinenr                     = new SourceCodeContext(currenttemplatename, "", 0);
            else
                m_currentlinenr                     = new SourceCodeContext(m_currentfunction.DeclaredAt.Conceptname, m_currentfunction.DeclaredAt.Filename, 0);

            m_evaluator                             = new ExpressionEvaluator(this);
        }
        private string GetXmlValue(XmlNode x, string defaultvalue)
        {
            if (x == null)
                return defaultvalue;
            else
                return x.InnerText;
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
        public void Notify(string msg, NotificationType t, SourceCodeContext scc)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, msg, t, scc);
        }
        public void Notify(string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, firstpart, linkeditem, lastpart, filename, linenr);
        }
        public void ProcessUpdate(object o)
        {
            Notify();
        }
        public void ProcessUpdate(object o, string msg, NotificationType t, SourceCodeContext scc)
        {
            Notify(msg, t, scc);
        }
        public void ProcessUpdate(object o, string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            Notify(firstpart, linkeditem, lastpart, filename, linenr);
        }

        #endregion

        public int Generate()
        {
            try
            {
                Regex r = new Regex(@"\s*(\w*)\b", RegexOptions.Compiled);
                while (m_source.Count > 0)
                {
                    string sLineOrg = m_source[0];
                    m_log.Write(m_Stack.Count, m_current.state, sLineOrg);

                    int ddPos = sLineOrg.IndexOf(':');
                    if (ddPos >= 0 && ddPos < 6)
                    {
                        int i = 0;
                        int tmplinenr = 0;
                        for (; i < ddPos && Char.IsDigit(sLineOrg, i); i++)
                            tmplinenr = tmplinenr * 10 + sLineOrg[i] - '0';
                        if (i == ddPos)
                        {
                            m_currentlinenr.Linenr = tmplinenr;
                            sLineOrg = sLineOrg.Substring(ddPos + 1);
                        }
                    }
                    m_source.RemoveAt(0);

                    string sLineTrim = sLineOrg.Trim();

                    if (sLineTrim == "")
                    {
                        if (m_current.state == InterpretationStatus.Active)
                        {
                            m_target.Add("");
                            m_log.WriteOutput("");
                        }
                        continue;
                    }

                    if (sLineTrim == "@")
                    {
                        m_log.WriteOutput("ignored=>@" + sLineTrim);
                        continue;
                    }

                    if (sLineTrim.StartsWith("@"))
                    {
                        sLineTrim = sLineTrim.Substring(1).TrimStart();
                        if (sLineTrim.StartsWith("--"))
                        {
                            m_log.WriteOutput("comment ignored=>" + sLineTrim);
                            continue;
                        }
                        Match m = r.Match(sLineTrim);
                        if (m.Groups.Count > 0)
                        {
                            string sCommand = m.Groups[0].Value;
                            switch (sCommand)
                            {
                                case "Do":
                                    ProcessDo(sLineTrim);
                                    continue;
                                case "While":
                                    ProcessWhile(sLineTrim);
                                    continue;
                                case "If":
                                case "Else":
                                case "ElseIf":
                                case "EndIf":
                                    ProcessIf(sLineTrim);
                                    continue;
                                case "Error":
                                    ProcessError(sLineTrim);
                                    continue;
                                case "Info":
                                    ProcessInfo(sLineTrim);
                                    continue;
                                case "Loop":
                                    ProcessLoop();
                                    continue;
                                case "Break":
                                    ProcessBreak();
                                    continue;
                                case "Return":
                                    ProcessReturn(sLineTrim);
                                    continue;
                                /* case "Split":
                                    string[]	ls			= ProcessSplit(sLineOrg);
                                    if (ls != null)
                                    {
                                        m_target.AddRange(ls);
                                        foreach (string l in ls)
                                            m_log.WriteOutput(l);
                                    }
                                    continue;
                                */
                                case "Exit":
                                    if (m_current.state == InterpretationStatus.Active)
                                        m_source.Clear();
                                    continue;
                                case "Var":
                                    ProcessVarDeclaration(sLineTrim);
                                    continue;
                                case "Global":
                                    ProcessVarDeclaration(sLineTrim);
                                    continue;
                            }
                        }
                    }

                    // Keep it from original?
                    if (ProcessKeepInfo(sLineOrg))
                        continue;

                    // Skip if not active or to skip
                    if (m_current.state != InterpretationStatus.Active)
                        continue;

                    sLineOrg = ProcessLine(sLineOrg);
                    if (sLineOrg != null && sLineOrg.Trim() != "")
                    {
                        m_target.Add(sLineOrg);
                        m_log.WriteOutput(sLineOrg);
                    }
                }
            }
            catch (Generator.Exceptions.SyntaxErrorException ex)
            {
                WriteError(ex.Message);
                return m_errors++;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                return m_errors++;
            }

            return m_errors;
        }

        /// <summary>
        /// Process the template line. Find all @-delimited tokens and
        /// process these. Join the results into the total result and return that.
        /// </summary>
        /// <param name="line">line to search tokens in</param>
        /// <returns>line with @-tokens replaced by their processing result</returns>
        public string ProcessLine(string line)
        {
            string result = "";

            // Always process the macros first
            line = ProcessMacros(line);
            int firstAt = line.IndexOf("@");
            while (firstAt >= 0)
            {
                int secondAt = line.IndexOf("@", firstAt + 1);
                if (secondAt < 0)
                {
                    secondAt = line.Length;
                    line += "@";
                }

                // Add the non-special part to the result
                if (firstAt > 0)
                    result = result + line.Substring(0, firstAt);
                string tokenresult = "";
                if (firstAt + 1 == secondAt)
                {
                    result += "@";
                }
                else
                {
                    try
                    {
                        string part = line.Substring(firstAt + 1, secondAt - firstAt - 1);
                        int alignat = part.IndexOf("~");
                        int tabto = 0;
                        if (alignat >= 0)
                        {
                            if (Int32.TryParse(part.Substring(alignat + 1), out tabto))
                            {
                                part = part.Substring(0, alignat);
                            }
                            else
                            {
                                alignat = -1;
                            }
                        }
                        if (part.Trim() != "")
                        {
                            Var evalresult = m_evaluator.Evaluate(part);
                            if (evalresult != null)
                                tokenresult = evalresult.ToString();
                        }
                        if (alignat >= 0 && tabto > result.Length)
                            tokenresult = tokenresult.PadRight(tabto - result.Length - 1);
                    }
                    catch (ApplicationException ex)
                    {
                        if (++m_errors > 100)
                        {
                            throw;
                        }
                        tokenresult = "";
                        WriteError(CurrentContext() + "Evaluation of '" + line.Substring(firstAt + 1, secondAt - firstAt - 1) + "' raised exception: " + ex.Message);
                    }
                }
                if (secondAt < line.Length - 1)
                    line = tokenresult + line.Substring(secondAt + 1);
                else
                    line = tokenresult;

                // Check if more lines are added to the line.
                // if that is the case, add extra lines to the source
                // and continue processing of first line.
                // Prefix with the current result to get the proper indentation.
                if (line.IndexOf("\n") >= 0)
                {
                    int i = 0;
                    //if (result.Trim() == "")
                    //	i = 0;
                    //System.Diagnostics.Debug.Assert(result.Trim() == "", "Line '" + line + "' should contain no space before multiline replacement");
                    string[] lines = line.Split('\n');
                    for (int j = lines.GetLength(0) - 1; j >= i; j--)
                        m_source.Insert(0, result + lines[j].TrimEnd('\r'));
                    return null;//line = lines[0];
                }

                line = ProcessMacros(line);
                firstAt = line.IndexOf("@");
            }

            return result + line;
        }

        public Var TokenEvaluator(String s)
        {
            StringVar sv = new StringVar(s);
            return TokenEvaluator(sv);
        }
        public Var TokenEvaluator(StringVar token)
        {
            XmlNode t;
            string tt;

            try
            {
                XmlNode x = GetValueNodeFor(token.v, out t, out tt);
                if (x != null)
                {
                    token.v = "";
                    XmlNode typeatt = t.Attributes["type"];
                    if ((typeatt != null) && (typeatt.Value == "Order" || typeatt.Value == "Number"))
                    {
                        // Interpret value as decimal, but default to -1.
                        if (x.InnerText != "")
                            return new DecimalVar(x.InnerText);
                        else if (t.Attributes["default"] != null)
                            return new DecimalVar(t.Attributes["default"].Value);
                        else
                            return new DecimalVar();
                    }
                    else if ((typeatt != null) && (typeatt.Value == "Checkbox"))
                    {
                        // Interpret value as boolean, but default to false.
                        if (x.InnerText != "")
                            return new BooleanVar(x.InnerText);
                        else if (t.Attributes["default"] != null)
                            return new BooleanVar(t.Attributes["default"].Value);
                        else
                            return new BooleanVar();
                    }
                    else if (t.Name == "template")
                    {
                        ConceptVar cv = new ConceptVar(t);
                        cv.tvalue = x;
                        cv.empty = false;
                        return cv;
                    }
                    else
                    {
                        if (x.InnerText != "")
                            return new StringVar(x.InnerText);
                        else if (t.Attributes["default"] != null)
                            return new StringVar(t.Attributes["default"].Value);
                        else
                            return new StringVar();
                    }
                }
                else if (t != null && tt != "type")
                {
                    token.v = "";
                    XmlNode typeatt = t.Attributes["type"];
                    if ((typeatt != null) && (typeatt.Value == "Order" || typeatt.Value == "Number"))
                    {
                        if (t.Attributes["default"] != null)
                            return new DecimalVar(t.Attributes["default"].Value);
                        else
                            return new DecimalVar();
                    }
                    else if ((typeatt != null) && (typeatt.Value == "Checkbox"))
                    {
                        if (t.Attributes["default"] != null)
                            return new BooleanVar(t.Attributes["default"].Value);
                        else
                            return new BooleanVar();
                    }
                    else
                    {
                        if (t.Attributes["default"] != null)
                            return new StringVar(t.Attributes["default"].Value);
                        else
                            return new StringVar("");
                    }
                }

            }
            catch (FieldNotFoundException ex)
            {
                WriteError(CurrentContext() + ex.Message);
                m_errors++;
                if (m_errors > 100)
                {
                    throw new TooManyErrorsException();
                }
                // Doe alsof we'm afgehandeld hebben
                token.v = "";
                return new StringVar();
            }

            if (m_VariableInfo.ContainsKey(token.v))
            {
                Var r = (m_VariableInfo[token.v] as Var);
                token.v = "";
                return r;
            }
            if (m_GlobalVariableInfo.ContainsKey(token.v))
            {
                Var r = (m_GlobalVariableInfo[token.v] as Var);
                token.v = "";
                return r;
            }

            if (token.v == "at")
            {
                token.v = "";
                return new StringVar("@@");
            }
            if (token.v == "tab")
            {
                token.v = "";
                return new StringVar("\t");
            }

            if (token.v == "version")
            {
                token.v = "";
                return new StringVar("3.1.2");
            }
            if (token.v == "today")
            {
                token.v = "";
                return new StringVar(DateTime.Today.ToString("yyyyMMdd"));
            }
            if (token.v == "now")
            {
                token.v = "";
                return new StringVar(DateTime.Now.ToString("hhmmss"));
            }
            if (token.v == "guid")
            {
                token.v = "";
                return new StringVar(Guid.NewGuid().ToString());
            }
            if (token.v == "project")
            {
                token.v = "";
                return new StringVar(TemplateCache.Instance().projectfile);
            }

            // If we get here, there was trouble. Token unknown
            return null;// throw new UnknownTokenException(m_currentlinenr, token.v);
        }

        public FunctionInfo FunctionFinder(String name)
        {
            return (m_FunctionInfo[name] as FunctionInfo);
        }
        public Var FunctionEvaluator(String name, ArrayList parameterlist)
        {
            FunctionInfo fi = m_FunctionInfo[name] as FunctionInfo;
            ArrayList functionparameters = fi.GetParameterPrototypes();
            if (functionparameters.Count < parameterlist.Count)
                throw new TooManyFunctionParametersException(m_currentlinenr, fi.Name, functionparameters.Count, parameterlist.Count);

            Hashtable variables = new Hashtable();
            for (int pi = 0; pi < functionparameters.Count; pi++)
            {
                Var formalpar = functionparameters[pi] as Var;
                if (pi < parameterlist.Count)
                {
                    // We hebben een actuele parameter, controleer het type
                    Var actualpar = parameterlist[pi] as Var;
                    if (actualpar.GetType() != formalpar.GetType())
                        throw new FunctionParameterTypeMismatch(m_currentlinenr, fi.Name, pi, formalpar.GetType().ToString(), actualpar.GetType().ToString());

                    // Voeg hem toe onder de naam van de formele parameter in de variabelenlijst
                    variables.Add(formalpar.name, actualpar);
                }
                else
                {
                    // Neem het prototype ding, en plaats hem onder de naam van de
                    // parameter in de variabelen lijst
                    variables.Add(formalpar.name, formalpar);
                }
            }

            // We hebben parameters in de lijst. Maak een eigen lineprocessor aan om de functie uit te voeren.
            LineProcessor lp = new LineProcessor(new CurrentInfo(), m_GlobalVariableInfo, variables, fi.BodyCopy(), m_target, m_log, m_KeepInfo, m_FunctionInfo, m_currenttemplatename, fi);
            lp.AddObserver(this);
            m_errors += lp.Generate();
            lp.RemoveObserver(this);
            if (lp.FunctionResult == null)
            {
                if (fi.ResultType() != "Void")
                {
                    throw new ReturnValueMissingException(m_currentlinenr, fi.Name);
                }
                return new VoidVar();
            }
            else
            {
                return lp.FunctionResult;
            }
        }

        private void ProcessReturn(string line)
        {
            if (m_currentfunction == null)
            {
                throw new ReturnNotInFunctionUnexpectedException(m_currentlinenr);
            }

            if (m_current.state != InterpretationStatus.Active)
                return;
            string sExpression = line.Substring(6);
            if (sExpression.Trim() == "")
            {
                if (m_currentfunction.ResultType() != "Void")
                    throw new ReturnMissingExpressionException(m_currentlinenr);
            }
            else
            {
                if (m_currentfunction.ResultType() == "Void")
                    throw new ReturnExpressionUnexpectedException(m_currentlinenr);
                m_functionresult = m_evaluator.Evaluate(sExpression);
                if (m_functionresult.TypeName() != m_currentfunction.ResultType())
                    throw new ReturnUnexpectedTypeException(m_currentlinenr, m_currentfunction.ResultType(), m_functionresult.TypeName());
            }

            // We zijn zowiezo aan het eind van ons latijn. Knikker de rest van de source weg.
            m_source.Clear();
        }

        public Var FunctionResult
        {
            get
            {
                return m_functionresult;
            }
        }
        /// Looping (DO + LOOP)
        private Stack m_LoopStack;

        /// <summary>
        /// Process If (and If-related: Else, ElseIf, EndIf) commands.
        /// Evaluate the expression
        /// </summary>
        /// <param name="line"></param>
        private void ProcessIf(string line)
        {
            line = ProcessMacros(line);
            line += " "; // voor de zekerheid toevoegen.

            if (line.StartsWith("If") || line.StartsWith("ElseIf"))
            {
                string sCondition = "";
                if (line.StartsWith("If"))
                    sCondition = line.Substring(2);
                else
                    sCondition = line.Substring(6);

                // When If start new level, otherwise stay on current level
                if (line.StartsWith("If"))
                {
                    CurrentInfo newcurrent = new CurrentInfo();
                    newcurrent.type = m_current.type;
                    newcurrent.val = m_current.val;
                    m_Stack.Push(m_current);
                    if (m_current.state != InterpretationStatus.Active)
                    {
                        newcurrent.state = InterpretationStatus.Skip;
                        m_current = newcurrent;
                        return;
                    }
                    m_current = newcurrent;
                }
                else if (m_current.state == InterpretationStatus.Skip)
                {
                    // Stays skip
                    return;
                }
                else if (m_current.state == InterpretationStatus.Active)
                {
                    // Active -> Skip
                    m_current.state = InterpretationStatus.Skip;
                    return;
                }
                // It is a new If, or it was inactive, but now we have an ElseIf..

                try
                {
                    if (sCondition.Trim() == "")
                        throw new IfMissingConditionException(m_currentlinenr);

                    Var vConditionResult
                                                    = m_evaluator.Evaluate(sCondition);
                    if (vConditionResult.empty)
                        throw new ConditionResultException(m_currentlinenr, sCondition);
                    if (vConditionResult is BooleanVar)
                        m_current.state = ((vConditionResult as BooleanVar).v ? InterpretationStatus.Active : InterpretationStatus.Inactive);
                    else if (vConditionResult is DecimalVar)
                        m_current.state = ((vConditionResult as DecimalVar).v != 0 ? InterpretationStatus.Active : InterpretationStatus.Inactive);
                    else if (vConditionResult is StringVar)
                        m_current.state = ((vConditionResult as StringVar).v.Trim() != "" ? InterpretationStatus.Active : InterpretationStatus.Inactive);
                    else
                        throw new ConditionResultException(m_currentlinenr, sCondition);
                }
                catch (SyntaxErrorException)
                {
                    m_current.state = InterpretationStatus.Skip;
                    throw;
                }
                catch (Exception ex)
                {
                    m_current.state = InterpretationStatus.Skip;
                    if (ex.InnerException is SyntaxErrorException)
                        throw ex.InnerException;
                    throw;
                }
            }
            else if (line.StartsWith("Else"))
            {
                if (m_current.state == InterpretationStatus.Active)
                    m_current.state = InterpretationStatus.Inactive;
                else if (m_current.state == InterpretationStatus.Inactive)
                    m_current.state = InterpretationStatus.Active;
                // else it is Skip, and stays Skip
            }
            else if (line.StartsWith("EndIf"))
            {
                if (m_Stack.Count == 0)
                    throw new Exceptions.EndIfNotMatchedException(m_currentlinenr);

                m_current = (m_Stack.Pop() as CurrentInfo);
            }
            else
            {
                throw new UnknownCommandException(m_currentlinenr, line);
            }

        }

        /// <summary>
        /// Process Split command.
        /// parm1: the expression to evaluate. It should result in a string
        /// parm2: the prefix to put in front
        /// parm3: the non-last postfix
        /// parm4: the last postfix
        /// Evaluate the expression
        /// </summary>
        /// <param name="line"></param>
        /* private string[] ProcessSplit(string line)
        { 
            if (m_current.state != InterpretationStatus.Active)
                return null;

            line									= ProcessMacros(line);
            string						linestart	= line.Substring(0,	line.IndexOf("@"));
            string						part		= line.Trim().Trim('@');
            part									= part.Replace("\\:", "@");
            string[]					sCommandPart= part.Split(':');

            if (sCommandPart.Length != 5)
                throw new ApplicationException("Split command needs 5 parts: Split, expression, prefix, postfix, lastpostfix. ('" + line + "')");

            for (int p = 0; p < 5; p++)
                sCommandPart[p] = sCommandPart[p].Replace("@", ":");
		
            string						expression	= sCommandPart[1];
            Var							vExprResult	= m_evaluator.Evaluate(expression);
            string[]					split		= vExprResult.ToString().Replace("\r", "").Split('\n');

            int							i			= 0;
            for (; i < split.Length - 1; i++)
                split[i] = linestart + sCommandPart[2] + split[i] + sCommandPart[3];
            split[i] = linestart + sCommandPart[2] + split[i] + sCommandPart[4];

            return split;			
        }*/

        /// <summary>
        /// Describe the current context for as far as not evident.
        /// All but the lowest item in the stack is taken into account
        /// creating a string with the type and the name of the thing
        /// on the working stack.
        /// </summary>
        /// <returns>string describing the current processed item</returns>
        private string CurrentContext()
        {
            object[] myStack = new Object[m_Stack.Count + 1];
            myStack[0] = m_current;
            m_Stack.CopyTo(myStack, 1);

            // Skip the most obvious one and all same values on top of stack
            XmlNode previous_val = (myStack[myStack.GetUpperBound(0)] as CurrentInfo).val;

            string result = "";   // String.Format("line {0:0000}: ", m_currentlinenr);
            for (int i = myStack.GetUpperBound(0) - 1; i >= 0; i--)
            {
                CurrentInfo c = (myStack[i] as CurrentInfo);

                // Do not repeat same value on stack multiple times
                if (previous_val == c.val)
                    continue;

                string context = "";
                if (c.typetype != null)
                {
                    switch (c.typetype)
                    {
                        case "tables":
                            context = "Table: ";
                            break;
                        case "sets":
                            context = "Set:";
                            break;
                        case "attributes":
                            context = "Attr:";
                            break;
                        default:
                            context = c.typetype + ":";
                            break;
                    }
                }
                else
                {
                    context = "unknown:";
                }
                // Add the name...
                string attrname;
                if (c.type.Attributes["nameattribute"] != null)
                {
                    attrname = c.type.Attributes["nameattribute"].Value;
                }
                else
                {
                    if (c.type.Name == "sets" || c.type.Name == "attributes")
                    {
                        attrname = c.type.SelectSingleNode("element[@type='Name']").Attributes["name"].Value;
                    }
                    else if (c.type.Attributes["type"] != null && c.type.Attributes["type"].Value == "AttributeSet")
                    {
                        // The selected attributes in the set get their
                        // name from the AttributeCombobox 
                        XmlNode namenode = c.type.SelectSingleNode("element[@type='AttributeCombobox']");
                        if (namenode == null)
                        {
                            namenode = c.type.SelectSingleNode("element[@type='Name']");
                        }
                        if (namenode != null)
                        {
                            attrname = namenode.Attributes["name"].Value;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "Name giving element unknown in context description. 'name' is assumed but this is dangerous. This messages can be ignored, but better is to find a way to get the right name attribute");
                            attrname = "name";
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "Name giving element unknown in context description. 'name' is assumed but this is dangerous. This messages can be ignored, but better is to find a way to get the right name attribute");
                        attrname = "name";
                    }
                }
                try
                {
                    XmlNode namenode = c.val.SelectSingleNode(attrname);
                    if (namenode != null)
                        context += " " + namenode.InnerText;
                }
                catch
                {
                    context += "[ex:name unknown]";
                }

                result = result + context + ";	";
                previous_val = c.val;
            }
            return result;
        }

        /// <summary>
        /// Process Error: command
        /// Evaluate the expression to give the error message
        /// </summary>
        /// <param name="line"></param>
        private void ProcessError(string line)
        {
            if (m_current.state != InterpretationStatus.Active)
                return;

            line = ProcessMacros(line);
            string msg = line.Substring(5);

            try
            {
                Var vMsgResult = m_evaluator.Evaluate(msg);
                String message = CurrentContext() + vMsgResult.ToString();
                WriteError(message);
                m_errors++;
            }
            catch (Exception ex)
            {
                WriteError(CurrentContext() + "Evaluation of '" + msg + "' raised exception: " + ex.Message);
                m_errors++;
            }
        }
        /// <summary>
        /// Process Info: command
        /// Evaluate the expression to give the error message
        /// </summary>
        /// <param name="line"></param>
        private void ProcessInfo(string line)
        {
            if (m_current.state != InterpretationStatus.Active)
                return;

            line = ProcessMacros(line);
            string msg = line.Substring(4);

            try
            {
                Var vMsgResult = m_evaluator.Evaluate(msg);
                String message = /* CurrentContext() + */ vMsgResult.ToString();
                WriteInfo(message);
            }
            catch (Exception ex)
            {
                WriteError(CurrentContext() + "Evaluation of '" + msg + "' raised exception: " + ex.Message);
                m_errors++;
            }
        }

        /// <summary>
        /// Process the Do command.
        /// First parameter is the value to iterate over. This can be 'attributes'
        /// or 'sets' or the name of another parameter. No specific type is needed.
        /// This could become necessary in the future to allow for iteration
        /// over values like "1=Blue,2=Orange,3=Violet".
        /// </summary>
        /// <param name="line"></param>
        private void ProcessDo(string line)
        {

            Match m = new Regex(@"^Do\s+(\S+)(\s.*)?$").Match(line);
            string sConcept = "";
            if (m.Groups[1].Success)
                sConcept = m.Groups[1].Value;
            string sRest = "";
            if (m.Groups[2].Success)
                sRest = m.Groups[2].Value.Trim();// line.Substring(m.Length).Trim();

            //string[]					parts		= line.Substring(1,	line.LastIndexOf("@") -1).Split(':');
            XmlNode type = null;
            string typetype = "";
            XmlNode val;
            XmlNodeList todo = null;
            Looptype lt = Looptype.None;
            string element = "";
            string filter = "";
            string orderby = "";

            // If not active, push skip-level on stack to react on right @Loop@ command
            if (m_current.state != InterpretationStatus.Active)
            {
                val = null;
                lt = Looptype.None;
            }
            else
            {
                val = GetValueNodeFor(sConcept, out	type, out typetype);
                if (val == null)
                //catch (FieldNotFoundException ex)
                {
                    // First check if parts[1] is not a global typename
                    // loop over instances over that type is meant
                    //					System.Diagnostics.Debug.WriteLine("Current typedef definitions:");
                    //					foreach (string s in TemplateCache.Instance().GetTypenamesList("TypeDefs"))
                    //					{
                    //						System.Diagnostics.Debug.WriteLine(s);
                    //					}
                    if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), sConcept) < 0)
                    {
                        val = null;
                        lt = Looptype.None;
                        orderby = "";
                        WriteError(CurrentContext() + "Do statement: '" + sConcept + "' unknown");
                        m_errors++;
                    }
                    else
                    {
                        // loop over that type 
                        type = TemplateCache.Instance().GetTemplateType(sConcept);
                        typetype = "collection";
                        val = TemplateCache.Instance().GetTypesList(sConcept);
                    }
                }
                if (typetype == "collection")
                {
                    lt = Looptype.Collection;
                    element = "type";
                    orderby = "";
                }
                else if (typetype == "sets")
                {
                    lt = Looptype.Sets;
                    element = "set";
                    orderby = type.SelectSingleNode("element[@type='Order']").Attributes["name"].Value;
                }
                else if (typetype == "attributes")
                {
                    lt = Looptype.Attributes;
                    element = "attribute";
                    orderby = type.SelectSingleNode("element[@type='Order']").Attributes["name"].Value;
                }
                else if (typetype == "fields")
                {
                    lt = Looptype.Field;	//??
                    element = "field";
                    orderby = ""; // no	specific ordering
                }

                if (sRest != "")
                {
                    //filter = "[" + ProcessLine(parts[2].Trim()) + "]";
                    filter = "[" + m_evaluator.Evaluate(sRest).ToString() + "]";
                }

                if (lt == Looptype.Field)
                {
                    val = BuildLoopFieldValue(val, type);
                }
                else if (lt == Looptype.Collection)
                {
                    val = BuildLoopCollectionValue(val, type);
                }

                if (val == null)
                    todo = null;
                else
                    todo = val.SelectNodes(element + filter);

            }
            LoopInfo loop = new LoopInfo(lt, todo, orderby, m_source);
            if (m_LoopStack == null)
                m_LoopStack = new Stack();

            CurrentInfo newcurrent = new CurrentInfo();
            newcurrent.type = type;
            newcurrent.typetype = typetype;
            if (loop.iterator == null || !loop.iterator.MoveNext())
            {
                newcurrent.state = InterpretationStatus.Skip;
                newcurrent.val = null;
            }
            else
            {
                newcurrent.state = m_current.state;
                newcurrent.val = (loop.iterator.Current as XmlNode);
            }

            m_LoopStack.Push(loop);
            m_Stack.Push(m_current);
            m_current = newcurrent;
        }

        /// <summary>
        /// Process the While command.
        /// First parameter is the expression to test, while true, continue
        /// </summary>
        /// <param name="line"></param>
        private void ProcessWhile(string line)
        {
            string sCondition = line.Substring(5);
            Looptype lt;

            // If not active, push skip-level on stack to react on right @Loop@ command
            if (m_current.state != InterpretationStatus.Active)
            {
                lt = Looptype.None;
            }
            else
            {
                lt = Looptype.Expression;
            }

            LoopInfo loop = new LoopInfo(lt, sCondition, m_source);
            if (m_LoopStack == null)
                m_LoopStack = new Stack();

            CurrentInfo newcurrent = new CurrentInfo();
            newcurrent.type = m_current.type;
            newcurrent.typetype = m_current.typetype;
            newcurrent.state = m_current.state;
            newcurrent.val = m_current.val;

            m_LoopStack.Push(loop);
            m_Stack.Push(m_current);
            m_current = newcurrent;

            if (m_current.state == InterpretationStatus.Active)
            {
                try
                {
                    Var evalresult = m_evaluator.Evaluate(sCondition);
                    if (evalresult.GetType() == typeof(BooleanVar))
                    {
                        if (!(evalresult as BooleanVar).v)
                        {
                            m_current.state = InterpretationStatus.Skip;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_current.state = InterpretationStatus.Skip;
                    if (ex.InnerException is SyntaxErrorException)
                    {
                        throw ex.InnerException;
                    }
                    WriteError(CurrentContext() + "Evaluation of '" + sCondition + "' raised exception: " + ex.Message);
                }
            }
        }

        private XmlNode BuildLoopFieldValue(XmlNode val, XmlNode type)
        {
            if (type == null)
                return null;

            if (type.Attributes["type"].Value != "LoopField")
                throw new ApplicationException("Field '" + type.Attributes["name"].Value + "' must have type 'LoopField' to be used in Do-command");

            // Split the value in parts: code1=description1|code2=description2
            // complete items are put in name.
            XmlNode fieldsnode = val.OwnerDocument.CreateElement("fields");
            if (val != null && val.InnerText.Trim() != "")
            {
                char fieldsep = TemplateGenerator.DEFAULT_FIELD_SEPERATOR;
                if (type.Attributes["fieldsep"] != null)
                    fieldsep = type.Attributes["fieldsep"].Value[0];
                char partsep = TemplateGenerator.DEFAULT_PART_SEPERATOR;
                if (type.Attributes["partsep"] != null)
                    partsep = type.Attributes["partsep"].Value[0];

                foreach (String name in val.InnerText.Split(fieldsep))
                {
                    XmlNode fieldnode = fieldsnode.OwnerDocument.CreateElement("field");
                    string[] fieldparts = name.Trim().Split(partsep);
                    XmlNode elm = type.FirstChild;
                    for (int i = 0; i < fieldparts.GetLength(0); i++)
                    {
                        fieldnode.AppendChild(fieldnode.OwnerDocument.CreateElement(elm.Attributes["name"].Value));
                        fieldnode.LastChild.InnerText
                                                    = fieldparts[i].Trim();
                        if (elm.NextSibling != null)
                            elm = elm.NextSibling;
                    }
                    fieldsnode.AppendChild(fieldnode);
                }
            }
            return fieldsnode;
        }


        /// <summary>
        /// Build a cloned list of all element to process. In the continuation 
        /// of the loop an element can be dropped from the list.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private XmlNode BuildLoopCollectionValue(XmlNode val, XmlNode type)
        {
            //if (type.Attributes["type"].Value != "TableCollection")
            //	throw new ApplicationException("Field '" +  type.Attributes["name"].Value + "' must have type 'TableCollection' to be used in Do-command");

            // Collect all tabels for this project and put them in a list.
            XmlNode collectnode = val.OwnerDocument.CreateElement("collection");
            if (val != null)
            {
                foreach (XmlNode t in val.ChildNodes)
                {
                    if (t.FirstChild != null)
                        collectnode.AppendChild(t.FirstChild.Clone());
                }
            }
            return collectnode;
        }

        private void ProcessLoop()
        {
            //			if (m_current.state != InterpretationStatus.Active)
            //				return;
            //
            LoopInfo loop = m_LoopStack.Peek() as LoopInfo;
            if (loop.type == Looptype.Expression)
            {

                try
                {
                    Var evalresult = m_evaluator.Evaluate(loop.expression);
                    if (evalresult.GetType() == typeof(BooleanVar))
                    {
                        if ((evalresult as BooleanVar).v)
                        {
                            m_source = loop.loopsource;
                            m_log.WriteInfo("Loop next");
                        }
                        else
                        {
                            m_LoopStack.Pop();
                            m_log.WriteInfo("Loop ended");
                            m_current = (m_Stack.Pop() as CurrentInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteError(CurrentContext() + "Evaluation of '" + loop.expression + "' raised exception: " + ex.Message);
                    m_LoopStack.Pop();
                    m_log.WriteInfo("Loop ended");
                    m_current = (m_Stack.Pop() as CurrentInfo);
                }
            }
            else if (loop.iterator != null && loop.iterator.MoveNext())
            {
                m_current.val = (loop.iterator.Current as XmlNode);
                m_source = loop.loopsource;
                m_log.WriteInfo("Loop next");
            }
            else
            {
                m_LoopStack.Pop();
                m_log.WriteInfo("Loop ended");
                m_current = (m_Stack.Pop() as CurrentInfo);
            }

        }
        // Get out of any loop.
        private void ProcessBreak()
        {
            // Put the state to skip 
            m_current.state = InterpretationStatus.Skip;

            // move the iterator to the end of the list
            LoopInfo loop = m_LoopStack.Peek() as LoopInfo;

            while (loop.iterator != null && loop.iterator.MoveNext())
            {
            }
        }

        private void ProcessVarDeclaration(string sLineTrim)
        {
            if (m_current.state != InterpretationStatus.Active)
                return;

            Match m = new Regex(@"^(Var|Global)\s+(\w+)\s+(\w+)(\s*=\s*(.+))?$").Match(sLineTrim);
            if (m.Groups.Count < 2)
                throw new SyntaxErrorException(m_currentlinenr, sLineTrim);
            bool blnGlobal = m.Groups[1].Value == "Global";
            string sVarType = m.Groups[2].Value;
            string sVarName = m.Groups[3].Value;
            string sVarInit = "";
            if (m.Groups.Count > 5)
                sVarInit = m.Groups[5].Value;

            Var v = null;
            if (m_VariableInfo.ContainsKey(sVarName))
            {
                v = (m_VariableInfo[sVarName] as Var);
            }
            else
            {
                switch (sVarType)
                {
                    case "Boolean":
                        v = new BooleanVar();
                        break;
                    case "Decimal":
                        v = new DecimalVar();
                        break;
                    case "String":
                        v = new StringVar();
                        break;
                    default:
                        XmlNode ttype = null;
                        foreach (XmlNode n in TemplateCache.Instance().GetTypesList("TypeDefs"))
                        {
                            if (n.Attributes["name"].Value == sVarType)
                            {
                                ttype = n;
                                break;
                            }
                        }
                        if (ttype == null)
                            throw new ApplicationException(String.Format("Variable declaration: concept type not found: ('{0}')", sVarType));

                        v = new ConceptVar(ttype);
                        break;
                }
                v.name = sVarName;
                if (blnGlobal)
                {
                    m_GlobalVariableInfo[v.name] = v;
                }
                else
                {
                    m_VariableInfo[v.name] = v;
                }
            }

            if (sVarInit != "")
                m_evaluator.Evaluate(sVarName + "=" + sVarInit);
            else
                v.Clear();

            if (blnGlobal && sVarName == "breakiebreakie")
            {
                //System.Diagnostics.Debug.Assert(false);
            }
        }

        //		private string ProcessToken(string token)
        //		{
        //			XmlNode t;
        //			
        //			if (token == "Tab")
        //				return "\t";
        //
        //			try 
        //			{
        //				XmlNode x = GetValueNodeFor(token, out t);
        //				if (x != null)
        //					return x.InnerText;
        //				else
        //					return "";
        //			}
        //			catch (Exception e)
        //			{
        //				return e.Message;
        //			}
        //		}

        /// <summary>
        /// Get the value node for the given token, and set the findtype value 
        /// to the found type. Fill the type of type in the typetype.
        /// Typetype is the name of the type: "normal", "sets", "attributes",
        ///  or temlatename "TableDefinition", "Project", ...
        /// </summary>
        /// <param name="token"></param>
        /// <param name="foundtype"></param>
        /// <param name="foundtypetype"></param>
        /// <returns></returns>
        private XmlNode GetValueNodeFor(string token, out XmlNode foundtype, out string foundtypetype)
        {
            // Split the token in ':' parts
            // split the first part in '.' separated parts.
            //string[]					major		= token.Split(':');
            string[] minor = token.Split('.');
            XmlNode type;
            XmlNode val;
            string typetype;
            int stackindex;

            // We kunnen kiezen: een variabele doorakkeren, en dan gaan we dus niet de 
            // stack aflopen. Anders kijken we de current door, en daarna alles uit de stack
            // ook nog.
            if (m_VariableInfo.ContainsKey(minor[0]) || m_GlobalVariableInfo.ContainsKey(minor[0]))
            {
                Var v = (m_VariableInfo[minor[0]] as Var);
                if (v == null) // dan was het toch een globale variabele
                    v = m_GlobalVariableInfo[minor[0]] as Var;

                if (v.GetType() != typeof(ConceptVar))
                {
                    foundtype = null;
                    foundtypetype = "";
                    return null;
                }
                // Als we maar 1 element hebben willen de variabele niet teruggeven.
                // De variabele moet als l-value kunnen dienen, en dat kunnen resultaten 
                // van deze functie niet.
                if (minor.Length == 1)
                {
                    foundtype = null;
                    foundtypetype = "";
                    return null;
                }

                ConceptVar r = (v as ConceptVar);
                type = r.typedef;//.SelectSingleNode("template");
                val = r.tvalue;
                System.Diagnostics.Debug.Assert(type.Name == "template");
                typetype = "type";
                stackindex = m_Stack.Count;
                // Kap het eerste ding ervanaf. De variabele naam hebben we gehad.
                minor = token.Substring(minor[0].Length + 1).Split('.');
            }
            else
            {
                type = m_current.type;
                val = m_current.val;
                typetype = m_current.typetype;
                stackindex = 0;
            }

            // When not found in current element, walk the stack up to 
        // try to find it there. 
        again:

            foreach (string v in minor)
            {
                // find the element definition in the current type.
                XmlNode subtype = null;
                if (v == "Current")
                    subtype = type;
                else if (type != null && (type.Name == "sets" || type.Name == "attributes" || (type.Attributes["type"] != null && type.Attributes["type"].Value == "LoopField")))
                    subtype = type.SelectSingleNode("element[@name='" + v + "']");
                else if (v == "attributes" || v == "sets")
                    subtype = type.SelectSingleNode(v);
                else if (type != null && type.Attributes["type"] != null && type.Attributes["type"].Value == "AttributeSet")
                    subtype = type.SelectSingleNode("element[@name='" + v + "']");
                else if (type != null)
                    subtype = type.SelectSingleNode("elements/element[@name='" + v + "']");

                // If element not found, but busy in attributeset, process the attribute
                if (subtype == null && type != null && type.Attributes["type"] != null && type.Attributes["type"].Value == "AttributeSet")
                {
                    XmlNode namegivingelement
                                                    = type.SelectSingleNode("element[@type='Name']");
                    if (namegivingelement == null)
                        namegivingelement = type.SelectSingleNode("element[@type='AttributeCombobox']");
                    if (namegivingelement != null)
                    {
                        string namegiving = namegivingelement.Attributes["name"].Value;
                        type = type.ParentNode.ParentNode.SelectSingleNode("attributes");
                        XmlNode attnamegivingelement = type.SelectSingleNode("element[@type='Name']");
                        System.Diagnostics.Debug.Assert(attnamegivingelement != null);
                        string attnamegiving = attnamegivingelement.Attributes["name"].Value;
                        val = val.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("attributes/attribute[" + attnamegiving + "='" + val.SelectSingleNode(namegiving).InnerText + "']");
                        subtype = type.SelectSingleNode("element[@name='" + v + "']");
                    }
                }
                if (subtype == null) // && ((type.Name == "attributes") || type.Name == "sets"))
                {
                    // try the parent context...
                    if (m_Stack.Count > 0 && stackindex < m_Stack.Count)
                    {
                        CurrentInfo st = ((m_Stack.ToArray())[stackindex] as CurrentInfo);
                        type = st.type;
                        val = st.val;
                        typetype = st.typetype;
                        stackindex++;
                        goto again;
                    }
                }

                if (subtype == null)
                {
                    // Als we gewoon een naam in de token hebben, en het ding bestaat niet ergens in een concept op de stack
                    // is er niks aan de hand. Zeggen we gewoon: niet gevonden. Misschien is het wel een 'vaste' waarde?
                    if (!token.Contains("."))
                    {
                        foundtype = null;
                        foundtypetype = "";
                        return null;
                    }
                    string msg = String.Format("Token '{0}' could not be resolved", token);
                    //experimentje om te kijken of de filenaam er makkelijk uit kan.
                    /* if (type.Attributes["sourcefile"] == null)
                        msg = msg + type.Name + "'";
                    else
                    {
                        msg = msg + Path.GetFileNameWithoutExtension(type.Attributes["sourcefile"].Value) + "'";
                    }*/
                    throw new Generator.Exceptions.FieldNotFoundException(msg, "", token);
                }

                XmlNode newval = val;
                if (val != null && v != "Current")
                {
                    newval = val.SelectSingleNode(v);
                }

                XmlNode typeattr = subtype.Attributes["type"];
                if (v == "Current")
                {
                    type = subtype;
                    typetype = "type";
                }
                else if (typeattr == null)
                {
                    type = subtype;
                    val = newval;
                    if (subtype.Name == "attributes")
                        typetype = "attributes";
                    else if (subtype.Name == "sets")
                        typetype = "sets";
                    else
                        typetype = "normal";
                }
                else if (typeattr.Value == "Text"
                || typeattr.Value == "Name"
                || typeattr.Value == "Guid"
                || typeattr.Value == "Number"
                || typeattr.Value == "Checkbox"
                || typeattr.Value == "Combobox"
                || typeattr.Value == "Order"
                )
                {
                    type = subtype;
                    val = newval;
                    typetype = "normal";
                }
                else if (typeattr.Value == "LoopField")   // tw added 15-11-2020, movedfrom hierboven
                {
                    type = subtype;
                    val = newval;
                    typetype = "fields";
                }
                else if (typeattr.Value == "AttributeCombobox")
                {
                   // type = type.SelectSingleNode("attributes");
                    typetype = "attributes";
                    if (newval.InnerText != "")
                        val = val.ParentNode.SelectSingleNode(@"attributes/attribute[name='" + newval.InnerText + "']");
                    else
                        val = null;
                }
                else if (typeattr.Value == "AttributeSet")
                {
                    type = subtype;
                    typetype = "attributes";
                    val = newval;
                }
                //				else // Check if value is a type name. 
                //					if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), typeattr.Value) >=0 )
                //				{
                //					
                //					type		= TemplateCache.Instance().GetTemplateType(typeattr.Value);
                //					typetype	= "children";
                //					val			= newval;
                //				}
                else
                {
                    // Get the template definition and the contents from the cache 
                    type = TemplateCache.Instance().GetTemplateType(typeattr.Value);
                    typetype = typeattr.Value;
                    if (newval != null && newval.InnerText.Trim() != "")
                        val = TemplateCache.Instance().GetValueFor(typeattr.Value, newval.InnerText);
                    else
                        val = null;
                }
            }

            // All subitems are processed.
            foundtype = type;
            foundtypetype = typetype;
            return val;
        }


        /// <summary>
        /// KEEP info
        /// </summary>
        #region Keep area

        private string GetKeepKey(string line, int pos)
        {
            string key = line.Substring(pos + KEEP_START_LEN);
            key = key.Substring(0, key.IndexOf("@"));
            return key;
        }
        private bool ProcessKeepInfo(string line)
        {
            if (m_current.state != InterpretationStatus.Active)
                return false;

            int iKeepStart = line.IndexOf(KEEP_START);
            if (iKeepStart >= 0)
            {
                string key = GetKeepKey(line, iKeepStart);
                int iKeyStart = line.IndexOf(key);
                //key = this.ProcessLine(key);
                key = m_evaluator.Evaluate(key).ToString();

                if (m_KeepInfo.ContainsKey(key))
                {
                    string[] kept = (m_KeepInfo[key] as string[]);
                    // insert the prologue of the keep before the kept text
                    if (iKeepStart > 0)
                        kept[0] = line.Substring(0, iKeepStart) + kept[0];

                    // Remove all lines from the template
                    int iKeepEnd = -1;
                    while (m_source.Count > 0 && (iKeepEnd = line.IndexOf(KEEP_END)) < 0)
                    {
                        line = m_source[0];
                        m_source.RemoveAt(0);
                    }
                    if (iKeepEnd >= 0 && iKeepEnd + KEEP_END.Length < line.Length)
                    {
                        kept[kept.Length - 1] += line.Substring(iKeepEnd + KEEP_END.Length);
                    }

                    m_target.AddRange(kept);
                    foreach (string s in kept)
                        m_log.WriteKept(s);
                }
                else
                {
                    // Not in our kept cache. Process as normal.
                    // but output start keep with interpreted key.
                    string oline = line.Substring(0, iKeyStart) + key + line.Substring(line.LastIndexOf("@"));
                    m_target.Add(oline);
                    m_log.WriteOutput(oline);
                }
                return true;

            }
            if (line.IndexOf(KEEP_END) > 0)
            {
                m_target.Add(line);
                m_log.WriteOutput(line);
                return true;
            }

            return false;
        }
        #endregion

        #region Macros area
        private Hashtable m_MacroInfo;

        public string ProcessMacro(string line)
        {
            string key;
            string val;

            if (m_MacroInfo == null)
                m_MacroInfo = new Hashtable();

            int i = line.IndexOf(":", 8);
            if (i < 0)
            {
                key = line.Substring(7).Trim('@').Trim();
                if (m_MacroInfo.ContainsKey(key))
                    m_MacroInfo.Remove(key);
            }
            else
            {
                string[] vals = new String[2];
                key = line.Substring(7, i - 7);
                val = line.Substring(i + 1).Trim('@').Trim();
                i = val.IndexOf("==>");
                if (i <= 0)
                {
                    if (m_MacroInfo.ContainsKey(key))
                        m_MacroInfo.Remove(key);
                }
                else
                {
                    vals[0] = val.Substring(0, i).Trim();
                    vals[1] = val.Substring(i + 3).Trim();
                    if (m_MacroInfo.ContainsKey(key))
                        m_MacroInfo[key] = vals;
                    else
                        m_MacroInfo.Add(key, vals);
                }
            }
            return "";
        }
        public string ProcessMacros(string line)
        {
            if (m_MacroInfo == null)
                return line;

            foreach (string[] s in m_MacroInfo.Values)
                line = line.Replace(s[0], s[1]);

            return line;
        }
        #endregion

        #region Functions area

        #endregion

        private void WriteError(string s)
        {
            //if (m_currentfunction == null)
            //    Notify(s, NotificationType.Erreur, m_currenttemplatename, m_currentlinenr);
            //else
            //    Notify(s, NotificationType.Erreur, new SourceCodeContext(m_currentfunction.DeclaredAt.Filename, m_currentlinenr.Linenr);
            Notify(s, NotificationType.Erreur, m_currentlinenr);
            m_log.WriteError(s);
        }
        private void WriteInfo(string s)
        {
            //if (m_currentfunction == null)
            //    Notify(s, NotificationType.Info, m_currenttemplatename, m_currentlinenr.Linenr);
            //else
            //    Notify(s, NotificationType.Info, m_currentfunction.DeclaredAt.Filename, m_currentlinenr.Linenr);
            Notify(s, NotificationType.Info, m_currentlinenr);
            m_log.WriteInfo(s);
        }
   }
}
