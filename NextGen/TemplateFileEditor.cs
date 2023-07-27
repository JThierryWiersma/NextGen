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

namespace Generator
{
    public partial class TemplateFileEditor : Form
    {
        private XmlNode             m_type_definition;
        private XmlNode             m_currenttype;
        private string              m_filename;
        private string              m_templatefilename;
        private bool                m_dirty;
        private bool                m_file_editable = true;
        private string              m_displayedtype;
        //private const String DoBuildPossibleIdentifiersList = "BuildPossibleIdentifiersList";

        private bool                m_libraryeditor; 

        private List<KnownVariable> globalvars;// Lijst met alle variabelen in de template
        private List<KnownVariable> actualvars;// lijst met alleen de actuele variabelen 
        
        public TemplateFileEditor()
        {
            InitializeComponent();
            m_libraryeditor                         = false;
        }
        public TemplateFileEditor(string name, string type)
        {
            InitializeComponent();
            //RepositionControls();

            m_libraryeditor                         = false;
            m_currenttype                           = TemplateCache.Instance().GetValueFor(type, name).CloneNode(true);
            m_type_definition                       = TemplateCache.Instance().GetTemplateType(type);
            m_filename                              = m_currenttype.Attributes["sourcefile"].Value;
            m_displayedtype                         = type;

            Text                                    = "Template: " + name;
            txtText.Lines                           = new String[0];

            XmlNode     templatefilenamenode        = m_currenttype.SelectSingleNode("templatefilename");
            if (templatefilenamenode != null)
            {
                string              f               = templatefilenamenode.InnerText;
                m_templatefilename                  = Path.Combine(Path.GetDirectoryName(m_filename), f);
            }
            FinishLoading();
        }

        /// <summary>
        /// Constructor voor library files. Zonder type.
        /// </summary>
        /// <param name="name"></param>
        public TemplateFileEditor(string name)
        {
            InitializeComponent();
            //RepositionControls();
            m_libraryeditor                         = true;

            m_currenttype                           = null;
            m_type_definition                       = null;
            m_filename                              = name;
            m_displayedtype                         = "Library";

            Text                                    = "Library: " + name;
            txtText.Lines                           = new String[0];
            m_templatefilename                      = TemplateCache.Instance().SolutionLocation + "\\Template\\" + name;
           
            FinishLoading();
        }
        
        public void FinishLoading()
        {
            TemplateMain.Instance().Cursor              = Cursors.WaitCursor;
            TemplateMain.Instance().StatusLabel.Text    = "Loading";
            TemplateMain.Instance().StatusLabel.Visible = false; // wordt toch niet getoond...(?)
            TemplateMain.Instance().StatusLabel.Invalidate();
            TemplateMain.Instance().ProgressBar.Value   = 0;
            TemplateMain.Instance().ProgressBar.Maximum = 100;
            TemplateMain.Instance().ProgressBar.Visible = true;
            
            /* if (File.Exists(m_templatefilename))
            {
                txtText.Lines = File.ReadAllLines(m_templatefilename);
            }
            */

            try
            {
                BuildPossibleIdentifiersList(File.ReadAllLines(m_templatefilename));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TemplateMain.Instance().ProgressBar.Visible = false;
                TemplateMain.Instance().StatusLabel.Visible = false;
                TemplateMain.Instance().Cursor = Cursors.Default;
            }

            foreach (KnownVariable k in globalvars)
            {
                lstAlleMogelijkheden.Items.Add(k);
            }
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

        public string GetDisplayedName()
        {
            string result; 
            if (m_libraryeditor)
            {
                result                              = m_filename;
            }
            else
            {
                string              nameatt         = m_type_definition.Attributes["nameattribute"].Value;
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


        private void butCheckSyntax_Click(object sender, EventArgs e)
        {
            FormatLines();
        }

        private void FormatLines()
        {
            int templateTabs = OptionsSettings.Instance().Templatetabs;
            int level = 0;
            int level_too_low_warningline = -1;

            // Alles wat een commmando is
            // - alles voor de @ weghalen
            // - achter de @ zoveel spaties * tabsize als we level hebben
            //      invoegen of juist weghalen.

            int linenr = -1;
            Regex rCommand = new Regex(@"^(?<pre_at_space>\s*)@(?<post_at_space>\s*)(?<command>\w+)\b", RegexOptions.Compiled);
            Regex rComment = new Regex(@"^(?<pre_at_space>\s*)@(?<post_at_space>\s*)(?<command>\-\-)", RegexOptions.Compiled);
            Regex rAssign = new Regex(@"^(?<pre_at_space>\s*)@(?<post_at_space>\s*)(?<command>\w+\s*[+-]?=)", RegexOptions.Compiled);
            foreach (string line in txtText.Lines)
            {
                linenr++;
                Match m = rCommand.Match(line);
                String command = "";
                // Heeft het de vorm van een commando?
                if (m.Success)
                {
                    command = m.Groups["command"].Value;
                }
                else
                {   // Lijkt het op commentaar dan?
                    m = rComment.Match(line);
                    if (m.Success)
                    {
                        command = "--";
                    }
                    else
                    {
                        // Of is het een assignment achtig iets
                        m = rAssign.Match(line);
                        if (!m.Success)
                            continue;
                        // Het is een assignment
                        FormatLine(line, linenr, level, m);
                        continue;
                    }
                }
                if (command == "Do" || 
                    command == "While" ||
                    command == "If" ||
                    command == "Function")
                {
                    FormatLine(line, linenr, level, m);
                    level += templateTabs;
                }
                else if (command == "Else" ||
                    command == "ElseIf")
                {
                    FormatLine(line, linenr, level - templateTabs, m);
                }
                else if (command == "EndIf" ||
                    command == "EndFunction" ||
                    command == "Loop")
                {
                    if (level == 0)
                    {
                        if (level_too_low_warningline < 0)
                        {
                            level_too_low_warningline = linenr;
                        }
                    }
                    else
                    {
                        level -= templateTabs;
                    }
                    FormatLine(line, linenr, level, m);
                }
                else if (command == "Var" ||
                    command == "Global" || 
                    command == "--" ||
                    command == "Error" ||
                    command == "Info" || 
                    command == "Return" ||
                    command == "Exit")
                {
                    FormatLine(line, linenr, level, m);
                }
                else
                {
                    // Of is het een assignment achtig iets
                    m = rAssign.Match(line);
                    if (m.Success)
                        FormatLine(line, linenr, level, m);
                }
            }
            if (level_too_low_warningline > 0)
            {
                MessageBox.Show("Level too low on line " + level_too_low_warningline.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        /// <param name="levelspaces">hoeveel spaties er achter de @ moeten</param>
        /// <param name="m"></param>
        private void FormatLine(string line, int linenr, int levelspaces, Match m)
        {
            int pre_at = m.Groups["pre_at_space"].Length;
            if (pre_at > 0)
            {
                // Spaties aan begin van de regel verwijderen
                txtText.Select(txtText.GetFirstCharIndexFromLine(linenr), pre_at);
                txtText.SelectedText = "";
            }
            int post_at = m.Groups["post_at_space"].Length;
            if (post_at > levelspaces)
            {
                // haal er een paar weg
                txtText.Select(txtText.GetFirstCharIndexFromLine(linenr) + 1, post_at - levelspaces);
                txtText.SelectedText = "";
            }
            else if (post_at < levelspaces)
            {
                // voeg er een paar toe
                txtText.Select(txtText.GetFirstCharIndexFromLine(linenr) + 1, 0);
                txtText.SelectedText = new string(' ', levelspaces - post_at);
            }
        }
        /// <summary>
        /// Voeg de appliesto class to als een variabele in de lijst.
        /// </summary>
        private void AddAppliesToVar()
        {
            XmlNode appliestonode = m_currenttype.SelectSingleNode("appliesto");
            Debug.Assert(appliestonode != null, "Appliestonode kon niet gevonden worden");
            String appliesto = appliestonode.InnerText;
            XmlNode t = TemplateCache.Instance().GetValueFor("TypeDefs", appliesto);
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
            KnownVariable kv = null;
            foreach (XmlNode el in type.SelectNodes("attributes/attribute[BAS='']"))
            {
                try
                {
                    string varname = el.SelectSingleNode("name").InnerText;
                    string vartype;
                    XmlNode tnode = el.SelectSingleNode("type");
                    if (tnode != null && tnode.InnerText != "")
                    {
                        switch (tnode.InnerText)
                        {
                            case "Name":
                            case "Text":
                            case "Combobox":
                                vartype = "String";
                                break;
                            case "Number":
                            case "Order":
                                vartype = "Decimal";
                                break;
                            case "Checkbox":
                                vartype = "Boolean";
                                break;
                            default:
                                vartype = "??? " + tnode.InnerText;
                                break;
                        }
                        kv = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                        globalvars.Add(kv);
                        actualvars.Add(kv);

                        continue;
                    }
                    // Probeer of het een 'concept' variabele is.
                    tnode = el.SelectSingleNode("Concept");
                    if (tnode != null && tnode.InnerText != "")
                    {
                        vartype = tnode.InnerText;
                        kv = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                        globalvars.Add(kv);
                        actualvars.Add(kv);

                        continue;
                    }
                    //// Probeer of het een 'userconcept' variabele is.
                    //tnode = el.SelectSingleNode("userconcept");
                    //if (tnode != null && tnode.InnerText != "")
                    //{
                    //    vartype = tnode.InnerText;
                    //    kv = new KnownVariable(varname, vartype, line, VariableSource.Concept, typename);
                    //    globalvars.Add(kv);
                    //    actualvars.Add(kv);

                    //    continue;
                    //}
                }
                catch(Exception e)
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
                if (command == "Exit" || command == "EndFunction" )
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
            txtText.SelectionStart = txtText.TextLength;
            txtText.SelectionColor = c;
            txtText.SelectedText = part;
        }
        /// <summary>
        /// Doe wat nodig is als het geen speciaal commando, maar code is.
        /// Alle expressies kleuren, de rest in de code-kleur.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessCode(string line, int linenr)
        {

            while (line != "")
            {
                int firstatpos = line.IndexOf('@');
                if (firstatpos < 0)
                {
                    ColorLine(line, OptionsSettings.Instance().CodeColor);
                    return;
                }
                if (firstatpos > 0)
                    ColorString(line.Substring(0, firstatpos), OptionsSettings.Instance().CodeColor);
                ColorString("@", OptionsSettings.Instance().CommandColor);
                int secondatpos = line.IndexOf('@', firstatpos + 1);
                if (secondatpos < 0)
                {
                    ColorLine(line.Substring(firstatpos + 1), OptionsSettings.Instance().ErrorColor);
                    return;
                }
                ColorExpression(line.Substring(firstatpos + 1, secondatpos - firstatpos - 1), linenr);
                ColorString("@", OptionsSettings.Instance().CommandColor);
                line = line.Substring(secondatpos + 1);
            }
            NextLine();
        }
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
        /// <summary>
        /// Een Function verwacht een Type Naam ([type naam]*)
        /// of een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessFunctionDeclaration(string line, int linenr)
        {
            int startpos = line.IndexOf("Function") + 8;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);

            string typestring;
            string rest = line.Substring(startpos);
            Match m = Regex.Match(rest, @"^(?<typecompleet>\s*(?<type>[-\w\.]+)\s+)(?<naam>[\w]+)\s*\(");
            if (!m.Success)
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }
            typestring = m.Groups["type"].Value;
            if (typestring == "Decimal" ||
                typestring == "String" ||
                typestring == "Void" ||
                typestring == "Boolean")
            {
            }
            else if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), typestring) >= 0)
            {
            }
            else
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }
            ColorString(m.Groups["typecompleet"].Value, OptionsSettings.Instance().ClassColor);
            ColorString(m.Groups["naam"].Value, OptionsSettings.Instance().ExpressionColor);
            int l = m.Groups["typecompleet"].Length + m.Groups["naam"].Length;
            ColorString(m.Value.Substring(l, m.Length - l), OptionsSettings.Instance().CommandColor);
            rest = rest.Substring(m.Length);

            m = Regex.Match(rest, @"^\s*(?<parameters>[^\)]*)\)\s*$");
            if (!m.Success)
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }

            string parameterlist = m.Groups["parameters"].Value;
            bool eerstekeer = true;
            if (parameterlist.Trim() == String.Empty)
            {
                // Kleur maakt niet uit, het zijn toch spaties
                ColorString(parameterlist, OptionsSettings.Instance().CommandColor);
            }
            else
            {
                foreach (string param in parameterlist.Split(','))
                {
                    if (!eerstekeer)
                        ColorString(",", OptionsSettings.Instance().CommandColor);
                    eerstekeer = false;

                    m = Regex.Match(param, @"^(?<typecompleet>\s*(?<type>\w+)\s+)(?<name>\w+)\s*$");
                    if (!m.Success)
                    {
                        ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                        return;
                    }
                    string ptype = m.Groups["type"].Value;
                    string pname = m.Groups["name"].Value;
                    if (ptype == "Decimal" ||
                        ptype == "String" ||
                        ptype == "Boolean" ||
                        Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), ptype) >= 0)
                    {
                    }
                    else
                    {
                        ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                        return;
                    }
                    ColorString(m.Groups["typecompleet"].Value, OptionsSettings.Instance().ClassColor);
                    ColorString(m.Groups["name"].Value, OptionsSettings.Instance().ExpressionColor);
                }
            }
            ColorLine(")", OptionsSettings.Instance().CommandColor);

        }
        /// <summary>
        /// Een Assignment verwacht een Variabele [+*/]= Expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessAssignment(string line, int linenr)
        {
            Match m = Regex.Match(line, @"^(?<start>\s*@\s*)(?<var>\w+\s*)(?<assign>[+-]?=)");
            if (!m.Success)
            {
                ColorLine(line, OptionsSettings.Instance().ErrorColor);
                return;
            }

            ColorString(m.Groups["start"].Value, OptionsSettings.Instance().CommandColor);
            ColorString(m.Groups["var"].Value, OptionsSettings.Instance().ExpressionColor);
            ColorString(m.Groups["assign"].Value, OptionsSettings.Instance().ExpressionColor);
            ColorExpression(line.Substring(m.Value.Length), linenr);
            NextLine();
        }
        /// <summary>
        /// Een Void Function call verwacht een Naam met haakjes en eventueel een parameterlijst
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessVoidFunctionCall(string line, int linenr)
        {
            Match m = Regex.Match(line, @"^(?<start>\s*@\s*)");
            if (!m.Success)
            {
                ColorLine(line, OptionsSettings.Instance().ErrorColor);
                return;
            }

            ColorString(m.Groups["start"].Value, OptionsSettings.Instance().CommandColor);
            ColorExpression(line.Substring(m.Value.Length), linenr);
            NextLine();
        }

        /// <summary>
        /// Een If verwacht een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessIf(string line, int linenr)
        {
            int startpos = line.IndexOf("If") + 2;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);
            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }
        /// <summary>
        /// Een ElseIf verwacht een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessElseIf(string line, int linenr)
        {
            int startpos = line.IndexOf("ElseIf") + 6;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);
            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }
        /// <summary>
        /// Een Return verwacht een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessReturn(string line, int linenr)
        {
            int startpos = line.IndexOf("Return") + 6;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);
            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }
        /// <summary>
        /// Een Error verwacht een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessError(string line, int linenr)
        {
            int startpos = line.IndexOf("Error") + 5;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);
            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }
        /// <summary>
        /// Een Info verwacht een expressie
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessInfo(string line, int linenr)
        {
            int startpos = line.IndexOf("Info") + 5;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);
            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }
        /// <summary>
        /// We krijgen iets mee wat een expressie en een lineend moet zijn.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="linenr"></param>
        private void ProcessExpressionAndLineEnd(string line, int linenr)
        {
            ColorExpression(line, linenr);
            NextLine();
        }
        private void NextLine()
        {
            txtText.AppendText("\n");
        }
        /// <summary>
        /// De string uitsnorren als een expressie en toevoegen aan de 
        /// text in de juiste kleuren
        /// </summary>
        /// <param name="exp"></param>
        private void ColorExpression(string expression, int linenr)
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

        /// <summary>
        /// Process the While command.
        /// First parameter is the expression to test, while true, continue
        /// </summary>
        /// <param name="line"></param>
        private void ProcessWhile(string line, int linenr)
        {
            int startpos = line.IndexOf("While") + 5;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);

            globalvars.Add(KnownVariable.LevelUp);
            actualvars.Add(KnownVariable.LevelUp);

            ProcessExpressionAndLineEnd(line.Substring(startpos), linenr);
        }


        private void ProcessVarDeclaration(string line, int linenr, string VarOrGlobal)
        {
            int startpos = line.IndexOf(VarOrGlobal) + VarOrGlobal.Length;
            ColorString(line.Substring(0, startpos), OptionsSettings.Instance().CommandColor);

            string typestring;
            string rest = line.Substring(startpos);
            Match m = Regex.Match(rest, @"^\s*(?<type>[-\w]+)\s*");
            if (!m.Success)
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }
            typestring = m.Groups["type"].Value;
            if (typestring == "Decimal" ||
                typestring == "String" ||
                typestring == "Boolean")
            {
                ColorString(m.Value, OptionsSettings.Instance().ClassColor);
            }
            else if ( Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), typestring) >= 0)
            {
                ColorString(m.Value, OptionsSettings.Instance().ClassColor);
            }
//            else if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("concept"), typestring) >= 0)
//            {
//                ColorString(m.Value, OptionsSettings.Instance().ClassColor);
//            }
            else
            {
                ColorString(m.Value, OptionsSettings.Instance().ErrorColor);
            }
            rest = rest.Substring(m.Length);
            m = Regex.Match(rest, @"^\s*(?<naam>\w+)\b");
            if (!m.Success)
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
                return;
            }
            string naamstring = m.Groups["naam"].Value;
            ColorString(m.Value, OptionsSettings.Instance().ExpressionColor);
            rest = rest.Substring(m.Length);
            if (Regex.IsMatch(rest, @"^\s*=[\w\s]+"))
            {
                ColorLine(rest, OptionsSettings.Instance().ExpressionColor);
            }
            else
            {
                ColorLine(rest, OptionsSettings.Instance().ErrorColor);
            }
            
            KnownVariable kv = new KnownVariable(naamstring, typestring, linenr, VariableSource.Variable);
            globalvars.Add(kv);
            actualvars.Add(kv);
        }

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

    }
    /*
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
        public KnownVariable(string n, string t, int l, VariableSource vs) : this(n, t, l, vs, "")
        {}
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
    }*/
}