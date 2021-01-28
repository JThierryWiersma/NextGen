using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Generator.Utility;

namespace Generator
{
    public partial class NewOpenProject : Form
    {
        private bool _default_new = false;

        public NewOpenProject()
        {
            InitializeComponent();
        }
        public NewOpenProject(bool default_new)
        {
            InitializeComponent();
            _default_new = default_new;
        }
        private void cmbProjects_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnOpen;
        }

        private void cmbSolutions_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnNew;
        }

        private void CheckOpenButtonsStatus(object sender, EventArgs e)
        {
            if (lstProjects.SelectedIndex != -1)
                //cmbProjects.Text.Trim() != "")
            {
                btnOpen.Enabled = true;
                btnDefinition.Enabled = true;
                btnTemplates.Enabled = true;
            }
            else
            {
                btnOpen.Enabled = false;
                btnDefinition.Enabled = false;
                btnTemplates.Enabled = false;
            }
        }

        private void CheckNewButtonsStatus(object sender, EventArgs e)
        {
            if (lstSolutions.SelectedIndex != -1 )
                //cmbSolutions.Text.Trim() != "")
            {
                btnNew.Enabled = true;
            }
            else
            {
                btnNew.Enabled = false;
            }
        }

        private void NewOpenProject_Load(object sender, EventArgs e)
        {
            foreach (string filename in OptionsSettings.Instance().GetLastUsedProjects())
            {
                lstProjects.Items.Add(new FileSelector(filename));
            }
            foreach (string filename in OptionsSettings.Instance().GetLastUsedSolutions())
            {
                lstSolutions.Items.Add(new FileSelector(filename));
            }
            
            // Als we voor nieuw kozen, of als er nog geen projects zijn, kies standaard
            // voor een nieuwe solution.
            if (_default_new || lstProjects.Items.Count == 0)
            {
                lstSolutions.Focus();
            }
            else
            {
                lstProjects.Focus();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FileSelector fs = (FileSelector)lstSolutions.SelectedItem;
            if (fs == null) return;

            string file = fs.CompletePath;

            TemplateCache.Instance().Clear(true);
            try
            {
                TemplateCache.Instance().LoadSolutionFile(file, false);
            }
            catch (ApplicationException aex)
            {
                OptionsSettings.Instance().UnRegisterLastUsedSolution(file);
                lstSolutions.Items.Remove(fs);
                MessageBox.Show(aex.Message, "Load solution error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.Title = "Create projectfile";
            openFileDialog.DefaultExt = ".xmp";
            openFileDialog.Filter = "NextGen projects (*.xmp)|*.xmp";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            // Treat all files to be opened. We'll have to do the same
            // as when initial loading with a list of filenames.
            file = openFileDialog.FileName;
            XmlDocument d = new XmlDocument();
            d.AppendChild(d.CreateElement("project"));
            XmlElement elm = d.CreateElement("solution");
            elm.AppendChild(d.CreateTextNode(TemplateCache.Instance().Solution));
            d.DocumentElement.AppendChild(elm);
            elm = d.CreateElement("solutionfilename");
            elm.AppendChild(d.CreateTextNode(TemplateCache.Instance().SolutionFilename));
            d.DocumentElement.AppendChild(elm);
            d.Save(file);

            string[] args = new string[] { file };

            try
            {
                TemplateMain.Instance().InitializeApplication(args);
                Close();
            }
            catch (ApplicationException aex)
            {
                OptionsSettings.Instance().UnRegisterLastUsedProject(file);
                MessageBox.Show(aex.Message, "New project error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Browse for the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProject_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Select NextGen project to open";
            openFileDialog.Filter = "NextGen projects (*.xmp)|*.xmp";
            openFileDialog.RestoreDirectory = false;

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            // Treat all files to be opened. We'll have to do the same
            // as when initial loading with a list of filenames.
            string file = openFileDialog.FileName;
            string[] files = new string[] { file };

            TemplateCache.Instance().Clear(true);
            try
            {
                TemplateMain.Instance().InitializeApplication(files);
                Close();
            }
            catch (ApplicationException aex)
            {
                OptionsSettings.Instance().UnRegisterLastUsedProject(file);
                MessageBox.Show(aex.Message, "Open project error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            // Treat all files to be opened. We'll have to do the same
            // as when initial loading with a list of filenames.
            FileSelector fs = (FileSelector)lstProjects.SelectedItem;
            if (fs == null) return;
            
            string file = fs.CompletePath;

            TemplateCache.Instance().Clear(true);
            try
            {
                TemplateMain.Instance().InitializeApplication( new string[] {file});
                Close();
            }
            catch (ApplicationException aex)
            {
                OptionsSettings.Instance().UnRegisterLastUsedProject(file);
                lstProjects.Items.Remove(fs);
                MessageBox.Show(aex.Message, "Open project error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstProjects_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.AcceptButton.PerformClick();
        }

        private void lstSolutions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.AcceptButton.PerformClick();
        }

        private void btnDefinition_Click(object sender, EventArgs e)
        {
            FileSelector fs = (FileSelector)lstProjects.SelectedItem;
            if (fs == null) return;
            
            string file = fs.CompletePath;

            // Laad het project bestand. 
            // Zoek de solution op die erbij hoort
            // Pluis uit waar de solution gedefinieerd is
            // En open dat project
            XmlDocument d = new XmlDocument();
            try
            {
                d.Load(file);
                string solution_file = "";
                string solution = d.SelectSingleNode("project/solution").InnerText;
                XmlNode location = d.SelectSingleNode("project/solutionfilename");
                if (location != null)
                {
                    solution_file = location.InnerText;
                }
                else
                {
                    solution_file = solution;
                }

                solution_file = Generator.Utility.TemplateUtil.Instance().CombineAndCompact(file, solution_file);
                d.Load(solution_file);
                XmlNode sourceproject = d.SelectSingleNode("solution/sourceproject");
                if (sourceproject == null)
                {
                    throw new ApplicationException("Could not find sourceproject for solution '" + solution + "'");
                }

                file = sourceproject.InnerText;
                file = Generator.Utility.TemplateUtil.Instance().CombineAndCompact(solution_file, file);

                TemplateCache.Instance().Clear(true);
                TemplateMain.Instance().InitializeApplication(new string[] { file });
                Close();
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message, "Open definition project error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnTemplates_Click(object sender, EventArgs e)
        {
            FileSelector fs = (FileSelector)lstProjects.SelectedItem;
            string file = fs.CompletePath;

            // Laad het project bestand. 
            // Zoek de solution op die erbij hoort
            // Instrueer de cache dat we een project hebben dat een solution 'na doet'
            // Zodat als er wat gevraagd wordt, (lijst met types e.d.) dit juist
            // geinterpreteerd wordt.
            TemplateCache.Instance().Clear(true);
            try
            {

                TemplateCache.Instance().LoadProjectFile(file, true);
                TemplateMain.Instance().InitializeApplication(null);
                Close();
            }
            catch (ApplicationException aex)
            {
                OptionsSettings.Instance().UnRegisterLastUsedProject(file);
                lstProjects.Items.Remove(fs);
                MessageBox.Show(aex.Message, "Open template project error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstProjects_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnOpen;
            if (lstProjects.Items.Count > 0 &&
                lstProjects.SelectedIndex == -1)
            {
                lstProjects.SelectedIndex = 0;
            }
            if (lstSolutions.SelectedIndex >= 0)
            {
                lstSolutions.SelectedIndex = -1;
            }

            CheckNewButtonsStatus(sender, e);
            CheckOpenButtonsStatus(sender, e);
        }

        /// <summary>
        /// Zorg dat de juiste knop op default staat en kies
        /// een solution als dat kan. Maak selectie in projects ongedaan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstSolutions_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnNew;
            if (lstSolutions.Items.Count > 0 &&
                lstSolutions.SelectedIndex == -1)
            {
                lstSolutions.SelectedIndex = 0;
            }
            if (lstProjects.SelectedIndex >= 0)
            {
                lstProjects.SelectedIndex = -1;
            }
            CheckNewButtonsStatus(sender, e);
            CheckOpenButtonsStatus(sender, e);
        }

        /// <summary>
        /// Zoek een solution op en zet die in de lijst, zodat er een nieuw
        /// project gestart kan worden op basis van die solution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSolution_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Select NextGen solution to open";
            openFileDialog.Filter = "NextGen solution (*.xms)|*.xms";
            openFileDialog.RestoreDirectory = false;

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            // Treat all files to be opened. We'll have to do the same
            // as when initial loading with a list of filenames.
            string file = openFileDialog.FileName;
            string[] files = new string[] { file };

            lstSolutions.Items.Insert(0, new FileSelector(file));
            lstSolutions.SelectedIndex = 0;
        }
    }
    public class FileSelector
    {
        private string _completepath;
        private string _name;
        public FileSelector(string completepath)
        {
            _completepath = completepath;
            _name = Path.GetFileName(_completepath);
        }
        public override string ToString()
        {
            return _name;
        }
        public string CompletePath
        {
            get { return _completepath; }
        }
    }
}