using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Collections.Specialized;
using System.IO;

namespace Generator.Utility
{
    /*private enum TemplateColors
    {
        CommandColor,
        CommentColor,
        ExpressionColor,
        VariableDeclarationColor,
        CodeColor,
    }*/

    public class OptionsSettings
    {
        const int    MAX_RECENTLYUSED_TO_REMEMBER   = 32;
        const string LRU_PROJECT                    = "LruProject";
        const string LRU_SOLUTION                   = "LruSolution";

        static private OptionsSettings _instance;
        static public OptionsSettings Instance()
        {
            if (_instance == null)
            {
                _instance = new OptionsSettings();
            }
            return _instance;
        }

        private Color _commandColor;

        public Color CommandColor
        {
            get { return _commandColor; }
            set { _commandColor = value; }
        }
        private Color _commentColor;

        public Color CommentColor
        {
            get { return _commentColor; }
            set { _commentColor = value; }
        }
        private Color _expressionColor;

        public Color ExpressionColor
        {
            get { return _expressionColor; }
            set { _expressionColor = value; }
        }
        private Color _codeColor;
        public Color CodeColor
        {
            get { return _codeColor; }
            set { _codeColor = value; }
        }
        private Color _classColor;
        public Color ClassColor
        {
            get { return _classColor; }
            set { _classColor = value; }
        }
        private Color _errorColor;
        public Color ErrorColor
        {
            get { return _errorColor; }
            set { _errorColor = value; }
        }
        private int _templatetabs;

        public int Templatetabs
        {
            get { return _templatetabs; }
            set { _templatetabs = value; }
        }
        private int _generatedtabs;

        public int Generatedtabs
        {
            get { return _generatedtabs; }
            set { _generatedtabs = value; }
        }

        public void SaveSettings()
        {

            RegistryKey masterkey = GetOurKey();
            if (masterkey == null)
            {
                throw new ApplicationException("Unable to open the registry");
            }

            masterkey.SetValue("ClassColor", _classColor.ToArgb(), RegistryValueKind.DWord);
            masterkey.SetValue("CommandColor", _commandColor.ToArgb(), RegistryValueKind.DWord);
            masterkey.SetValue("CommentColor", _commentColor.ToArgb(), RegistryValueKind.DWord);
            masterkey.SetValue("ExpressionColor", _expressionColor.ToArgb(), RegistryValueKind.DWord);
            masterkey.SetValue("CodeColor", _codeColor.ToArgb(), RegistryValueKind.DWord);
            masterkey.SetValue("ErrorColor", _errorColor.ToArgb(), RegistryValueKind.DWord);

            masterkey.SetValue("TemplateTabs", _templatetabs, RegistryValueKind.DWord);
            masterkey.SetValue("GeneratedTabs", _generatedtabs, RegistryValueKind.DWord);
            masterkey.Flush();
        }
        private OptionsSettings()
        {
            RegistryKey masterkey = GetOurKey();
            // Create/get colorkeys
            _classColor = Color.FromArgb((int)masterkey.GetValue("ClassColor", Color.DarkRed.ToArgb()));
            _commandColor = Color.FromArgb((int)masterkey.GetValue("CommandColor", Color.SteelBlue.ToArgb()));
            _commentColor = Color.FromArgb((int)masterkey.GetValue("CommentColor", Color.LawnGreen.ToArgb()));
            _expressionColor = Color.FromArgb((int)masterkey.GetValue("ExpressionColor", Color.SteelBlue.ToArgb()));
            _codeColor = Color.FromArgb((int)masterkey.GetValue("CodeColor", Color.Black.ToArgb()));
            _errorColor = Color.FromArgb((int)masterkey.GetValue("ErrorColor", Color.Red.ToArgb()));

            // Get tab settings
            _templatetabs = (Int32) masterkey.GetValue("TemplateTabs", 2);
            _generatedtabs = (Int32) masterkey.GetValue("GeneratedTabs", 4);
        }

        /// <summary>
        /// Returns the last used solution folder, if any.
        /// The parent of the folder where the .xms file was found.
        /// </summary>
        /// <returns></returns>
        public string GetLastUsedSolutionFolder()
        {
            RegistryKey masterkey = GetOurKey();
            return (string)masterkey.GetValue("LastUsedSolutionFolder", "");
        }

        /// <summary>
        /// Sets the last used solution folder.
        /// The parent of the folder where the .xms file was found.
        /// </summary>
        /// <returns></returns>
        public void SetLastUsedSolutionFolder(string Folder)
        {
            RegistryKey masterkey = GetOurKey();
            masterkey.SetValue("LastUsedSolutionFolder", Folder, RegistryValueKind.String);
        }

        /// <summary>
        /// Returns the last used project folder, if any
        /// The parent of the folder where the .xmp file was found.
        /// </summary>
        /// <returns></returns>
        public string GetLastUsedProjectFolder()
        {
            RegistryKey masterkey = GetOurKey();
            return (string)masterkey.GetValue("LastUsedProjectFolder", "");
        }

        /// <summary>
        /// Sets the last used project folder.
        /// The parent of the folder where the .xmp file was found.
        /// </summary>
        /// <returns></returns>
        public void SetLastUsedProjectFolder(string Folder)
        {
            RegistryKey masterkey = GetOurKey();
            masterkey.SetValue("LastUsedProjectFolder", Folder, RegistryValueKind.String);
        }

        /// <summary>
        /// Doorloop alle waardes genaamd 't'_{getal startend op 0} en verzamel 
        /// de string waardes in een lijst. Geef die lijst terug op het moment
        /// dat het element niet meer gevonden is.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private StringCollection CollectLruListFor(string t)
        {
            StringCollection result = new StringCollection();
            RegistryKey masterkey = GetOurKey();
            for (int i = 0; i < MAX_RECENTLYUSED_TO_REMEMBER; i++)
            {
                string valuename = String.Format("{0}_{1}", t, i.ToString());
                string filename = (string)masterkey.GetValue(valuename, "");
                if (filename == String.Empty)
                    break;
                result.Add(filename);
            }
            return result;
        }
        /// <summary>
        /// Geef een lijst met de laatst gebruikte projectfiles terug
        /// </summary>
        /// <returns></returns>
        public string[] GetLastUsedProjects()
        {
            StringCollection filenames = CollectLruListFor(LRU_PROJECT);
            string[] result = new string[filenames.Count];
            filenames.CopyTo(result, 0);
            return result;
        }
        /// <summary>
        /// Lever een lijst met de laatst gebruikte solutions op.
        /// Zorg ervoor dat de DSL-Design solution er altijd bij staat.
        /// </summary>
        /// <returns></returns>
        public string[] GetLastUsedSolutions()
        {
            StringCollection filenames = CollectLruListFor(LRU_SOLUTION);
            /* if (!filenames.Contains("DSL Design"))
            {
                if (filenames.Count >= MAX_RECENTLYUSED_TO_REMEMBER)
                    filenames.RemoveAt(MAX_RECENTLYUSED_TO_REMEMBER - 1);
                filenames.Add("DSL Design");
            } */
            string[] result = new string[filenames.Count];
            filenames.CopyTo(result, 0);
            return result;
        }

        public void ValidateLastUsedSolutions()
        {
            foreach (string file in this.GetLastUsedSolutions())
            {
                if (! File.Exists(file))
                {
                    this.UnRegisterLastUsedSolution(file);
                }
                else
                {
                    this.RegisterFor(LRU_SOLUTION, file);
                }
            }
        }
        public void ValidateLastUsedProjects()
        {
            foreach (string file in this.GetLastUsedProjects())
            {
                if (!File.Exists(file))
                {
                    this.UnRegisterLastUsedProject(file);
                }
                else
                {
                    this.RegisterFor(LRU_PROJECT, file);
                }
            }
        }

        /// <summary>
        /// Neem voor de Last-recent-used list met de gegeven naam t, 
        /// de meegegeven filename op.
        /// Check eerst of ie al in de lijst staat en haal hem eruit, zet hem dan 
        /// op positie 0 er weer in en sla de lijst op.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="filename"></param>
        private void RegisterFor(string t, string filename)
        {
            StringCollection strings = CollectLruListFor(t);
            while (strings.Contains(filename))
            {
                strings.Remove(filename);
            }
            strings.Insert(0, filename);

            RegistryKey masterkey = GetOurKey();
            for (int i = 0; i < MAX_RECENTLYUSED_TO_REMEMBER; i++)
            {
                string valuename = String.Format("{0}_{1}", t, i.ToString());
                if (i < strings.Count)
                {
                    masterkey.SetValue(valuename, strings[i], RegistryValueKind.String);
                }
                else 
                {
                    masterkey.DeleteValue(valuename, false);
                }
            }
            masterkey.Flush();
        }
        /// <summary>
        /// Registreer dat de meegegeven project en solution net gebruikt zijn
        /// en deze dus op een relevante manier in de LRU lijsten moeten komen.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="solution"></param>
        public void RegisterLastUsedProject(string project, string solution)
        {
            RegisterFor(LRU_PROJECT, project);
            RegisterFor(LRU_SOLUTION, solution);
        }

        /// <summary>
        /// Verwijder het bestand uit de lijst. Het was niet meer geldig en willen
        /// we dus niet oneindig in onze lru lijst hebben staan.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="file"></param>
        private void UnRegister(string t, string file)
        {
            RegistryKey masterkey = GetOurKey();
            string previousvalue = "";

            for (int i = 0; i < MAX_RECENTLYUSED_TO_REMEMBER; i++)
            {
                string valuename = String.Format("{0}_{1}", t, i.ToString());
                string filename = (string)masterkey.GetValue(valuename, "");
                if (filename == String.Empty)
                    break;
                if (previousvalue == "")
                {
                    if (file == filename)
                    {
                        // boosdoener gevonden. Leeg maken en overschrijven met de opvolgers.
                        masterkey.DeleteValue(valuename);
                        previousvalue = valuename;
                    }
                }
                else
                {
                    masterkey.SetValue(previousvalue, filename);
                    previousvalue = valuename;
                }
            }
        }
        public void UnRegisterLastUsedProject(string project)
        {
            UnRegister(LRU_PROJECT, project);
        }
        public void UnRegisterLastUsedSolution(string solution)
        {
            UnRegister(LRU_SOLUTION, solution);
        }

        private RegistryKey GetOurKey()
        {
            RegistryKey result = System.Windows.Forms.Application.UserAppDataRegistry;
            return result;
        }
    }
}
