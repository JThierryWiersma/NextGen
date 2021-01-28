using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Generator.VisitorPattern;
using Generator.Utility;
using Generator.Statements;

namespace Generator.CodeProcessors
{
    public class CodePrinter : CodeProcessor, ICodeLineVisitor
    {
        RichTextBox txtText;
        public CodePrinter(RichTextBox t)
        {
            txtText = t;
        }
        public void Visit(CodeLine l)
        {
            String line = l.Content;
            while (line != "")
            {
                int firstatpos = line.IndexOf('@');
                if (firstatpos < 0)
                {
                    ColorCode(line);
                    break;
                }
                if (firstatpos > 0)
                    ColorCode(line.Substring(0, firstatpos));
                ColorCommand("@");
                int secondatpos = line.IndexOf('@', firstatpos + 1);
                if (secondatpos < 0)
                {
                    ColorError(line.Substring(firstatpos + 1));
                    return;
                }
                ColorExpression(line.Substring(firstatpos + 1, secondatpos - firstatpos - 1));
                ColorCommand("@");
                line = line.Substring(secondatpos + 1);
            }
            //NextLine();
        }

        public void Visit(Comment l)
        {
            ColorCommand(l);
            ColorComment(l.pComment + l.Comment);
            //NextLine();
        }

        public void Visit(BreakStatement l)
        {
            VisitStatementComment(l);
        }

        public void Visit(ReturnStatement l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(ExitStatement l)
        {
            VisitStatementComment(l);
        }

        public void Visit(VarDeclaration l)
        {
            ColorCommand(l);
            if (IsClassnameKnown(l.VarType))
            {
                ColorClass(l.pVarType + l.VarType);
            }
            else
            {
                ColorError(l.pVarType + l.VarType);
            }
            ColorExpression(l.pVarName + l.VarName + l.pAssign + l.InitialisationExpression);
            ColorError(l);
            //NextLine();
        }

        public void Visit(GlobalVarDeclaration l)
        {
            Visit(l as VarDeclaration);
        }

        public void Visit(InfoMessage l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(ErrorMessage l)
        {
            VisitStatementExpressionComment(l);
        }

        /// <summary>
        /// Doe een standaard statement met een expressie en evt een commentaar
        /// </summary>
        /// <param name="l"></param>
        private void VisitStatementExpressionComment(Statement l)
        {
            ColorCommand(l);
            ColorExpression(l.pParam + l.Param);
            ColorError(l);
            //NextLine();
            return;
        }
        /// <summary>
        /// Doe een standaard statement ZONDER een expressie maar met evt een commentaar
        /// </summary>
        /// <param name="l"></param>
        private void VisitStatementComment(Statement l)
        {
            ColorCommand(l);
            ColorError(l.pParam + l.Param); // zou in unrecognised moeten staan!
            ColorError(l);
            //NextLine();
            return;
        }

        public void Visit(IfStatement l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(ElseIfStatement l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(ElseStatement l)
        {
            VisitStatementComment(l);
        }

        public void Visit(EndIfStatement l)
        {
            VisitStatementComment(l);
        }

        public void Visit(DoStatement l)
        {
            ColorCommand(l);
            if (IsClassnameKnown(l.Param))
            {
                ColorClass(l.pParam + l.Param);
            }
            else
            {
                ColorExpression(l.pParam + l.Param);
            }
            ColorError(l);
            //NextLine();
        }

        public void Visit(WhileStatement l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(LoopStatement l)
        {
            VisitStatementComment(l);
        }

        public void Visit(Include l)
        {
            VisitStatementExpressionComment(l);
        }

        public void Visit(FunctionDeclaration l)
        {
            ColorCommand(l);
            if (l.ResultType == "Void" || IsClassnameKnown(l.ResultType))
            {
                ColorClass(l.pResultType + l.ResultType);
            }
            else
            {
                ColorError(l.pResultType + l.ResultType);
            }
            ColorExpression(l.pName + l.Name);
            if (l.ParListFound)
            {
                ColorCommand(l.pParList + "(");
                bool first = true;
                foreach (FunctionParameterDeclaration fpd in l.ParList)
                {
                    if (first)
                        first = false;
                    else
                        ColorExpression(",");
                    if (fpd.ParType == String.Empty)
                    {
                        ColorError(fpd.Unrecognised);
                        continue;
                    }
                    if (IsClassnameKnown(fpd.ParType))
                        ColorClass(fpd.pParType + fpd.ParType);
                    else
                        ColorError(fpd.pParType + fpd.ParType);
                    if (fpd.pParName == string.Empty)
                    {
                        ColorError(fpd.Unrecognised);
                        continue;
                    }
                    ColorExpression(fpd.pParName + fpd.ParName + fpd.pNext);
                }
                ColorCommand(")");
            }
            ColorError(l); // wat onherkend + comment.
            //NextLine();
            return;
        }

        public void Visit(EndFunctionDeclaration l)
        {
            VisitStatementComment(l);
        }

        public void Visit(Assignment l)
        {
            ColorCommand(l);
            ColorExpression(l.Varname + l.pAssign + l.Assign + l.Expression);
            ColorError(l);
            //NextLine();
        }

        public void Visit(VoidFunctionCall l)
        {
            ColorCommand(l);
            ColorExpression(l.Expression);
            ColorError(l);
            //NextLine();
        }

        // delegate void ColorLineCallback(int line, Color c);
        /*private void ColorLine(int line, Color c)
        {
            if (txtText.InvokeRequired)
            {
                ColorLineCallback cb = new ColorLineCallback(ColorLine);
                this.Invoke(cb, new object[] {line, c});
                return;
            }

            int startpos = txtText.GetFirstCharIndexFromLine(line);
            int length;
            if (line + 1 == txtText.Lines.Length)
            {
                length = txtText.TextLength - startpos;
            }
            else
            {
                length = txtText.GetFirstCharIndexFromLine(line + 1) - 1 - startpos;
            }
            txtText.Select(startpos, length);
            txtText.SelectionColor = c;
        }*/
        /// <summary>
        /// Zorg dat er een newline achter komt
        /// </summary>
        /// <param name="line"></param>
        /// <param name="c"></param>
        private void ColorLine(string line, Color c)
        {
            ColorString(line + '\n', c);
        }
        /// <summary>
        /// Voeg de string toe en kleur het in de gegeven kleur
        /// </summary>
        /// <param name="part"></param>
        /// <param name="c"></param>
        private void ColorString(string part, Color c)
        {
            if (part == String.Empty)
                return;
            int start = txtText.SelectionStart; 
            //txtText.SelectionStart = txtText.TextLength;
            txtText.SelectionColor = c;
            txtText.SelectedText = part;
            txtText.SelectionStart = start + part.Length;
        }
        public void NextLine()
        {
            txtText.SelectedText = "\n";
            txtText.SelectionStart++;
        }
        private void ColorError(string c)
        {
            ColorString(c, OptionsSettings.Instance().ErrorColor);
        }
        /// <summary>
        /// Kleur het 'unrecognised' stuk, evt gevolgd door nog wat commentaar
        /// </summary>
        /// <param name="l"></param>
        private void ColorError(Statement l)
        {
            ColorError(l.Unrecognised);
            ColorComment(l.pComment + l.Comment);
        }
        private void ColorCommand(string c)
        {
            ColorString(c, OptionsSettings.Instance().CommandColor);
        }
        /// <summary>
        /// Kleur het begin van het statement
        /// </summary>
        /// <param name="l"></param>
        private void ColorCommand(Statement l)
        {
            ColorCommand(l.pAt + "@" + l.pCommand + l.Command);
        }
        private void ColorComment(string c)
        {
            ColorString(c, OptionsSettings.Instance().CommentColor);
        }
        private void ColorCode(string c)
        {
            ColorString(c, OptionsSettings.Instance().CodeColor);
        }
        private void ColorClass(string c)
        {
            ColorString(c, OptionsSettings.Instance().ClassColor);
        }

        /// <summary>
        /// De string uitsnorren als een expressie en toevoegen aan de 
        /// text in de juiste kleuren
        /// </summary>
        /// <param name="exp"></param>
        private void ColorExpression(string expression) //, int linenr)
        {
            ColorString(expression, OptionsSettings.Instance().ExpressionColor);
            return;
            /*
            string processed = "";
            int i;

            while (expression != "")
            {
                // Allow for continue from multiple levels.
                bool bGotoNextPart
                                                    = false;

                // first eat all spaces
                if (Char.IsWhiteSpace(expression[0]))
                {
                    do
                    {
                        processed += expression[0];
                        expression = expression.Remove(0, 1);
                    } while (Char.IsWhiteSpace(expression[0]));
                    continue;
                }

                // Look for an operator. If found process it...
                foreach (OperatorInfo op in OperatorInfo.cvOperators.Values)
                {
                    if (expression.StartsWith(op.name) && (!op.IsText || !Char.IsLetter(expression[op.name.Length])))
                    {
                        processed += op.name;
                        expression = expression.Remove(0, op.name.Length);
                        bGotoNextPart = true;
                        break;
                    }
                }
                if (bGotoNextPart)
                    continue;

                // if (bLastTokenWasOperand)
                //    throw new ApplicationException(String.Format("Syntax error. Operator expected ('{0}')", expression));

                // Now check to see if first part of expression is a number
                if (Char.IsDigit(expression, 0))
                {
                    i = 0;
                    // Eat the integer part of the number
                    do
                    {
                        i++;
                    } while (Char.IsDigit(expression, i));
                    if (expression[i] == '.')
                    {
                        // It is a decimal
                        do
                        {
                            i++;
                        } while (Char.IsDigit(expression, i));
                    }
                    processed = expression.Substring(0, i);
                    expression = expression.Remove(0, i);
                    continue;
                }

                // Check if first part is string constant
                if (expression[0] == '\"')
                {
                    i = 1;
                    while (i < expression.Length && expression[i] != '\"')
                    {
                        if (expression[i] == '\\')
                        {
                            if ((i + 1) == expression.Length)
                            {
                                ColorString(processed, OptionsSettings.Instance().ExpressionColor);
                                ColorString(expression, OptionsSettings.Instance().ErrorColor);
                                return;
                                //throw new ApplicationException("Syntax error in expression:'" + expression + "'. String terminator expected");
                            }
                            // Shift additional position
                            i++;
                        }
                        i++;
                    }
                    if (i >= expression.Length)
                    {
                        ColorString(processed, OptionsSettings.Instance().ExpressionColor);
                        ColorString(expression, OptionsSettings.Instance().ErrorColor);
                        return;
                        //throw new ApplicationException("Syntax error in expression:'" + expression + "'. String terminator expected");
                    }

                    processed += expression.Substring(0, i + 1);
                    expression = expression.Remove(0, i + 1);
                    continue;
                }

                // Zoek een identifier, zoek de bijbehorende var
                // en zet die op current. Zoek daarna verder
                // naar een identifier en zoek dat ding op in de current.
                // Ga zo door tot bTokenEnded.
                bool bTokenEnded = false;
                XmlNode currenttype = null;
                KnownVariable currentvar = null;
                i = 0;
                while (!bTokenEnded)
                {
                    if (!Char.IsLetter(expression, i))
                    {
                        bTokenEnded = true;
                        break;
                    }
                    i++;

                    // identifier consists of letter + 0/more letter/digit/_
                    while (i < expression.Length && (Char.IsLetterOrDigit(expression, i)
                        || expression[i] == '_'))
                        i++;
                    // Nu hebben we een identifier. 
                    string id = expression.Substring(0, i);
                    // Als we nog aan het begin staan kijken we de actuele variabelen door.
                    // Later kijken we in de huidige variabele
                    if (currentvar == null)
                    {
                        foreach (KnownVariable kv in actualvars)
                        {
                            if (kv.name == id)
                            {
                                currentvar = kv;
                                if (kv.typestring != "String" &&
                                    kv.typestring != "Decimal" &&
                                    kv.typestring != "Boolean")
                                {
                                    currenttype = TemplateCache.Instance().GetTemplateType(kv.typestring);
                                }
                                break;
                            }
                        }
                        if (currentvar == null)
                        {
                            // niet gevonden. Lees tot iets anders dan letters of cijfers of .
                            // en zet dat in errorkleur neer. Ga daarna verder.
                            ColorString(processed, OptionsSettings.Instance().ExpressionColor);
                            while (i < expression.Length && (Char.IsLetterOrDigit(expression[i])
                                || expression[i] == '.' || expression[i] == '_'))
                            {
                                i++;
                            }

                            ColorString(expression.Substring(0, i), OptionsSettings.Instance().ErrorColor);
                            processed = "";
                            if (i < expression.Length)
                                expression = expression.Substring(i);
                            else
                                expression = "";
                            i = 0;
                            bGotoNextPart = true;
                            break;
                        }
                    }
                    else
                    {
                        // we waren al bezig en zoeken nu iets in de currentvar.
                        if (currenttype == null)
                        {
                            // we hadden het over een String ofzo. Verder gaan is dan niet mogelijk

                        }
                        else
                        {
                        }

                    }
                    if (i > expression.Length || expression[i] != '.')
                    {
                        bTokenEnded = true;
                        break;
                    }
                    i++; // Read over the '.' and get next identifier
                }

                if (bGotoNextPart)
                    continue;

                ColorString(processed, OptionsSettings.Instance().ExpressionColor);
                ColorString(expression, OptionsSettings.Instance().ErrorColor);
                return;
            }
            */
            // ColorString(expression, OptionsSettings.Instance().ExpressionColor);
        }
        /*
        private bool CheckValidIdentifier(string id, int linenr)
        {
            int idx = actualvars.Count;
            // Zoek eerst de index van de var die meetelt. 
            // dat zijn dus niet dingen met een hoger regelnummer
            while (idx >= 0 && actualvars[idx].linenr >= linenr)
            {
                idx --;
            }
            // Doorloop de relevante variabelen, 
            // en sla de stukken over die (tijdelijk) een dieper level hebben
            // omdat ze nav een Do-loop oid gedefinieerd zijn
            while (idx > 0 && vars[idx].name != id)
            {
                // als we een leveldown tegenkomen, doorskippen naar de bijbehorende levelup.
                // En de juiste levelcount bijhouden...
                if (vars[idx].leveldown)
                {
                    int level = -1;
                    while (idx >= 0 && level != 0)
                    {
                        if (vars[idx].levelup)
                            level++;
                        if (vars[idx].leveldown)
                            level--;
                        idx--;
                    }
                }
                if (vars[idx].name == id)
                {
                    return true;
                }
            }
        }
         * */

    }
}
