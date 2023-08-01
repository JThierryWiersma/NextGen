namespace Generator
{
    partial class NewOpenProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewOpenProject));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpOpen = new System.Windows.Forms.GroupBox();
            this.lstProjects = new System.Windows.Forms.ListBox();
            this.btnProject = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTemplates = new System.Windows.Forms.Button();
            this.btnDefinition = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.grpNew = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lstSolutions = new System.Windows.Forms.ListBox();
            this.btnSolution = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpOpen.SuspendLayout();
            this.grpNew.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpOpen);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpNew);
            this.splitContainer1.Size = new System.Drawing.Size(342, 276);
            this.splitContainer1.SplitterDistance = 156;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.TabStop = false;
            // 
            // grpOpen
            // 
            this.grpOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOpen.Controls.Add(this.lstProjects);
            this.grpOpen.Controls.Add(this.btnProject);
            this.grpOpen.Controls.Add(this.label3);
            this.grpOpen.Controls.Add(this.btnTemplates);
            this.grpOpen.Controls.Add(this.btnDefinition);
            this.grpOpen.Controls.Add(this.btnOpen);
            this.grpOpen.Location = new System.Drawing.Point(12, 8);
            this.grpOpen.Name = "grpOpen";
            this.grpOpen.Size = new System.Drawing.Size(317, 145);
            this.grpOpen.TabIndex = 1;
            this.grpOpen.TabStop = false;
            this.grpOpen.Text = "Open existing project";
            // 
            // lstProjects
            // 
            this.lstProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProjects.ColumnWidth = 150;
            this.lstProjects.FormattingEnabled = true;
            this.lstProjects.IntegralHeight = false;
            this.lstProjects.Location = new System.Drawing.Point(119, 17);
            this.lstProjects.MultiColumn = true;
            this.lstProjects.Name = "lstProjects";
            this.lstProjects.Size = new System.Drawing.Size(107, 120);
            this.lstProjects.TabIndex = 1;
            this.toolTip.SetToolTip(this.lstProjects, "Select one of the recently used projects below or start a new project using the P" +
        "roject... button");
            this.lstProjects.SelectedIndexChanged += new System.EventHandler(this.CheckOpenButtonsStatus);
            this.lstProjects.Enter += new System.EventHandler(this.lstProjects_Enter);
            this.lstProjects.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstProjects_MouseDoubleClick);
            // 
            // btnProject
            // 
            this.btnProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProject.Location = new System.Drawing.Point(232, 111);
            this.btnProject.Name = "btnProject";
            this.btnProject.Size = new System.Drawing.Size(79, 26);
            this.btnProject.TabIndex = 5;
            this.btnProject.Text = "&Project...";
            this.toolTip.SetToolTip(this.btnProject, "Browse to a project not in the last-recent-used list");
            this.btnProject.UseVisualStyleBackColor = true;
            this.btnProject.Click += new System.EventHandler(this.btnProject_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Last used projects:";
            // 
            // btnTemplates
            // 
            this.btnTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTemplates.Location = new System.Drawing.Point(232, 79);
            this.btnTemplates.Name = "btnTemplates";
            this.btnTemplates.Size = new System.Drawing.Size(79, 26);
            this.btnTemplates.TabIndex = 4;
            this.btnTemplates.Text = "&Templates";
            this.toolTip.SetToolTip(this.btnTemplates, "Open the system concept and template editor for the selected project");
            this.btnTemplates.UseVisualStyleBackColor = true;
            this.btnTemplates.Visible = false;
            this.btnTemplates.Click += new System.EventHandler(this.btnTemplates_Click);
            // 
            // btnDefinition
            // 
            this.btnDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefinition.Location = new System.Drawing.Point(232, 47);
            this.btnDefinition.Name = "btnDefinition";
            this.btnDefinition.Size = new System.Drawing.Size(79, 26);
            this.btnDefinition.TabIndex = 3;
            this.btnDefinition.Text = "&Definition";
            this.toolTip.SetToolTip(this.btnDefinition, "Open the definition of the selected project");
            this.btnDefinition.UseVisualStyleBackColor = true;
            this.btnDefinition.Visible = false;
            this.btnDefinition.Click += new System.EventHandler(this.btnDefinition_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(232, 15);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(79, 26);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "&Open";
            this.toolTip.SetToolTip(this.btnOpen, "Open the selected project");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // grpNew
            // 
            this.grpNew.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNew.Controls.Add(this.btnCancel);
            this.grpNew.Controls.Add(this.label4);
            this.grpNew.Controls.Add(this.lstSolutions);
            this.grpNew.Controls.Add(this.btnSolution);
            this.grpNew.Controls.Add(this.btnNew);
            this.grpNew.Location = new System.Drawing.Point(12, 3);
            this.grpNew.Name = "grpNew";
            this.grpNew.Size = new System.Drawing.Size(317, 110);
            this.grpNew.TabIndex = 0;
            this.grpNew.TabStop = false;
            this.grpNew.Text = "Create project";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(232, 80);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 26);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.toolTip.SetToolTip(this.btnCancel, "Start a new project based on the selected solution");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Last used DSL";
            // 
            // lstSolutions
            // 
            this.lstSolutions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSolutions.ColumnWidth = 150;
            this.lstSolutions.FormattingEnabled = true;
            this.lstSolutions.IntegralHeight = false;
            this.lstSolutions.Location = new System.Drawing.Point(119, 16);
            this.lstSolutions.MultiColumn = true;
            this.lstSolutions.Name = "lstSolutions";
            this.lstSolutions.Size = new System.Drawing.Size(107, 88);
            this.lstSolutions.TabIndex = 6;
            this.toolTip.SetToolTip(this.lstSolutions, "Select one of the recently used solutions below to create a project for or select" +
        " a solution type using the Solution.... button");
            this.lstSolutions.SelectedIndexChanged += new System.EventHandler(this.CheckNewButtonsStatus);
            this.lstSolutions.Enter += new System.EventHandler(this.lstSolutions_Enter);
            this.lstSolutions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSolutions_MouseDoubleClick);
            // 
            // btnSolution
            // 
            this.btnSolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSolution.Location = new System.Drawing.Point(232, 48);
            this.btnSolution.Name = "btnSolution";
            this.btnSolution.Size = new System.Drawing.Size(79, 26);
            this.btnSolution.TabIndex = 8;
            this.btnSolution.Text = "&DSL...";
            this.toolTip.SetToolTip(this.btnSolution, "To browse to a solution not in de last-recent-used list");
            this.btnSolution.UseVisualStyleBackColor = true;
            this.btnSolution.Click += new System.EventHandler(this.btnSolution_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(232, 16);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(79, 26);
            this.btnNew.TabIndex = 7;
            this.btnNew.Text = "&New...";
            this.toolTip.SetToolTip(this.btnNew, "Start a new project based on the selected solution");
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xmt";
            this.openFileDialog.Filter = "Template\tfiles (*.xmt)|*.xmt";
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Title = "Select template file\tto open";
            // 
            // NewOpenProject
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(342, 276);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 310);
            this.Name = "NewOpenProject";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing or New Project";
            this.Load += new System.EventHandler(this.NewOpenProject_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpOpen.ResumeLayout(false);
            this.grpOpen.PerformLayout();
            this.grpNew.ResumeLayout(false);
            this.grpNew.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpOpen;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnTemplates;
        private System.Windows.Forms.Button btnDefinition;
        private System.Windows.Forms.GroupBox grpNew;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSolution;
        private System.Windows.Forms.Button btnProject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListBox lstProjects;
        private System.Windows.Forms.ListBox lstSolutions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
    }
}