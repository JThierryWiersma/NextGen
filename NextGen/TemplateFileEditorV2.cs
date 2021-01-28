using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using Generator.Utility;
using Generator.Expressions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Generator.VisitorPattern;
using Generator.Statements;
using Generator.CodeProcessors;
using Generator.ObserverPattern;

namespace Generator
{
    public partial class TemplateFileEditorV2 : Form, IObserver, ISubject
    {
        private XmlNode         m_type_definition;
        private XmlNode         m_currenttype;
        private string          m_filename;
        private string          m_templatefilename;
        private bool            m_dirty;
        private bool            m_file_editable     = true;
        private string          m_displayedtype;
        private List<CodeLine>  m_codelines;
        //private const String DoBuildPossibleIdentifiersList = "BuildPossibleIdentifiersList";

        private bool m_libraryeditor;

        private List<KnownVariable> globalvars;// Lijst met alle variabelen in de template
        private List<KnownVariable> actualvars;// lijst met alleen de actuele variabelen 

        private int             m_previousline      = -1;
        private bool            m_linechanged       = false;

        public TemplateFileEditorV2()
        {
            InitializeComponent();
            m_libraryeditor                         = false;
        }
        public TemplateFileEditorV2(string name, string type)
        {
            InitializeComponent();

            m_libraryeditor                         = false;
            m_currenttype                           = TemplateCache.Instance().GetValueFor(type, name).CloneNode(true);
            m_type_definition                       = TemplateCache.Instance().GetTemplateType(type);
            m_filename                              = m_currenttype.Attributes["sourcefile"].Value;
            m_displayedtype                         = type;

            Text                                    = "Templatefile: " + name;
            txtText.Lines                           = new String[0];

            XmlNode             templatefilenamenode= m_currenttype.SelectSingleNode("templatefilename");
            if (templatefilenamenode != null)
            {
                string          f                   = templatefilenamenode.InnerText;
                m_templatefilename                  = Path.Combine(Path.GetDirectoryName(m_filename), f);
            }
            FinishLoading();
        }

        /// <summary>
        /// Constructor voor library files. Zonder type.
        /// </summary>
        /// <param name="name"></param>
        public TemplateFileEditorV2(string name)
        {
            InitializeComponent();
            m_libraryeditor                         = true;
            m_currenttype                           = null;
            m_type_definition                       = null;
            m_filename                              = name;
            m_displayedtype                         = "Library";

            Text                                    = "Library: " + name;
            txtText.Lines                           = new String[0];
            m_templatefilename                      = TemplateCache.Instance().SolutionLocation + "\\TemplateFile\\" + name;

            FinishLoading();
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

        public void FinishLoading()
        {
            TemplateMain        m                   = TemplateMain.Instance();
            m.Cursor                                = Cursors.WaitCursor;
            m.StatusLabel.Text                      = "Loading";
            m.StatusLabel.Visible                   = false; // wordt toch niet getoond...(?)
            m.StatusLabel.Invalidate();
            m.ProgressBar.Value                     = 0;
            m.ProgressBar.Maximum                   = 100;
            m.ProgressBar.Visible                   = true;

            try
            {
                Parser          p                   = new Parser();
                m_codelines                         = p.Load(m_templatefilename);
                CodePrinter     cp                  = new CodePrinter(txtText);
                //BuildPossibleIdentifiersList(File.ReadAllLines(m_templatefilename));
                foreach (CodeLine c in m_codelines)
                {
                    m.ProgressBar.Value             = (c.Linenr * 100 / m_codelines.Count);
                    c.Accept(cp);
                    cp.NextLine();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                m.ProgressBar.Visible               = false;
                m.StatusLabel.Visible               = false;
                m.Cursor                            = Cursors.Default;
            }

            /* 
            foreach (KnownVariable k in globalvars)
            {
                lstAlleMogelijkheden.Items.Add(k);
            } 
             */
        }

        public void Gotoline(int linenr)
        {
            if (linenr > 0)
                linenr--;
            int charpos = txtText.GetFirstCharIndexFromLine(linenr);
            txtText.Select(charpos, 0);
            txtText.ScrollToCaret();
            txtText.Focus();
        }
        public string GetDisplayedTypeName()
        {
            return m_displayedtype;
        }
        public bool dirty
        {
            get
            {
                return m_dirty;
            }
            set
            {
                if (value)
                {
                    if (!m_dirty)
                    {
                        btnApply.Enabled = m_file_editable;

                        if (!m_file_editable)
                            MessageBox.Show("Changes will not be saved! Reload the file from a writable version to be able to save your changes!", "Read-only warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    btnApply.Enabled = false;
                }
                m_dirty = value;
            }
        }

        public void Mark_Dirty(object sender, System.EventArgs e)
        {
            dirty = true;
        }
        public Boolean IsDisplaying(string name)
        {
            if (Path.GetFileName(m_filename) == name)
                return true;

            if (Path.GetFileName(GetDisplayedName()) == name)
                return true;

            if (Path.GetFileName(m_templatefilename) == name)
                return true;

            return false;
        }
        public string GetDisplayedName()
        {
            string              result;
            if (m_libraryeditor)
            {
                result                              = m_filename;
            }
            else
            {
                string          nameatt             = m_type_definition.Attributes["nameattribute"].Value;
                result                              = m_currenttype.SelectSingleNode(nameatt).InnerText.Trim();
            }
            return result;
        }

        public Boolean IsLibraryEditor
        {
            get
            {
                return m_libraryeditor;
            }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            this.SaveTemplate();
            this.Close();
            /*
            globalvars = new List<KnownVariable>();
            actualvars = new List<KnownVariable>();
            txtText.Text = "";
            FinishLoading();
             * */
        }


        private void btnCheckSyntax_Click(object sender, EventArgs e)
        {
            TemplateMain.Instance().ShowOutputText();
            CodeFormatter       f                   = new CodeFormatter(txtText);
            f.ProcessAll(m_codelines);
            SyntaxChecker       sc                  = new SyntaxChecker(m_libraryeditor);
            sc.AddObserver(this);
            sc.ProcessAll(m_codelines);
        }

        /// <summary>
        /// Voeg de appliesto class to als een variabele in de lijst.
        /// </summary>
        private void AddAppliesToVar()
        {
            XmlNode             appliestonode       = m_currenttype.SelectSingleNode("appliesto");
            Debug.Assert(appliestonode != null, "Appliestonode kon niet gevonden worden");
            String              appliesto           = appliestonode.InnerText;
            XmlNode             t                   = TemplateCache.Instance().GetValueFor("TypeDefs", appliesto);
            AddConceptPartsToVar(t, -1, appliesto);
        }
        /// <summary>
        /// Neem het type zoals beschreven, en haal de elementen daaruit
        /// en definieer die als variabeltjes op de beschreven regel.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="line"></param>
        /// <param name="typename"></param>
        private void AddConceptPartsToVar(XmlNode type, int line, string typename)
        {
            KnownVariable       kv                  = null;
            foreach (XmlNode el in type.SelectNodes("attributes/attribute[BAS='']"))
            {
                try
                {
                    string      varname             = el.SelectSingleNode("name").InnerText;
                    string      vartype;
                    XmlNode     tnode               = el.SelectSingleNode("type");
                    if (tnode != null && tnode.InnerText != "")
                    {
                        switch (tnode.InnerText)
                        {
                            case "Name":
                            case "Text":
                            case "Combobox":
                                vartype             = "String";
                                break;
                            case "Number":
                            case "Order":
                                vartype             = "Decimal";
                                break;
                            case "Checkbox":
                                vartype             = "Boolean";
                                break;
                            default:
                                vartype             = "??? " + tnode.InnerText;
                                break;
                        }
                        kv = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                        globalvars.Add(kv);
                        actualvars.Add(kv);

                        continue;
                    }
                    // Probeer of het een 'concept' variabele is.
                    tnode = el.SelectSingleNode("concept");
                    if (tnode != null && tnode.InnerText != "")
                    {
                        vartype                     = tnode.InnerText;
                        kv                          = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                        globalvars.Add(kv);
                        actualvars.Add(kv);

                        continue;
                    }
                    // Probeer of het een 'userconcept' variabele is.
                    tnode = el.SelectSingleNode("userconcept");
                    if (tnode != null && tnode.InnerText != "")
                    {
                        vartype = tnode.InnerText;
                        kv = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                        globalvars.Add(kv);
                        actualvars.Add(kv);

                        continue;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("exception raised in AddConceptPartsToVar: " + e.Message);
                }
            }
        }
        private void AddGlobalVariables()
        {
            globalvars.Add(new KnownVariable("at", "String", -2, VariableSource.Global));
            globalvars.Add(new KnownVariable("tab", "String", -2, VariableSource.Global));
            globalvars.Add(new KnownVariable("version", "String", -2, VariableSource.Global));
            globalvars.Add(new KnownVariable("now", "String", -2, VariableSource.Global));
            globalvars.Add(new KnownVariable("today", "String", -2, VariableSource.Global));
            globalvars.Add(new KnownVariable("Guid", "String", -2, VariableSource.Global));
        }
        /*
        /// <summary>
        /// Draait in de backgroundworker.
        /// </summary>
        private void BuildPossibleIdentifiersList(string[] lines)
        {
            OptionsSettings options = OptionsSettings.Instance();

            int linenr = -1;
            globalvars = new List<KnownVariable>();
            actualvars = new List<KnownVariable>();

            AddGlobalVariables();
            // Kopieer alle globalen over naar actuals.
            actualvars.AddRange(globalvars);

            // Voeg alle concept onderdelen van het concept waarop de template
            // draait toe als variabelen.
            if (!m_libraryeditor)
                AddAppliesToVar();

            //string[] lines = GetTxtLines();
            foreach (string line in lines)
            {
                linenr++;
                TemplateMain.Instance().ProgressBar.Value = (linenr * 100 / lines.Length);
                // Bekijk de regel. 
                // Mogelijkheden zijn:
                // - de onderdelen van de appliestoconcept 
                // - een loop over een bepaald type ding.
                //     levert: alle attributen van het ding
                // - een loop over de attributen
                //     levert: alle onderdelen van het attribuut
                // - een loop over de sets
                //     levert: alle onderdelen van de sets
                // - loop over de setattributen (?)
                //     levert de onderdelen daar weer van.
                // - een variabele declaratie van een bepaald type
                // 
                //string sLineTrim = line.TrimStart();
                //if (!sLineTrim.StartsWith("@"))
                //{
                //    ProcessCode(line, linenr);
                //    continue;
                //}
                String sLineTrim = line.Trim();
                Match m = Regex.Match(sLineTrim, @"^@\s*(?<command>\w+)\b");
                String command = "";
                if (m.Length > 0)
                {
                    command = m.Groups["command"].Value;
                }

                if (command == "Else" ||
                    command == "EndIf")
                {
                    if (m.Length == sLineTrim.Length)
                    {
                        ColorLine(line, options.CommandColor);
                    }
                    else
                    {
                        // eerste stuk ok, rest in error kleur
                        int p1 = line.IndexOf(command) + command.Length;
                        ColorString(line.Substring(0, p1), options.CommandColor);
                        ColorLine(line.Substring(p1), options.ErrorColor);
                    }
                    continue;
                }
                if (command == "Loop")
                {
                    // Drop alle actuele zaken voor het concept uit de loop.
                    // Laat gewone variabelen staan. Die blijven bruikbaar
                    int idx = actualvars.Count - 1;
                    while (idx >= 0 && actualvars[idx].Level != 1)
                    {
                        if (actualvars[idx].variablesource == VariableSource.Concept)
                        {
                            actualvars.RemoveAt(idx);
                        }
                        idx--;
                    }
                    // En we gaan een leven down. Noteer het in de globals
                    globalvars.Add(KnownVariable.LevelDown);

                    if (m.Length == sLineTrim.Length)
                    {
                        ColorLine(line, options.CommandColor);
                    }
                    else
                    {
                        // eerste stuk ok, rest in error kleur
                        int p1 = line.IndexOf(command) + command.Length;
                        ColorString(line.Substring(0, p1), options.CommandColor);
                        ColorLine(line.Substring(p1), options.ErrorColor);
                    }
                    continue;
                }

                if (command == "Do")
                {
                    ProcessDo(line, linenr);
                    // kan een concept achter staan, of ?
                    continue;
                }
                if (command == "While")
                {
                    ProcessWhile(line, linenr);
                    // kan een concept achter staan, of ?
                    continue;
                }
                if (command == "Error")
                {
                    ProcessError(line, linenr);
                    continue;
                }
                if (command == "Info")
                {
                    ProcessInfo(line, linenr);
                    continue;
                }
                if (command == "Return")
                {
                    ProcessReturn(line, linenr);
                    continue;
                }
                if (command == "Exit" || command == "EndFunction")
                {
                    if (m.Length == sLineTrim.Length)
                    {
                        ColorLine(line, options.CommandColor);
                    }
                    else
                    {
                        // eerste stuk ok, rest in error kleur
                        int p1 = line.IndexOf(command) + command.Length;
                        ColorString(line.Substring(0, p1), options.CommandColor);
                        ColorLine(line.Substring(p1), options.ErrorColor);
                    }
                    continue;
                }
                if (command == "If")
                {
                    ProcessIf(line, linenr);
                    continue;
                }
                if (command == "ElseIf")
                {
                    ProcessElseIf(line, linenr);
                    continue;
                }
                // Check of het een variabele declaratie is.
                if (command == "Var" || command == "Global")
                {
                    // Bepaal de naam.
                    //ColorLine(line, options.VariableDeclarationColor);
                    ProcessVarDeclaration(line, linenr, command);
                    continue;
                }
                // Check of het een variabele declaratie is.
                if (command == "Function")
                {
                    // Bepaal de naam.
                    //ColorLine(line, options.VariableDeclarationColor);
                    ProcessFunctionDeclaration(line, linenr);
                    continue;
                }
                if (Regex.IsMatch(sLineTrim, @"^\s*@\s*\-\-"))
                {
                    int p1 = line.IndexOf("@") + 1;
                    ColorString(line.Substring(0, p1), options.CommandColor);
                    ColorLine(line.Substring(p1), options.CommentColor);
                    continue;
                }
                if (Regex.IsMatch(sLineTrim, @"^\s*@\s*(?<var>\w+\s*)(?<assign>[+-]?=)"))
                {
                    ProcessAssignment(line, linenr);
                    continue;
                }
                if (Regex.IsMatch(sLineTrim, @"^\s*@\s*(?<functionname>\w+\s*)[(]"))
                {
                    ProcessVoidFunctionCall(line, linenr);
                    continue;
                }
                ProcessCode(line, linenr);
            }
            txtText.Select(0, 0);
            txtText.ScrollToCaret();
        }
        */
        /*
        /// <summary>
        /// Een Do verwacht een Conceptclass (Evt + een expressie)
        /// of een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessDo(string line, int linenr)
        {
            int startpos = line.IndexOf("Do") + 2;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);

            string typestring;
            string rest = line.Substring(startpos);
            Match m = Regex.Match(rest, @"^\s*(?<type>[-\w\.]+)\b");
            if (!m.Success)
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }
            // We voegen een nieuw level toe aan de globals/actuals
            globalvars.Add(KnownVariable.LevelUp);
            actualvars.Add(KnownVariable.LevelUp);

            typestring = m.Groups["type"].Value;
            if (typestring == "Decimal" ||
                typestring == "String" ||
                typestring == "Boolean")
            {
                ColorString(m.Value, OptionsSettings.Instance().ClassColor);
                ColorExpression(rest.Substring(m.Length), linenr);
            }
            else if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), typestring) >= 0)
            {
                ColorString(m.Value, OptionsSettings.Instance().ClassColor);
                XmlNode type = TemplateCache.Instance().GetValueFor("TypeDefs", typestring);
                AddConceptPartsToVar(type, linenr, typestring);
                ColorExpression(rest.Substring(m.Length), linenr);
            }
            //else if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("concept"), typestring) >= 0)
            //{
            //    ColorString(m.Value, OptionsSettings.Instance().ClassColor);
            //    XmlNode type = TemplateCache.Instance().GetValueFor("concept", typestring);
            //    AddConceptPartsToVar(type, linenr, typestring);
            //    ColorExpression(rest.Substring(m.Length), linenr);
            //}
            else
            {
                ColorExpression(m.Value, linenr);
            }
            NextLine();
        }
         * */
        

        private void DisplayCurrentLineNumber(int line)
        {
            //this.CurrentLineNumber.Text = String.Format("Line: {0}", line);
        }
        private void txtText_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            DisplayCurrentLineNumber(txtText.GetLineFromCharIndex(txtText.GetCharIndexFromPosition(mea.Location)));
        }

        private void txtText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyData == Keys.Space)
            {
                //  MessageBox.Show("AHA EEN SPATIE MET CONTROL");
            }
        }



        private void txtText_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Space)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MessageBox.Show("AHA EEN SPATIE MET CONTROL");

            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                txtText.SelectedText = "\n";
            }
        }

        private void txtText_KeyUp(object sender, KeyEventArgs e)
        {
            DisplayCurrentLineNumber(txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine()));
        }
        /*
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBar.Value = e.ProgressPercentage;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (KnownVariable k in vars)
            {
                lstAlleMogelijkheden.Items.Add(k);
            }
            this.StatusLabel.Visible = false;
            this.ProgressBar.Visible = false;
            txtText.ResumeLayout();
        }
        
        delegate string[] GetTxtLinesCallback();
        private string[] GetTxtLines()
        {
            if (txtText.InvokeRequired)
            {
                GetTxtLinesCallback c = new GetTxtLinesCallback(GetTxtLines);
                return this.Invoke(c) as string[];
            }
            else
            {
                return txtText.Lines;
            }
        }*/

        private void TemplateFileEditor_Shown(object sender, EventArgs e)
        {
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.SaveTemplate();
        }
        private void SaveTemplate()
        {
            File.WriteAllLines(m_templatefilename, txtText.Lines);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public string EditorFilename()
        {
            return Path.GetFileName(m_templatefilename);
        }
        private void txtText_TextChanged(object sender, EventArgs e)
        {
            //Notify("TextChanged, currentline =  " + txtText_GetCurrentLine().ToString(), NotificationType.Info, null);
            m_linechanged = true;
        }
        private void CheckLineChange()
        {
            CheckLineChange(false);
        }
        private void UpdateLines(int linenr, int length)
        {
            Notify(String.Format("Updatelines: {0} + {1}", linenr, length), NotificationType.Info, null);

            // Bepaal de plek waar we weer terug willen komen
            int selstart = txtText.SelectionStart;
            int sellen   = txtText.SelectionLength;
            CodePrinter cp = new CodePrinter(txtText);
            this.Cursor = Cursors.WaitCursor;
            txtText.SuspendLayout();
            while (length > 0)
            {
                // Controleer de regels op syntax 
                int p1 = txtText.GetFirstCharIndexFromLine(linenr);
                int p2 = txtText.GetFirstCharIndexFromLine(linenr + 1);

                string line = txtText.Lines[linenr];
                m_codelines[linenr] = CodeLine.BuildCodeLine(m_filename, linenr + 1, line);
                //... do vanalles
                //txtText.SelectionStart = p1;
                //txtText.SelectionLength = p2 - p1;
                txtText.Select(p1, p2 - p1 - 1);
                txtText.SelectedText = "";
                m_codelines[linenr].Accept(cp);
                //...
                length--;
                linenr++;
            }
            // zet de selection weer terug
            txtText.SelectionStart = selstart;
            txtText.SelectionLength = sellen;
            txtText.ResumeLayout();
            this.Cursor = Cursors.Default;
        }
        private void CheckLineChange(bool alwaysupdate)
        {
            int                 currentline         = txtText_GetCurrentLine();
            if (m_previousline != currentline || alwaysupdate)
            {
                // We hebben een line change
                if (m_previousline >= 0 && m_linechanged)
                {
                    UpdateLines(m_previousline, 1);
                }
                // Zet vorige regel op huidige regel.
                m_previousline                      = currentline;
                m_linechanged                       = false;
            }
        }
        private int txtText_GetCurrentLine()
        {
            return txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine());
        }
        private void txtText_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            //Notify("Keydown KeyChar = " + e.KeyChar.ToString(), NotificationType.Info, null);
        }

        private void txtText_KeyUp_1(object sender, KeyEventArgs e)
        {
            // Paste
            if (e.Control && e.KeyCode == Keys.V)
            {
                Notify("keyup Paste!", NotificationType.Info, null);
            }
            // Cut
            else if (e.Control && e.KeyCode == Keys.X)
            {
                Notify("keyup Cut!", NotificationType.Info, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                Notify("keyup Delete!", NotificationType.Info, null);
            }
            else if (e.KeyCode == Keys.Back)
            {
                Notify("keyup Back!", NotificationType.Info, null);
            }
            else
            {
                CheckLineChange();
                return;
            }
            int huidigeregel = txtText_GetCurrentLine();
            int aantalregels = txtText.Lines.Length;
            if (huidigeregel != m_previousline)
            {
                // moet haast een back toets zijn geweest, maar dat maakt niet uit.
                // Verdere regels hernummeren
                if (huidigeregel > m_previousline)
                    UpdateLines(m_previousline, huidigeregel - m_previousline + 1);
                else
                    UpdateLines(huidigeregel, m_previousline - huidigeregel + 1);
            }
            else
            {

                UpdateLines(huidigeregel, m_selectionaantal + aantalregels - m_aantalregels);

            }
            m_previousline = huidigeregel;
            m_linechanged = false;
        }

        private void txtText_KeyDown_1(object sender, KeyEventArgs e)
        {
            
            //Notify("Keydown KeyCode = " + e.KeyCode.ToString(), NotificationType.Info, null);
        }

        private void txtText_MouseClick(object sender, MouseEventArgs e)
        {
            //Notify("MouseClick line = " + txtText_GetCurrentLine().ToString(), NotificationType.Info, null);
            CheckLineChange();
        }

        private void txtText_MouseUp(object sender, MouseEventArgs e)
        {
            //Notify("Mouseup line = " + txtText_GetCurrentLine().ToString(), NotificationType.Info, null);
            CheckLineChange();

        }

        private void txtText_Leave(object sender, EventArgs e)
        {
            CheckLineChange(true);
        }
        /*
        private void VerwerkPaste()
        {
            if (txtText.SelectionLength > 0)
            {
                // Werking gelijk aan delete toets: het geselecteerde verwijderen
                // VerwijderSelectieBijwerken();
            }
            // Paste buffer tussenvoegen, dus die moeten opnieuw gevalideerd
            // en de rest schuift weer op.
            // Tel het aantal regels voor en na de paste en bereken het verschil.
            _selectionaantal = txtText.SelectedText.Split('\n').Length;
            _huidigeregel = txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine());
            _aantalregels = txtText.Lines.Length;
        }
        private void VerwerkDelete()
        {
            if (txtText.SelectionLength > 0)
            {
                // Werking gelijk aan delete toets: het geselecteerde verwijderen
                //VerwijderSelectieBijwerken();
            }
            else
            {
                // Voorliggende karakter verwijderen, alleen als het een \n teken is
                // moeten we wat speciaals doen, nml de vervolgregels hernummeren.
            }
            _selectionaantal = txtText.SelectedText.Split('\n').Length;
            _huidigeregel = txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine());
            _aantalregels = txtText.Lines.Length;
        }
        private void VerwerkCut()
        {
            // Voor het aantal newlines in de selectie 
            //VerwijderSelectieBijwerken();
            _selectionaantal = txtText.SelectedText.Split('\n').Length;
            _huidigeregel = txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine());
            _aantalregels = txtText.Lines.Length;
        }
        private void VerwerkBack()
        {
            if (txtText.SelectionLength > 0)
            {
                // Werking gelijk aan delete toets: het geselecteerde verwijderen
                //VerwijderSelectieBijwerken();
            }
            else
            {
                // Achterliggende karakter verwijderen, alleen als het een \n teken is
                // moeten we wat speciaals doen, nml de vervolgregels hernummeren.
            }
            _selectionaantal = txtText.SelectedText.Split('\n').Length;
            _huidigeregel = txtText.GetLineFromCharIndex(txtText.GetFirstCharIndexOfCurrentLine());
            _aantalregels = txtText.Lines.Length;
        } */
        private void txtText_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            // Paste
            if (e.Control && e.KeyCode == Keys.V)
            {
                Notify("preview Paste!", NotificationType.Info, null);
                /* if (System.Windows.Forms.Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string cb = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Text);
                    int n = cb.Split('\n').Length;
                } */
            }
            // Cut
            else if (e.Control && e.KeyCode == Keys.X)
            {
                Notify("preview Cut!", NotificationType.Info, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                Notify("preview Delete!", NotificationType.Info, null);
            }
            else if (e.KeyCode == Keys.Back)
            {
                Notify("preview Back!", NotificationType.Info, null);
            }
            else
            {
                return;
            }
            m_selectionaantal = txtText.SelectedText.Split('\n').Length;
            m_aantalregels = txtText.Lines.Length;

            Notify(String.Format("Sel#: {0}, Cur#:{1} CurCount#:{2}", m_selectionaantal, m_previousline, m_aantalregels), NotificationType.Info, null);
        }
        int m_selectionaantal = 0;
        int m_aantalregels = 0;
        /*
        private Color ClassColor { get { return OptionsSettings.Instance().ClassColor; } }
        private Color CodeColor { get { return OptionsSettings.Instance().CodeColor; } }
        private Color CommandColor { get { return OptionsSettings.Instance().CommandColor; } }
        private Color CommentColor { get { return OptionsSettings.Instance().CommentColor; } }
        private Color ErrorColor { get { return OptionsSettings.Instance().ErrorColor; } }
        private Color ExpressionColor { get { return OptionsSettings.Instance().ExpressionColor; } }
        */
    }
    public enum VariableSource
    {
        Global,
        Concept,
        Variable
    }
    public class KnownVariable
    {
        public string name;
        public string typestring;
        public int linenr;
        public VariableSource variablesource;
        public string conceptname;
        public static KnownVariable LevelUp = new KnownVariable(1);
        public static KnownVariable LevelDown = new KnownVariable(-1);
        public int Level = 0;
        private KnownVariable(int l)
        {
            Level = l;
        }
        public KnownVariable(string n, string t, int l, VariableSource vs, string cn)
        {
            name = n;
            typestring = t;
            linenr = l;
            variablesource = vs;
            conceptname = cn;

            //Debug.Assert(cn == "" || vs != VariableSource.Concept);
        }
        public KnownVariable(string n, string t, int l, VariableSource vs)
            : this(n, t, l, vs, "")
        { }
        public override string ToString()
        {
            if (variablesource == VariableSource.Concept)
            {
                return String.Format("{0} {1}: {2} [{3}]", linenr, name, typestring, conceptname);
            }
            else
            {
                return String.Format("{0} {1}: {2}", linenr, name, typestring);
            }
        }

    }
}