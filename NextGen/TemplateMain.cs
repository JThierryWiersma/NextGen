/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Forms;
using Generator.Utility;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Drawing;
using Generator.Exceptions;
using Generator.ObserverPattern;
using Generator.Statements;

namespace Generator
{
    public delegate void AddToOutputTextCallback(string firstpart, string linkeditem, string lastpart, LinkInfoType linktype, SourceCodeContext scc);
    
    /// <summary>
	/// Summary description for TemplateMain.
	/// </summary>
	public class TemplateMain : System.Windows.Forms.Form, IObserver
    {
        private System.ComponentModel.IContainer components;
        private MenuItem mnuNewProject;
        private MenuItem mnuOpenProject;
        private MenuItem menuItem1;
        private ToolStripStatusLabel toolStripStatusLabel3;
        public ToolStripStatusLabel toolStripProject;
        public ToolStripStatusLabel toolStripSolution;
        private MenuItem mnuCopy;
        private bool firstload = true;
		public TemplateMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
            GenerateOutput.Instance().RemoveObserver(this);
            
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("xxxx");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Concepts", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node6");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node5ffffff", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateMain));
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("xxxx");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Concepts", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node6");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Node5ffffff", new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14,
            treeNode15});
            this.mnuMain = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuNewProject = new System.Windows.Forms.MenuItem();
            this.mnuOpenProject = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuOpen = new System.Windows.Forms.MenuItem();
            this.mnuAdd = new System.Windows.Forms.MenuItem();
            this.mnuNewFile = new System.Windows.Forms.MenuItem();
            this.mnuCopy = new System.Windows.Forms.MenuItem();
            this.mnuDelete = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.mnuAddNew = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.mnuSolution = new System.Windows.Forms.MenuItem();
            this.mnuImport = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.mnuView = new System.Windows.Forms.MenuItem();
            this.mnuViewConcepts = new System.Windows.Forms.MenuItem();
            this.mnuViewOutput = new System.Windows.Forms.MenuItem();
            this.mnuTools = new System.Windows.Forms.MenuItem();
            this.mnuReloadCache = new System.Windows.Forms.MenuItem();
            this.miEE = new System.Windows.Forms.MenuItem();
            this.mnuSaveCache = new System.Windows.Forms.MenuItem();
            this.mnuInvent = new System.Windows.Forms.MenuItem();
            this.ChangeDefinition = new System.Windows.Forms.MenuItem();
            this.mnuOptions = new System.Windows.Forms.MenuItem();
            this.mnuRegistration = new System.Windows.Forms.MenuItem();
            this.mnuWindows = new System.Windows.Forms.MenuItem();
            this.mnuCloseAll = new System.Windows.Forms.MenuItem();
            this.mnuCloseBut = new System.Windows.Forms.MenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabWindow = new System.Windows.Forms.TabControl();
            this.tabConcepts = new System.Windows.Forms.TabPage();
            this.lstConceptList = new System.Windows.Forms.TreeView();
            this.conceptfolder = new System.Windows.Forms.ImageList(this.components);
            this.tabTemplates = new System.Windows.Forms.TabPage();
            this.lstTemplateList = new System.Windows.Forms.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.GenerationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSolution = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1.SuspendLayout();
            this.tabWindow.SuspendLayout();
            this.tabConcepts.SuspendLayout();
            this.tabTemplates.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuTools,
            this.mnuWindows});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuNewProject,
            this.mnuOpenProject,
            this.menuItem1,
            this.mnuOpen,
            this.mnuAdd,
            this.mnuCopy,
            this.mnuDelete,
            this.menuItem8,
            this.mnuAddNew,
            this.menuItem9,
            this.mnuSolution,
            this.mnuImport,
            this.menuItem13,
            this.mnuExit});
            this.mnuFile.Text = "&File";
            this.mnuFile.Popup += new System.EventHandler(this.mnuFile_Popup);
            // 
            // mnuNewProject
            // 
            this.mnuNewProject.Index = 0;
            this.mnuNewProject.Text = "&New Project";
            this.mnuNewProject.Click += new System.EventHandler(this.mnuNewProject_Click_1);
            // 
            // mnuOpenProject
            // 
            this.mnuOpenProject.Index = 1;
            this.mnuOpenProject.Text = "&Open Project";
            this.mnuOpenProject.Click += new System.EventHandler(this.mnuOpenProject_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // mnuOpen
            // 
            this.mnuOpen.Index = 3;
            this.mnuOpen.Text = "&Open";
            this.mnuOpen.Visible = false;
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuAdd
            // 
            this.mnuAdd.Index = 4;
            this.mnuAdd.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuNewFile});
            this.mnuAdd.Text = "&Add";
            // 
            // mnuNewFile
            // 
            this.mnuNewFile.Index = 0;
            this.mnuNewFile.Text = "&File";
            // 
            // mnuCopy
            // 
            this.mnuCopy.Index = 5;
            this.mnuCopy.Text = "&Copy";
            this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Index = 6;
            this.mnuDelete.Text = "&Delete";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            this.menuItem8.Text = "-";
            this.menuItem8.Visible = false;
            // 
            // mnuAddNew
            // 
            this.mnuAddNew.Index = 8;
            this.mnuAddNew.Text = "&Add\tnew";
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 9;
            this.menuItem9.Text = "-";
            this.menuItem9.Visible = false;
            // 
            // mnuSolution
            // 
            this.mnuSolution.Index = 10;
            this.mnuSolution.Text = "&Solution";
            // 
            // mnuImport
            // 
            this.mnuImport.Index = 11;
            this.mnuImport.Text = "Import...";
            this.mnuImport.Click += new System.EventHandler(this.mnuImport_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 12;
            this.menuItem13.Text = "-";
            // 
            // mnuExit
            // 
            this.mnuExit.Index = 13;
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuView
            // 
            this.mnuView.Index = 1;
            this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuViewConcepts,
            this.mnuViewOutput});
            this.mnuView.Text = "&View";
            this.mnuView.Popup += new System.EventHandler(this.mnuView_Popup);
            // 
            // mnuViewConcepts
            // 
            this.mnuViewConcepts.Checked = true;
            this.mnuViewConcepts.Index = 0;
            this.mnuViewConcepts.Text = "Concept explorer";
            this.mnuViewConcepts.Click += new System.EventHandler(this.mnuViewConcepts_Click);
            // 
            // mnuViewOutput
            // 
            this.mnuViewOutput.Checked = true;
            this.mnuViewOutput.Index = 1;
            this.mnuViewOutput.Text = "Output window";
            this.mnuViewOutput.Click += new System.EventHandler(this.mnuViewOutput_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.Index = 2;
            this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuReloadCache,
            this.miEE,
            this.mnuSaveCache,
            this.mnuInvent,
            this.ChangeDefinition,
            this.mnuOptions,
            this.mnuRegistration});
            this.mnuTools.MergeOrder = 30;
            this.mnuTools.Text = "&Tools";
            // 
            // mnuReloadCache
            // 
            this.mnuReloadCache.Index = 0;
            this.mnuReloadCache.MergeOrder = 10;
            this.mnuReloadCache.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.mnuReloadCache.Text = "Reload cache";
            this.mnuReloadCache.Visible = false;
            this.mnuReloadCache.Click += new System.EventHandler(this.mnuReloadCache_Click);
            // 
            // miEE
            // 
            this.miEE.Index = 1;
            this.miEE.MergeOrder = 20;
            this.miEE.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.miEE.Text = "ExpressionEvaluator";
            this.miEE.Visible = false;
            this.miEE.Click += new System.EventHandler(this.miEE_Click);
            // 
            // mnuSaveCache
            // 
            this.mnuSaveCache.Index = 2;
            this.mnuSaveCache.Text = "Save\tcache";
            this.mnuSaveCache.Visible = false;
            this.mnuSaveCache.Click += new System.EventHandler(this.mnuSaveCache_Click);
            // 
            // mnuInvent
            // 
            this.mnuInvent.Index = 3;
            this.mnuInvent.Text = "Invent BigInteger";
            this.mnuInvent.Visible = false;
            this.mnuInvent.Click += new System.EventHandler(this.mnuInvent_Click);
            // 
            // ChangeDefinition
            // 
            this.ChangeDefinition.Index = 4;
            this.ChangeDefinition.Text = "Change typedefinition";
            this.ChangeDefinition.Visible = false;
            this.ChangeDefinition.Click += new System.EventHandler(this.ChangeDefinition_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.Index = 5;
            this.mnuOptions.Text = "Options";
            this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
            // 
            // mnuRegistration
            // 
            this.mnuRegistration.Index = 6;
            this.mnuRegistration.Text = "Registration";
            this.mnuRegistration.Click += new System.EventHandler(this.mnuRegistration_Click);
            // 
            // mnuWindows
            // 
            this.mnuWindows.Index = 3;
            this.mnuWindows.MdiList = true;
            this.mnuWindows.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuCloseAll,
            this.mnuCloseBut});
            this.mnuWindows.MergeOrder = 40;
            this.mnuWindows.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.mnuWindows.Text = "&Windows";
            // 
            // mnuCloseAll
            // 
            this.mnuCloseAll.Index = 0;
            this.mnuCloseAll.Text = "Close &All";
            this.mnuCloseAll.Click += new System.EventHandler(this.mnuCloseAll_Click);
            // 
            // mnuCloseBut
            // 
            this.mnuCloseBut.Index = 1;
            this.mnuCloseBut.Text = "Close All &but active";
            this.mnuCloseBut.Click += new System.EventHandler(this.mnuCloseBut_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xmt";
            this.openFileDialog.Filter = "Template\tfiles (*.xmt)|*.xmt";
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Title = "Select template file\tto open";
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOutput.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtOutput.Location = new System.Drawing.Point(0, 359);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(985, 192);
            this.txtOutput.TabIndex = 4;
            this.txtOutput.Text = "";
            this.txtOutput.WordWrap = false;
            this.txtOutput.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtOutput_MouseClick);
            this.txtOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtOutput_MouseMove);
            this.txtOutput.TextChanged += new System.EventHandler(this.txtOutput_TextChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 356);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(985, 3);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // importFileDialog
            // 
            this.importFileDialog.DefaultExt = "*.xml";
            this.importFileDialog.Filter = "XML files|*.xml|All files|*.*";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabWindow);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(707, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(2);
            this.panel1.Size = new System.Drawing.Size(278, 356);
            this.panel1.TabIndex = 17;
            // 
            // tabWindow
            // 
            this.tabWindow.Controls.Add(this.tabConcepts);
            this.tabWindow.Controls.Add(this.tabTemplates);
            this.tabWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWindow.ImageList = this.conceptfolder;
            this.tabWindow.Location = new System.Drawing.Point(2, 2);
            this.tabWindow.Name = "tabWindow";
            this.tabWindow.SelectedIndex = 0;
            this.tabWindow.Size = new System.Drawing.Size(274, 352);
            this.tabWindow.TabIndex = 17;
            // 
            // tabConcepts
            // 
            this.tabConcepts.Controls.Add(this.lstConceptList);
            this.tabConcepts.ImageIndex = 1;
            this.tabConcepts.Location = new System.Drawing.Point(4, 23);
            this.tabConcepts.Name = "tabConcepts";
            this.tabConcepts.Padding = new System.Windows.Forms.Padding(3);
            this.tabConcepts.Size = new System.Drawing.Size(266, 325);
            this.tabConcepts.TabIndex = 0;
            this.tabConcepts.Text = "Concepts";
            this.tabConcepts.UseVisualStyleBackColor = true;
            // 
            // lstConceptList
            // 
            this.lstConceptList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstConceptList.FullRowSelect = true;
            this.lstConceptList.HotTracking = true;
            this.lstConceptList.ImageIndex = 0;
            this.lstConceptList.ImageList = this.conceptfolder;
            this.lstConceptList.Indent = 15;
            this.lstConceptList.ItemHeight = 18;
            this.lstConceptList.Location = new System.Drawing.Point(3, 3);
            this.lstConceptList.Name = "lstConceptList";
            treeNode1.Name = "";
            treeNode1.Text = "Node2";
            treeNode2.Name = "";
            treeNode2.Text = "Node3";
            treeNode3.Name = "";
            treeNode3.Text = "xxxx";
            treeNode4.Name = "";
            treeNode4.Text = "Concepts";
            treeNode5.Name = "";
            treeNode5.Text = "Node6";
            treeNode6.Name = "";
            treeNode6.Text = "Node7";
            treeNode7.Name = "";
            treeNode7.Text = "Node8";
            treeNode8.Name = "";
            treeNode8.Text = "Node5ffffff";
            this.lstConceptList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode8});
            this.lstConceptList.SelectedImageIndex = 0;
            this.lstConceptList.ShowLines = false;
            this.lstConceptList.Size = new System.Drawing.Size(260, 319);
            this.lstConceptList.Sorted = true;
            this.lstConceptList.TabIndex = 9;
            this.lstConceptList.DoubleClick += new System.EventHandler(this.mnuTemplateOpen_Click);
            // 
            // conceptfolder
            // 
            this.conceptfolder.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("conceptfolder.ImageStream")));
            this.conceptfolder.TransparentColor = System.Drawing.Color.Red;
            this.conceptfolder.Images.SetKeyName(0, "explorer_exe_Ico83_ico_Ico1.ico");
            this.conceptfolder.Images.SetKeyName(1, "cabview_dll_Ico14_ico_Ico1.ico");
            this.conceptfolder.Images.SetKeyName(2, "shell32_dll_Ico1084_ico_Ico1.ico");
            this.conceptfolder.Images.SetKeyName(3, "cabview_dll_Ico14_ico_Ico1_New.ico");
            this.conceptfolder.Images.SetKeyName(4, "cabview_dll_Ico14_ico_Ico1_New.PNG");
            // 
            // tabTemplates
            // 
            this.tabTemplates.Controls.Add(this.lstTemplateList);
            this.tabTemplates.ImageIndex = 2;
            this.tabTemplates.Location = new System.Drawing.Point(4, 23);
            this.tabTemplates.Name = "tabTemplates";
            this.tabTemplates.Padding = new System.Windows.Forms.Padding(3);
            this.tabTemplates.Size = new System.Drawing.Size(266, 325);
            this.tabTemplates.TabIndex = 1;
            this.tabTemplates.Text = "Templates";
            this.tabTemplates.UseVisualStyleBackColor = true;
            // 
            // lstTemplateList
            // 
            this.lstTemplateList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTemplateList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTemplateList.FullRowSelect = true;
            this.lstTemplateList.ImageIndex = 0;
            this.lstTemplateList.ImageList = this.conceptfolder;
            this.lstTemplateList.Indent = 15;
            this.lstTemplateList.ItemHeight = 18;
            this.lstTemplateList.Location = new System.Drawing.Point(3, 3);
            this.lstTemplateList.Name = "lstTemplateList";
            treeNode9.Name = "";
            treeNode9.Text = "Node2";
            treeNode10.Name = "";
            treeNode10.Text = "Node3";
            treeNode11.Name = "";
            treeNode11.Text = "xxxx";
            treeNode12.Name = "";
            treeNode12.Text = "Concepts";
            treeNode13.Name = "";
            treeNode13.Text = "Node6";
            treeNode14.Name = "";
            treeNode14.Text = "Node7";
            treeNode15.Name = "";
            treeNode15.Text = "Node8";
            treeNode16.Name = "";
            treeNode16.Text = "Node5ffffff";
            this.lstTemplateList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode16});
            this.lstTemplateList.SelectedImageIndex = 0;
            this.lstTemplateList.ShowLines = false;
            this.lstTemplateList.Size = new System.Drawing.Size(260, 319);
            this.lstTemplateList.Sorted = true;
            this.lstTemplateList.StateImageList = this.conceptfolder;
            this.lstTemplateList.TabIndex = 10;
            this.lstTemplateList.DoubleClick += new System.EventHandler(this.lstTemplateList_DoubleClick);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(702, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(5, 356);
            this.splitter2.TabIndex = 18;
            this.splitter2.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GenerationStatus,
            this.StatusLabel,
            this.ProgressBar,
            this.toolStripStatusLabel3,
            this.toolStripSolution,
            this.toolStripProject});
            this.statusStrip1.Location = new System.Drawing.Point(0, 551);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(985, 24);
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // GenerationStatus
            // 
            this.GenerationStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.GenerationStatus.Name = "GenerationStatus";
            this.GenerationStatus.Size = new System.Drawing.Size(100, 19);
            this.GenerationStatus.Text = "Generationstatus";
            this.GenerationStatus.Visible = false;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(50, 19);
            this.StatusLabel.Text = "Loading";
            this.StatusLabel.Visible = false;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Maximum = 1000;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(100, 18);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.Visible = false;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(861, 19);
            this.toolStripStatusLabel3.Spring = true;
            // 
            // toolStripSolution
            // 
            this.toolStripSolution.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripSolution.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripSolution.Name = "toolStripSolution";
            this.toolStripSolution.Size = new System.Drawing.Size(58, 19);
            this.toolStripSolution.Text = "Solution:";
            this.toolStripSolution.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripProject
            // 
            this.toolStripProject.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripProject.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripProject.Name = "toolStripProject";
            this.toolStripProject.Size = new System.Drawing.Size(51, 19);
            this.toolStripProject.Text = "Project:";
            this.toolStripProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TemplateMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(985, 575);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Menu = this.mnuMain;
            this.Name = "TemplateMain";
            this.Text = "NextGen 2.1.0.12";
            this.Shown += new System.EventHandler(this.TemplateMain_Shown);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TemplateMain_Closing);
            this.Load += new System.EventHandler(this.TemplateMain_Load);
            this.panel1.ResumeLayout(false);
            this.tabWindow.ResumeLayout(false);
            this.tabConcepts.ResumeLayout(false);
            this.tabTemplates.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.MainMenu 
										mnuMain;
        private System.Windows.Forms.MenuItem
                                        miEE;
        public System.Windows.Forms.MenuItem 
										mnuFile;
        private System.Windows.Forms.MenuItem 
										mnuAdd;
        private System.Windows.Forms.MenuItem 
										mnuOpen;
        private System.Windows.Forms.MenuItem 
										mnuAddNew;
        private System.Windows.Forms.MenuItem 
										menuItem8;
        private System.Windows.Forms.MenuItem 
										mnuSolution;
        private System.Windows.Forms.MenuItem 
										menuItem13;
        private System.Windows.Forms.MenuItem 
										mnuExit;
        private System.Windows.Forms.MenuItem 
										menuItem9;
        private System.Windows.Forms.MenuItem 
										mnuReloadCache;
        private System.Windows.Forms.OpenFileDialog 
										openFileDialog;
        private System.Windows.Forms.MenuItem 
										mnuWindows;
        private System.Windows.Forms.MenuItem 
										mnuSaveCache;
        private System.Windows.Forms.MenuItem 
										mnuTools;
        private System.Windows.Forms.MenuItem 
										mnuInvent;
        private System.Windows.Forms.MenuItem
                                        ChangeDefinition;
        private System.Windows.Forms.MenuItem
                                        mnuNewFile;
        private System.Windows.Forms.Splitter 
										splitter1;
        private System.Windows.Forms.RichTextBox
                                        txtOutput;
        private System.Windows.Forms.MenuItem 
										mnuView;
        private System.Windows.Forms.MenuItem 
										mnuViewConcepts;
        private System.Windows.Forms.MenuItem 
										mnuViewOutput;
        private System.Windows.Forms.MenuItem 
										mnuDelete;
        private System.Windows.Forms.MenuItem 
										mnuCloseAll;
        private System.Windows.Forms.MenuItem 
										mnuCloseBut;
        private MenuItem mnuImport;
        private OpenFileDialog importFileDialog;
        private Panel panel1;
        private TabControl tabWindow;
        private TabPage tabConcepts;
        private TreeView lstConceptList;
        private TabPage tabTemplates;
        private Splitter splitter2;
        private TreeView lstTemplateList;
        private ImageList conceptfolder;
        private ImageList imageList1;
        private MenuItem mnuOptions;
        private MenuItem mnuRegistration;
        public StatusStrip statusStrip1;
        private ToolStripStatusLabel GenerationStatus;
        public ToolStripStatusLabel StatusLabel;
        public ToolStripProgressBar ProgressBar;

		private static					TemplateMain cm_Instance = null;
		
		public static TemplateMain Instance()
		{
			if (cm_Instance == null)
				cm_Instance							= new TemplateMain();
			return cm_Instance;
		}

		/// <summary>
		/// Generate the userconcept identified by the concept and name.
		/// Identify the concept by passing the conceptname and the name.
		/// </summary>
		/// <param name="concept_name"></param>
		public static ArrayList GenerateUserConcept(string concept_request, string name_request)
		{
			
			//XmlNode					instance	= TemplateCache.Instance().GetValueFor(name, concept).CloneNode(true);
			XmlNode						alltemplates= TemplateCache.Instance().GetTypesList("__TemplateFile");

			string[]					todolist	= TemplateCache.Instance().GetViewableTypenamesList(true, true);

			// First add all items to do to a list, then do them and present progress...
			ArrayList					todo		= new ArrayList();

			foreach(string concept in TemplateCache.Instance().GetViewableTypenamesList(true, true))
			{
				// allow for passing of '' as concept to get all concepts
				if (concept_request != "" && concept != concept_request)
					continue;

				XmlNode					definition	= TemplateCache.Instance().GetTemplateType(concept);
				foreach (XmlNode instance in TemplateCache.Instance().GetTypesList(concept))
				{
					// Generate all of concept when no name given, or only with the given name
					string				inst_name	= instance.FirstChild.SelectSingleNode(definition.Attributes["nameattribute"].Value).InnerText;
					if (name_request != "" && inst_name != name_request)
						continue;

					// 
					foreach (XmlNode templatefiletype in alltemplates)
					{
						string			applies_to	= templatefiletype.SelectSingleNode("type/appliesto").InnerText;
						if (applies_to == concept)
						{
							todo.Add(new GenerationRequest(
								definition, 
								instance.FirstChild,
								templatefiletype.FirstChild, 
								inst_name)
								);			
						}
					}
				}
			}

			return todo;
		}

		private void TemplateMain_Load(object sender, System.EventArgs e)
		{
#if THIERRY
			mnuSaveCache.Visible					= true;
			mnuInvent.Visible						= true;
#else
			mnuSaveCache.Visible					= false;
			mnuInvent.Visible						= false;
#endif
            InitializeMenus();

			HideOutputText();
		}
	
        /*
		public static void _old_kanweg_LoadSolutionFile(string filename)
		{
			XmlDocument					d			= new XmlDocument();
			bool						loaded		= false;
			Exception					loadex		= null;
			String						location	= "";

			// If path has root, it must be a full path. Otherwise test userdata dir first and then appdir.
			if (Path.IsPathRooted(filename))
			{
				try
				{
					location						= filename;
					d.Load(location);
					loaded							= true;
				}
				catch (Exception ex)
				{
					loadex							= ex;
				}
			}
			else
			{
				// try userdata directory.
				try
				{
					location						= Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
					d.Load(location);
					loaded							= true;
				}
				catch (Exception ex)
				{
					loadex							= ex;	
				}
				// if no success try app path dir.
				if (! loaded)
				{
					try
					{
						location					= Path.Combine(TemplateUtil.Instance().AppPath(), filename);
						d.Load(location);
						loaded						= true;
					}
					catch (Exception ex)
					{
						loadex						= ex;	
					}
				}
			}
			if (! loaded)
			{
				MessageBox.Show("Exception occured during load of solutionfile '" + filename + "'. " + loadex.Message);
			}


			try
			{
				string					solution	= d.SelectSingleNode("solution/code").InnerText;
				//string				location	= d.SelectSingleNode("solution/location").InnerText;
				//location							= TemplateUtil.Instance().AbsolutePath(filename, location);
				string					name		= d.SelectSingleNode("solution/name").InnerText;
				TemplateCache.Instance().SetSolution(solution, name, location, filename);
//				this.Text							= "DSL: " + name;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception occured during load of solutionfile '" + filename + "'. " + ex.Message);
			}
			
		}
        */

		public void InitializeApplication(string[] args)
		{
            HideOutputText();
			this.Cursor								= System.Windows.Forms.Cursors.WaitCursor;

            if (args != null)
            {
                foreach (string arg in args)
                {
                    if (arg.EndsWith(".xmp"))
                        TemplateCache.Instance().LoadProjectFile(arg, false);
                    else if (arg.EndsWith(".xms"))
                        TemplateCache.Instance().LoadSolutionFile(arg, false);
                }
            }
			InitializeMenus();
			this.Cursor								= System.Windows.Forms.Cursors.Default;
		}
		
		private	string					lastbuildmenusforproject;
		private	string					lastbuildmenusforsolution;

		private void InitializeMenus()
		{
            if (lastbuildmenusforsolution != TemplateCache.Instance().Solution)
			{
				// Fill the variable parts of the menus.
				// remove all items from the New menu, except the project.
				mnuAdd.MenuItems.Clear();
				foreach (String typetype in TemplateCache.Instance().GetViewableTypenamesList(true, false))
				{
					// Make 'New>File>' menu
					XmlNode				tt			= TemplateCache.Instance().GetTemplateType(typetype);
					XmlAttribute		shortcut	= tt.Attributes["shortcut"];
					Shortcut			shortcutkey	= Shortcut.None;
					if (shortcut != null && shortcut.Value != "")
						try
						{
							shortcutkey				= (Shortcut) Enum.Parse(typeof(Shortcut), shortcut.Value, true)	;
						}
						catch
						{
							shortcutkey				= Shortcut.None;
						}

					MenuItem			mi			= new MenuItem(typetype, new EventHandler(this.mnuStart_NewItem), shortcutkey);
					mnuAdd.MenuItems.Add(mi);
				}

				if (TemplateCache.Instance().GetViewableTypenamesList(false, true).GetLength(0) > 0)
				{
					mnuTools.Visible				= true;
				}
				else
				{
					mnuTools.Visible				= false;
				}
				lastbuildmenusforsolution			= TemplateCache.Instance().Solution;
				// Change of solution implies change of relevant 
				// content in projectdir.
				lastbuildmenusforproject			= null;	
			}

			InitializeMenusProjectDirectoryDependent();
		}

		private void InitializeMenusProjectDirectoryDependent()
		{
            if (lastbuildmenusforproject != TemplateCache.Instance().ProjectDirectory)
			{
				// Addnew menu doen we niet. Dat doe je maar op meta niveau
				mnuAddNew.Visible					= false;
				mnuSolution.Visible					= false;
				/*
								// Now get all types editable and add this in AddNew menu
								mnuAddNew.MenuItems.Clear();
				//				bool addUser = (TemplateCache.ProjectDirectory != "");
								foreach (String typetype in TemplateCache.Instance().GetViewableTypenamesList(false, true))
								{
									// Make 'AddNew>' menu
									mnuAddNew.MenuItems.Add(typetype, new EventHandler(this.mnuStart_AddNewItem));
								}
				*/
				BuildConceptList(true);
                lastbuildmenusforproject            = TemplateCache.Instance().ProjectDirectory;
			}
			bool						enabled		= (TemplateCache.Instance().ProjectDirectory != "");
			foreach (MenuItem mi in mnuAdd.MenuItems)
			{
    			mi.Enabled						= enabled;
			}
		}

		private void mnuOpenLink_Click(object sender, System.EventArgs e)
		{
			MenuItem					mi			= (MenuItem) sender;
			Control						c			= ((ContextMenu) mi.Parent).SourceControl;
			string						type		= (c.Tag as	string);
			string						val			= c.Text;

			try
			{
				TemplateForm			tf			= new TemplateForm(val,	type);
				tf.MdiParent						= this;
				tf.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error for open of type '" + type + "' and name '" + val + "'.\nException:" + ex.Message);
			}
		}

		// Context menus to open a TemplateForm for the linked type.
		// For each type we need one context menu. Key is typename (i.e. "DataType")
		private static Hashtable m_cmCollection;

		public void CreateLinkContextMenu(Control c, string type)
		{
			//return;
			if (m_cmCollection == null)
				m_cmCollection						= new Hashtable();

			if (! m_cmCollection.ContainsKey(type))
			{
				// Create a contextmenu (when type editable for the user)
				MenuItem				mi			= new MenuItem("Open");
				mi.DefaultItem						= true;
				mi.Index							= 0;
				mi.Click							+= new System.EventHandler(this.mnuOpenLink_Click);

				ContextMenu				cm			= new ContextMenu(new MenuItem[] {mi});
				m_cmCollection.Add(type, cm);
			}
			c.Tag									= type;
			c.ContextMenu							= (m_cmCollection[type]	as ContextMenu);
		}
		
		/// <summary>
		/// Refresh the contents for the given value of the given type.
		/// Value is 'n' element, type is typedef. Update menu's.
		/// When newname is "", it is removed from the menus.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="type"></param>
		public void PropagateChanges(XmlNode type, string oldname, string newname)
		{
			if (type == null)
				return;

			//System.Diagnostics.Debug.Assert(type.ParentNode.Name == "n", "Zou element n als parent verwachten....");
			string						typename	= type.Attributes["definition"].Value;

			if (typename != "TemplateFile" && oldname != newname)
			{
				// Update the nodes.
				TreeNode				typenode	= null;
				foreach (TreeNode tn in lstConceptList.Nodes)
					if (tn.Text == typename)
					{
						typenode					= tn;
						break;
					}
				TreeNode				theNode		= null;
				foreach (TreeNode tn in typenode.Nodes)
					if (tn.Text == oldname)
					{
						theNode						= tn;
					}
				// if not found, it must be a new item
				if (newname == "")
				{
					if (theNode != null)
						theNode.Remove();
				}
				else if (theNode == null)
				{			
					theNode							= new TreeNode(newname,	null);
					for (int i = 0; i < typenode.Nodes.Count; i++)
					{
						if (String.Compare(newname, typenode.Nodes[i].Text, true) < 0)
						{
							// juiste plek gevonden!
							typenode.Nodes.Insert(i, theNode);
							theNode					= null;	// klaar signaal
							break;
						}
					}
					if (theNode != null)
						typenode.Nodes.Add(theNode);
				}
				else // update the name
				{
					theNode.Text					= newname;
					// Get it out of the list and insert it on the proper spot
					theNode.Parent.Nodes.Remove(theNode);
					for (int i = 0; i < typenode.Nodes.Count; i++)
					{
						if (String.Compare(newname, typenode.Nodes[i].Text, true) < 0)
						{
							// juiste plek gevonden!
							typenode.Nodes.Insert(i, theNode);
							theNode					= null;	// klaar signaal
							break;
						}
					}
					if (theNode != null)
						typenode.Nodes.Add(theNode);
				}
			}

			foreach (Form f in this.MdiChildren)
			{
				TemplateForm			tf			= f	as TemplateForm;
				if (tf != null)
				{
					tf.PropagateChanges(type, oldname, newname);
				}
			}

		}

		private void miEE_Click(object sender, System.EventArgs e)
		{
			/* ExpressionEvaluationForm	eef			= new ExpressionEvaluationForm();
			eef.Owner								= this;
			eef.Show();*/
		}

		private void SortMenu(MenuItem menu)
		{
			int							itemCount	= menu.MenuItems.Count;
			MenuItem[]					itemArray	= new MenuItem[itemCount];
			string[]					itemKeys	= new String[itemCount];
			menu.MenuItems.CopyTo(itemArray, 0);
			for (int i = 0; i < itemCount; i++)
				itemKeys[i] = itemArray[i].Text;

			Array.Sort(itemKeys, itemArray);
			menu.MenuItems.Clear();
			menu.MenuItems.AddRange(itemArray);
		}

		/// <summary>
		/// Build the submenus under the Templates menu. When
		/// 'project_dir' is set, do those in the projectdirectory
		/// otherwise do those in the solution subdirectories.
		/// </summary>
		/// <param name="project_dir"></param>
		private void BuildConceptList(bool project_dir)
		{
			lstConceptList.Nodes.Clear();
            this.Update();
            Cursor = Cursors.WaitCursor;
            
            // Fill toolstrip texts on project and solution
            string project = TemplateCache.Instance().projectfile;
            string solution = TemplateCache.Instance().SolutionLocation;
            if (project == "" && TemplateCache.Instance().TemplatefilesEditable)
            {
                project = "Templates and concepts";
            }
            toolStripProject.Text = "Project: " + System.IO.Path.GetFileNameWithoutExtension(project);
            toolStripProject.ToolTipText = project;
            toolStripSolution.Text = "Solution: " + System.IO.Path.GetFileNameWithoutExtension(solution);
            toolStripSolution.ToolTipText = solution;

            string[] types = TemplateCache.Instance().GetViewableTypenamesList(project_dir, !project_dir);
            ProgressBar.Maximum = types.Length;
            ProgressBar.Value = 0;
            ProgressBar.Visible = true;

			foreach (string type in types)
            {
                ProgressBar.Value++;

        		TreeNode			n			= lstConceptList.Nodes.Add(type, type);
    			foreach (String typename in TemplateCache.Instance().GetTypenamesList(type))
				{
					TreeNode newitem = n.Nodes.Add(typename);
                    newitem.ImageIndex = 1;
                    newitem.SelectedImageIndex = 1;
                }
			}
            BuildTemplateList();
            ProgressBar.Visible = false;
            Cursor = Cursors.Default;
        }
        public void BuildTemplateList()
        {
            lstTemplateList.Nodes.Clear();

            if (!TemplateCache.Instance().TemplatefilesEditable)
            {
                if (tabWindow.TabPages.Count > 1)
                {
                    tabWindow.TabPages.RemoveAt(1);
                }
                return;
            }

            if (tabWindow.TabCount == 1)
            {
                tabWindow.TabPages.Add(tabTemplates);
            }
            /* if (todolist.GetLength(0) == 0)
            {
                tabWindow.
            } */

            foreach (XmlNode x in TemplateCache.Instance().GetTypesList("__TemplateFile"))
            {
                // Zoek op of we de 'appliesto' al in de lijst hebben staan.
                XmlNode appliestonode = x.SelectSingleNode("type/appliesto");
                TreeNode appliesto = null;
                if (appliestonode != null)
                {
                    String appliestoname = appliestonode.InnerText;
                    TreeNode[] results = lstTemplateList.Nodes.Find(appliestoname, true);
                    if (results.Length > 0)
                    {
                        appliesto = results[0];
                    }
                    else
                    {
                        appliesto = lstTemplateList.Nodes.Add(appliestoname, appliestoname);
                    }

                    TreeNode newitem = appliesto.Nodes.Add(x.Attributes["name"].Value); 
                    newitem.ImageIndex = 2;
                    newitem.SelectedImageIndex = 2;
                }
            }
            lstTemplateList.ExpandAll();
            tabTemplates.Show();
        
        }

		private void ConceptsMenuDelete(string type, string name)
		{

			foreach (TreeNode n in lstConceptList.Nodes)
			{
				if (n.Text == type)
				{
					for (int idx = 0; idx < n.Nodes.Count; idx++)
					{
						if (n.Nodes[idx].Text == name)
						{
							n.Nodes.RemoveAt(idx);
							break;
						}
					}
				}
			}
		}

		private void ConceptsMenuAdd(string type, string name)
		{
			foreach (TreeNode n in lstConceptList.Nodes)
			{
				if (n.Text == type)
				{
					TreeNode			newNode		= new TreeNode(name, 4, 4);
					bool				inserted	= false;
					for (int idx = 0; idx < n.Nodes.Count; idx++)
					{

                        if (n.Nodes[idx].Text.CompareTo(name) == 0)
                        {
                            // niet echt inserted, maar er hoeft geen item bij.
                            inserted = true;
                            break;
                        }
						if (n.Nodes[idx].Text.CompareTo(name) > 0)
						{
							n.Nodes.Insert(idx, newNode);
							inserted				= true;
							break;
						}
					}
					// if not a bigger text is found
					if (! inserted)
						n.Nodes.Add(newNode);
				}
			}
		}
		private void mnuTemplateOpen_Click(object sender, System.EventArgs e)
		{
			MenuItem					thisMenu	= (sender as MenuItem);

			string						type;
			string						val;

			if (thisMenu != null)
			{
				type								= (thisMenu.Parent as MenuItem).Text;
				val									= thisMenu.Text;
			}
			else
			{
				TreeView				n			= (sender as TreeView);
				if (n != null && n.SelectedNode != null && n.SelectedNode.Parent != null)
				{
					type							= n.SelectedNode.Parent.Text;
					val								= n.SelectedNode.Text;
				}
				else
					return;
			}
		
			foreach (Form f in this.MdiChildren)
			{
				TemplateForm			tf			= f	as TemplateForm;
				if (tf != null && tf.GetDisplayedName() == val && tf.GetDisplayedTypeName() == type)
				{
					tf.Activate();
					return;
				}
			}
			try
			{
				TemplateForm			tf			= new TemplateForm(val,	type);
				tf.MdiParent						= this;
				tf.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error for open of type '" + type + "' and name '" + val + "'.\nException:" + ex.Message);
			}
		}

		private void mnuTemplateFileOpen_Click(object sender, System.EventArgs e)
		{
			MenuItem					thisMenu	= (sender as MenuItem);
		
			string						type		= "__TemplateFile";
			string						val			= thisMenu.Text;

			try
			{
				TemplateForm			tf			= new TemplateForm(val,	type);
				tf.MdiParent						= this;
				tf.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error for open of type '" + type + "' and name '" + val + "'.\nException:" + ex.Message);
			}
		}
		private void mnuStart_NewItem(object sender, System.EventArgs e)
		{
			MenuItem					mi			= (MenuItem) sender;

			//			if (!CloseAllForms())
			//				return;

			// Type of solution stays the same.
			// Start new project/tabledefinition is same solution context.
			//			TemplateCache.Clear(false);

			openFileDialog.Multiselect				= false;
			openFileDialog.CheckFileExists			= false;
			openFileDialog.Title					= "Create new "	+ mi.Text;
			// it's a user type, and should be in projectdirectory
			string						type_dir	= System.IO.Path.Combine(TemplateCache.Instance().ProjectDirectory, mi.Text);

			// remove any ../ from the path
			type_dir								= System.IO.Path.GetFullPath(type_dir);
			if (!System.IO.Directory.Exists(type_dir))
				System.IO.Directory.CreateDirectory(type_dir);

			openFileDialog.Filter					= "Concept files (*.xmt)|*.xmt";
			openFileDialog.InitialDirectory			= type_dir + "\\";
			openFileDialog.FileName					= "";
			openFileDialog.RestoreDirectory			= true;
			
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			string						filename	= openFileDialog.FileName.Replace("'", "");
			if (System.IO.File.Exists(filename))
			{
				if (MessageBox.Show(this, "File exists! Are you sure to overwrite it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return;
			}

            if (System.IO.Path.GetDirectoryName(filename) != type_dir)
            {
                MessageBox.Show(this, "File should be created in the subdirectory '" + type_dir + "' in the solution directory!", "File location incorrect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

			// Set the new projectdirectory for the cache.
			//TemplateCache.ProjectDirectory = System.IO.Path.GetDirectoryName(filename);
			//InitializeMenusProjectDirectoryDependent();

			string						name		= System.IO.Path.GetFileNameWithoutExtension(filename);
			XmlNode						type		= null;
			XmlNode						val			= TemplateCache.Instance().AddNewTemplateFile(mi.Text, name, filename, out type);

			ConceptsMenuAdd(mi.Text, name);
			//this.PropagateChanges(val, type, "", name);

			TemplateForm				tf			= new TemplateForm(name, mi.Text);
			tf.MdiParent							= this;
			tf.Show();
		}

		/// <summary>
		/// Add an item to the right directory. Directory depends on
		/// the type. For user types use the subdirectory in the projectdirectory, for
		/// other types use their respective subdirectories.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuStart_AddNewItem(object sender, System.EventArgs e)
		{
			MenuItem					mi			= (MenuItem) sender;
			string						type		= mi.Text;

			openFileDialog.Multiselect				= false;
			openFileDialog.CheckFileExists			= false;
			openFileDialog.Title					= "Add new " + mi.Text + " to this solution";
			openFileDialog.Filter					= "Template	files (*.xmt)|*.xmt";

			string						cache_type_dir = TemplateCache.Instance().GetDirectoryForType(type);
			if (!Directory.Exists(cache_type_dir))
				Directory.CreateDirectory(cache_type_dir);

			openFileDialog.InitialDirectory			= cache_type_dir;
			while (true)
			{
				openFileDialog.FileName				= "";
				openFileDialog.InitialDirectory		= cache_type_dir;
				if (openFileDialog.ShowDialog(this) != DialogResult.OK)
					return;

				if (String.Compare(Path.GetDirectoryName(openFileDialog.FileName), cache_type_dir, true) != 0)
				{
					if (MessageBox.Show(this, "To add an item to the current solution, it can only be created in the initial given directory", "Fixed directory is necessary", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
						return;
				}
				else
					break;
			}

			string						filename	= openFileDialog.FileName;
			if (System.IO.File.Exists(filename))
			{
				if (MessageBox.Show(this, "File exists! Are you sure to overwrite it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return;
                XmlNode                 previouscontents
                                                    = TemplateCache.Instance().GetValueFor(filename);
				if (previouscontents != null)
				{
					// Get the type, and remove it completely from the cache.
                    XmlNode             previoustypedef
                                                    = TemplateCache.Instance().GetTemplateType(previouscontents.Attributes["definition"].Value);
					string				previousname= previouscontents.SelectSingleNode(previoustypedef.Attributes["nameattribute"].Value).InnerText;
					ConceptsMenuDelete(type, previousname);
					//this.PropagateChanges(previouscontents, previoustypedef, previousname, "");
				}
			}
			
			string						name		= System.IO.Path.GetFileNameWithoutExtension(filename);
			XmlNode						typedef		= null;
			XmlNode						val			= TemplateCache.Instance().AddNewTemplateFile(mi.Text, name, filename, out typedef);

			ConceptsMenuAdd(type, name);
			//this.PropagateChanges(val, typedef, "", name);

			TemplateForm				tf			= new TemplateForm(name, mi.Text);
			tf.MdiParent							= this;
			tf.Show();
		}

		//		private void mnuStart_Solution(object sender, System.EventArgs e)
		//		{
		//			MenuItem mi = (MenuItem) sender;
		//
		//			if (!CloseAllForms())
		//				return;
		//
		//			// Retrieve the solution code to be opened, given the name
		//			// of the type of solution we find in the menu text
		//			string newsolution = "";
		//			foreach (string opt_sol in TemplateUtil.Instance().ConfigSolutions())
		//				if (TemplateUtil.Instance().ConfigSolutionInfo(opt_sol, "Name") == mi.Text)
		//				{
		//					newsolution = opt_sol;
		//					break;
		//				}
		//			System.Diagnostics.Debug.Assert(newsolution != "", "Solution type not found! Can be default is selected");
		//
		//			// Clear the cache partially when its the same solution
		//			// and complete when it another type of solution
		//			TemplateCache.Clear(newsolution != TemplateCache.Solution);
		//
		//			// Treat all files to be opened. We'll have to do the same
		//			// as when initial loading with a list of filenames.
		//			InitializeApplication(new string[] { newsolution });
		//		}
		//
		/// <summary>
		/// Get a list of all mdi TemplateForm children which are dirty.
		/// </summary>
		/// <returns></returns>
		public TemplateForm[] DirtyOpenChildren()
		{
			ArrayList					l			= new ArrayList();
			foreach (Form f in this.MdiChildren)
			{
				if (! (f is TemplateForm))
					continue;
				TemplateForm			tf			= (f as	TemplateForm);
				if (tf.dirty)
					l.Add(tf);
			}
			return (l.ToArray(typeof(TemplateForm)) as TemplateForm[]);
			}

		/// <summary>
		/// Return true when one of the open templateforms is dirty.
		/// </summary>
		/// <returns></returns>
		public bool HasOpenFormsWhichNeedApply()
		{
			TemplateForm[]				dirties		= DirtyOpenChildren();
			return (dirties.GetLength(0) > 0);
		}

		/// <summary>
		/// Close all forms, and optionally save the changes ones which need apply.
		/// User can indicate to apply all, close without apply, or cancel action.
		/// </summary>
		/// <returns>true when forms where closed</returns>
		private bool CloseAllForms(bool evenActiveOne)
		{
			TemplateForm[]				dirties		= DirtyOpenChildren();
			if (dirties.GetLength(0) > 0)
			{
				switch (MessageBox.Show(this, "Apply changes to all open windows?", "Apply changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						foreach (TemplateForm tf in dirties)
							tf.Close(true, false);
						break;
					case DialogResult.No:
						foreach (TemplateForm tf in dirties)
							tf.Close(false, false);
						break;
					case DialogResult.Cancel:
						return false;
				}
			}

			Form						active		= this.ActiveMdiChild;
			foreach (Form f in this.MdiChildren)
				if (evenActiveOne || f != active)
					f.Close();

			return true;
		}

		private void mnuReloadCache_Click(object sender, System.EventArgs e)
		{
			if (!CloseAllForms(true))
				return;

			// Reinitialize menu
			TemplateCache.Reload();
			InitializeApplication(new string[0]);
		}

		private void mnuOpen_Click(object sender, System.EventArgs e)
		{
			if (!CloseAllForms(true))
				return;
			
			openFileDialog.Multiselect				= false;
			openFileDialog.CheckFileExists			= true;
			openFileDialog.Title					= "Select NextGen solution/project to open";
			openFileDialog.Filter					= "NextGen projects	(*.xmp)|*.xmp|NextGen solutions	(*.xms)|*.xms";
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			// Treat all files to be opened. We'll have to do the same
			// as when initial loading with a list of filenames.
			string[]					files		= new string[1];
			files[0] = openFileDialog.FileName;

            TemplateCache.Instance().Clear(true);
			InitializeApplication(files);
		}

		private void mnuSaveCache_Click(object sender, System.EventArgs e)
		{
#if THIERRY
			try
			{
				TemplateCache.Instance().SaveSolutionCache();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
#endif
		}

		private void mnuInvent_Click(object sender, System.EventArgs e)
		{
#if THIERRY
            System.Security.Cryptography.RNGCryptoServiceProvider 
										r			= new System.Security.Cryptography.RNGCryptoServiceProvider();
            
			byte[]						bb			= new Byte[32];
			r.GetNonZeroBytes(bb);
			BigInteger					b			= new BigInteger(bb, bb.GetLength(0));
			MessageBox.Show("Use the number: " + b.ToString(10) + "\n" 
				+ "and the string: " + System.Convert.ToBase64String(bb) + "\n"
				+ "Get them from the Clipboard.", "That's a big number!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			Clipboard.SetDataObject("number: [" + b.ToString(10) + "] string: [" + System.Convert.ToBase64String(bb) + "]", true);
#endif
		}

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			// MessageBox.Show("nog todo ;-)", "Werkt nog niet", MessageBoxButtons.AbortRetryIgnore | MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
            if (!CloseAllForms(true))
                return;

            this.Close();
		}

		private void ChangeDefinition_Click(object sender, System.EventArgs e)
		{
			/* if (!CloseAllForms(true))
				return;

			Form						f			= new ChangeDefinition();
			f.ShowDialog(this); */
		}

		private void _mnuNewProject_Click(object sender, System.EventArgs e)
		{
			if (!CloseAllForms(true))
				return;
			
			openFileDialog.Multiselect				= false;
			openFileDialog.CheckFileExists			= false;
			openFileDialog.Title					= "Create projectfile";
			openFileDialog.DefaultExt				= ".xmp";
			openFileDialog.Filter					= "NextGen projects	(*.xmp)|*.xmp";
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			// Treat all files to be opened. We'll have to do the same
			// as when initial loading with a list of filenames.
			string						file		= openFileDialog.FileName;
			XmlDocument					d			= new XmlDocument();
			d.AppendChild(d.CreateElement("project"));
			XmlElement					elm			= d.CreateElement("solution");
            elm.AppendChild(d.CreateTextNode(TemplateCache.Instance().Solution));
			d.DocumentElement.AppendChild(elm);
			elm										= d.CreateElement("solutionfilename");
            elm.AppendChild(d.CreateTextNode(TemplateCache.Instance().SolutionFilename));
			d.DocumentElement.AppendChild(elm);
			d.Save(file);
			string[]					args		= new string[1];
			args[0] = file;
			InitializeApplication(args);
		}


		private void btnOutputClose_Click(object sender, System.EventArgs e)
		{
			HideOutputText();
		}
		public void HideOutputText()
		{
			txtOutput.Visible						= false;
			splitter1.Visible						= false;
		}
		public void ShowOutputText()
		{
			splitter1.Visible						= true;
			txtOutput.Visible						= true;
		}
		/*public void StartOutput()
		{
			ShowOutputText();
		}*/

        private const string CRLF = "\x0D\x0A";

        private void btnConceptsClose_Click(object sender, System.EventArgs e)
		{
			HideConceptsList();
		}
		private void HideConceptsList()
		{
			lstConceptList.Visible					= false;
			splitter2.Visible						= false;
		}
		private void ShowConceptsList()
		{
			splitter2.Visible						= true;
			lstConceptList.Visible					= true;
		}

		private void mnuViewConcepts_Click(object sender, System.EventArgs e)
		{

			if (lstConceptList.Visible)
				HideConceptsList();
			else
				ShowConceptsList();
		}

		private void mnuViewOutput_Click(object sender, System.EventArgs e)
		{
			if (txtOutput.Visible)
				HideOutputText();
			else
				ShowOutputText();
		}

		private void mnuView_Popup(object sender, System.EventArgs e)
		{
			mnuViewConcepts.Checked					= (lstConceptList.Visible);
			mnuViewOutput.Checked					= (txtOutput.Visible);
		}

		private void mnuCloseAll_Click(object sender, System.EventArgs e)
		{
			CloseAllForms(true);
		}

		private void mnuCloseBut_Click(object sender, System.EventArgs e)
		{
			CloseAllForms(false);
		}

		private void mnuFile_Popup(object sender, System.EventArgs e)
		{
			mnuDelete.Enabled						= (this.ActiveMdiChild != null);
		}

		private void mnuDelete_Click(object sender, System.EventArgs e)
		{
			TemplateForm				f			= this.ActiveMdiChild as TemplateForm;
			if (f == null)
				return;

			string						type		= f.GetDisplayedTypeName();
			string						name		= f.GetDisplayedName();			

			XmlNode						typedef		= TemplateCache.Instance().GetValueFor(type, name);

			XmlAttribute				writable	= typedef.Attributes["writable"];
			if (writable == null || writable.Value != "true")
			{
				MessageBox.Show(String.Format("The file for {0} / {1} is not editable", type, name));
				return;
			}
			f.Close(false, false);

			TemplateCache.Instance().DeleteValue(type, name);
			ConceptsMenuDelete(type, name);
		}


		private void lstConceptList_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
//			PropertyBag bag = new PropertyBag();
//			bag.GetValue += new PropertySpecEventHandler(this.bag_GetValue);
//			bag.SetValue += new PropertySpecEventHandler(this.bag_SetValue);
//			bag.Properties.Add(new PropertySpec("Some Number", typeof(int)));
//			// ... add other properties ...
//
//			PropertyGrid p = new PropertyGrid();
//			p.SelectedObject = bag;
//			p.Show();
		}

		private void TemplateMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel								= !	CloseAllForms(true);
		}

        private void mnuImport_Click(object sender, EventArgs e)
        {
            importFileDialog.Multiselect = false;
            importFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Import Enterprise Architect export file";
            //string xsltfile = "";

            String importDir = "C:\\";
            AppSettingsReader asr = new AppSettingsReader();
            if (asr.GetValue("importdir", typeof(String)) != null)
                importDir = (String)asr.GetValue("importdir", typeof(String));

            importFileDialog.Filter = "XML files (*.xml)|*.xml";
            importFileDialog.InitialDirectory = importDir;
            importFileDialog.FileName = "";
            importFileDialog.RestoreDirectory = false;

            if (importFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            string filename = importFileDialog.FileName;

            this.Cursor = Cursors.WaitCursor;
            this.ProgressBar.Value = 0;
            this.ProgressBar.Style = ProgressBarStyle.Marquee;
            this.ProgressBar.Maximum = 100;
            this.ProgressBar.Visible = true;
            //Application.RaiseIdle(null);
            //this.ProgressBar.Value = 2;
            this.Invalidate();
            this.Update();

            string importfailedfor = ",";
            int imported = 0;
            try
            {
                System.Xml.Xsl.XslCompiledTransform xsl = new System.Xml.Xsl.XslCompiledTransform();
                XmlDocument xdoc = new XmlDocument();
                string xsltfile = "";
                if (asr.GetValue("xsltfile", typeof(String)) != null)
                    xsltfile = (String)asr.GetValue("xsltfile", typeof(String));

                if (xsltfile == "")
                {
                    // Gebruik het standaard uitgeleverde xslt bestand in de resource
                    xdoc.LoadXml(Generator.Properties.Resources.Ea2Nextgen);
                }
                else
                {
                    // Laad het bestand uit de gegeven bestandsnaam
                    xsltfile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), xsltfile);
                    xdoc.Load(xsltfile);
                }

                xsl.Load(xdoc);

                String source_xml_content = File.ReadAllText(filename);
                String result = source_xml_content;

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(result); // 
                // Value = 5;
                MemoryStream ms = new MemoryStream();

                xsl.Transform(xd, null, ms);
                this.ProgressBar.Style = ProgressBarStyle.Blocks;
                this.ProgressBar.Value = 10;
                if ((String)asr.GetValue("tempfile", typeof(String)) != null)
                {
                    string tempfile = (String)asr.GetValue("tempfile", typeof(String));
                    /* */
                    string outputdirectory = Path.GetDirectoryName(tempfile);
                    if (!Directory.Exists(outputdirectory))
                        Directory.CreateDirectory(outputdirectory);

                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    StreamWriter sw = File.CreateText(tempfile);
                    sw.WriteLine(sr.ReadToEnd());
                    sw.Close();
                    /* */
                }
                ms.Position = 0;
                XmlDocument xd2 = new XmlDocument();
                xd2.Load(ms);

                XmlNodeList results = xd2.SelectNodes("results/type[@definition!='']");
                int progression = 0;
                foreach (XmlNode element in results)
                {
                    this.ProgressBar.Value = 10 + (++progression * 90 / results.Count);
                    XmlAttribute attdef = element.Attributes["definition"];
                    if (attdef == null)
                        continue;
                    string definition = attdef.Value;
                    XmlNode namenode = element.SelectSingleNode("name");
                    if (namenode == null)
                        continue;
                    string name = namenode.InnerText;
                    // Remove odd characters from the (file)name
                    foreach (char c in Path.GetInvalidFileNameChars())
                        name.Replace(c.ToString(), "");
                    namenode.InnerText = name;

                    XmlNode newitem = null;
                    try
                    {
                        string type_dir = System.IO.Path.Combine(TemplateCache.Instance().ProjectDirectory, definition);
                        string fname = System.IO.Path.Combine(type_dir, name + ".xmt");

                        XmlNode node;
                        newitem = TemplateCache.Instance().AddNewTemplateFile(definition, name, fname, out node);
                        imported += 1;
                    }
                    catch (ArgumentException)
                    {
                        System.Diagnostics.Debug.WriteLine("Could not create file for '" + name + "'");
                        if (!importfailedfor.Contains("," + name + ","))
                        {
                            importfailedfor += name + ",";
                        }
                        continue;
                    }
                    catch (ApplicationException)
                    {
                        System.Diagnostics.Debug.WriteLine("Could not create file for '" + name + "'");
                        if (!importfailedfor.Contains("," + name + ","))
                        {
                            importfailedfor += name + ",";
                        }
                        continue;
                    }
                    //XmlDocumentFragment df = newitem.OwnerDocument.CreateDocumentFragment();
                    foreach (XmlNode n in element.ChildNodes)
                    {
                        if (n.Name != "name")
                            newitem.FirstChild.AppendChild(Kloon(newitem.OwnerDocument, n));
                    }
                    string f;
                    TemplateCache.Instance().SaveFile(out f, newitem.FirstChild);
                    ConceptsMenuAdd(definition, name);
                }
            }
            finally
            {
                this.ProgressBar.Visible = false;
                this.Cursor = Cursors.Default;
            }
            if (importfailedfor == ",")
            {
                MessageBox.Show("Successfully imported " + imported.ToString() + " concepts.", "Import result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Successfully imported " + imported.ToString() + " concepts. Import failed for: " + importfailedfor.Substring(1, importfailedfor.Length - 2), "Import result", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }

        }

        private XmlNode Kloon(XmlDocument doc, XmlNode toClone)
        {
            XmlNode newNode = doc.CreateNode(toClone.NodeType, toClone.Name, doc.NamespaceURI);
            if (toClone.Value != null)
                newNode.Value = toClone.Value;
            foreach (XmlNode n in toClone.ChildNodes)
            {
                newNode.AppendChild(Kloon(doc, n));
            }
            return newNode;
        }

        /// <summary>
        /// Verwerk een dubbelklik op een template file.
        /// Zoek uit of we wel legaal klikten, kijk daarna of we al niet een window open hebben
        /// met dat template erin, en activeer die dan. Anders: open een nieuw window
        /// om dat template te editen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstTemplateList_DoubleClick(object sender, EventArgs e)
        {
			string						val;

			TreeView				n			= (sender as TreeView);
            if (n != null && n.SelectedNode != null && n.SelectedNode.Parent != null)
            {
                val = n.SelectedNode.Text;
                OpenTemplateFileEditorFor(val, 1);
            }
        }

        private void OpenTemplateFileEditorFor(string name, int goto_linenr)
        {
            string type = "__TemplateFile";

            foreach (Form f in this.MdiChildren)
            {
                TemplateFileEditorV2 tf = f as TemplateFileEditorV2;
                if (tf != null && tf.IsDisplaying(name))
                {
                    tf.Activate();
                    tf.Gotoline(goto_linenr);
                    return;
                }
            }
            try
            {
                TemplateFileEditorV2 tf;
                // Als value niet gevonden kan worden moet het een library zijn.
                if (TemplateCache.Instance().GetValueFor(type, name) == null)
                {
                    tf = new TemplateFileEditorV2(name);
                }
                else
                {
                    tf = new TemplateFileEditorV2(name, type);
                }
                tf.MdiParent = this;
                tf.Show();
                tf.Gotoline(goto_linenr);
                tf.AddObserver(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during start of editor on '" + name + "'.\nException:" + ex.Message);
            }

        }
        private void mnuOptions_Click(object sender, EventArgs e)
        {
            Options o = new Options();
            o.ShowDialog();
        }

        private void mnuRegistration_Click(object sender, EventArgs e)
        {
            Welcome w = new Welcome(false);
            w.ShowDialog();
        }

        private void TemplateMain_Shown(object sender, EventArgs e)
        {
            if (!firstload)
            {
                return;
            }

            firstload = false;

            NewOpenProject nop = new NewOpenProject();
            nop.ShowDialog(this);
        }

        private void mnuNewProject_Click_1(object sender, EventArgs e)
        {
            if (!CloseAllForms(true))
                return;

            NewOpenProject nop = new NewOpenProject(true);
            nop.ShowDialog(this);
        }

        private void mnuOpenProject_Click(object sender, EventArgs e)
        {
            if (!CloseAllForms(true))
                return;
            
            NewOpenProject nop = new NewOpenProject(false);
            nop.ShowDialog(this);
        }

        private static int __linenrtofind;
        private static bool IsOnLine(LinkInfo li)
        {
            return (li._outputlinenr == __linenrtofind);
        }

        /// <summary>
        /// Haal de LinkInfo op van het gegeven punt (onder de muis)
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private LinkInfo GetMouseLinkInfo(Point pt)
        {
            if (linkinfo == null)
                return null;

            int idx = txtOutput.GetCharIndexFromPosition(pt);
            int outputlinenr = txtOutput.GetLineFromCharIndex(idx);
            int linestartcharidx = txtOutput.GetFirstCharIndexFromLine(outputlinenr);

            __linenrtofind = outputlinenr;
            LinkInfo result = linkinfo.Find(new Predicate<LinkInfo>(IsOnLine));
            if (result == null ||
                idx - linestartcharidx < result._position ||
                idx - linestartcharidx > result._position + result._length)
            {
                result = null;
            }

            return result;
        }
        /// <summary>
        /// Check de positie onder de muis en zet hem de cursor op 'hand'
        /// als het een link is.
        /// </summary>
        /// <param name="pt"></param>
        private void UpdateCursorIcon(Point pt)
        {
            if (GetMouseLinkInfo(pt) == null)
            {
                txtOutput.Cursor = Cursors.Default;
            }
            else
            {
                txtOutput.Cursor = Cursors.Hand;               
            }
        }
        private void txtOutput_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateCursorIcon(e.Location);
        }

        private void txtOutput_MouseClick(object sender, MouseEventArgs e)
        {
            LinkInfo li = GetMouseLinkInfo(e.Location);
            if (li == null)
                return;

            if (li._linkinfotype == LinkInfoType.Outputfile)
            {
                this.Cursor = Cursors.WaitCursor;
                Utility.AppStarter.BrowseTo(li._filename, 5000);
                this.Cursor = Cursors.Default;
            }
            else if (li._linkinfotype == LinkInfoType.Templatefile)
            {
                
                OpenTemplateFileEditorFor(Path.GetFileName(li._filename), li._linenr);
            }
        }

        private void txtOutput_TextChanged(object sender, EventArgs e)
        {
            UpdateCursorIcon(Cursor.HotSpot);
        }

        private System.Collections.Generic.List<LinkInfo> linkinfo;
        /// <summary>
        /// Todo: nog ff notitie maken van bestandsnaam en regelnummer in dat bestand.
        /// </summary>
        /// <param name="firstpart"></param>
        /// <param name="linkeditem"></param>
        /// <param name="lastpart"></param>
        /// <param name="filename"></param>
        /// <param name="linenr"></param>
        public void AddToOutputText(string firstpart, string linkeditem, string lastpart, LinkInfoType linktype, SourceCodeContext scc)
        {
            if (this.txtOutput.InvokeRequired)
            {
                AddToOutputTextCallback d = new AddToOutputTextCallback(AddToOutputText);
                this.Invoke(d, new object[] { firstpart, linkeditem, lastpart, linktype, scc });
            }
            else
            {
                if (linkeditem == null && scc != null && scc.Filename != null && scc.Linenr > 0)
                {
                    // We willen een link forceren met het regelnummer, achter de tekst.
                    linktype = LinkInfoType.Templatefile;
                    firstpart = firstpart + " (";
                    linkeditem = "line " + scc.Linenr.ToString();
                    lastpart = ")";
                }
                lock (txtOutput)
                {
                    if (firstpart != null)
                    {
                        txtOutput.AppendText(firstpart);
                    }
                    if (linkeditem != null)
                    {
                        Color prev_color = txtOutput.ForeColor;
                        txtOutput.SelectionStart = txtOutput.TextLength;
                        txtOutput.SelectionColor = Color.DarkBlue;
                        txtOutput.SelectionFont = new Font(txtOutput.Font, FontStyle.Underline);
                        txtOutput.SelectedText = linkeditem;
                        txtOutput.SelectionStart = txtOutput.TextLength;
                        txtOutput.SelectionColor = prev_color;
                        txtOutput.SelectionFont = txtOutput.Font;
                        
                        if (linkinfo == null)
                            linkinfo = new System.Collections.Generic.List<LinkInfo>();
                        // Onze tekst staat op de huidige lengte - 1
                        string linkname = scc.Conceptname;
                        if (linkname == string.Empty)
                            linkname = scc.Filename;
                        System.Diagnostics.Debug.Assert(linkname != string.Empty);
                        linkinfo.Add(new LinkInfo(linkname, linktype, scc.Linenr, firstpart.Length, linkeditem.Length, txtOutput.Lines.Length - 1));

                        // Als we geen lastpart hebben, forceren we er een spatie in, om de link
                        // niet verder actief te laten zijn dan die spatie.
                        if (lastpart == null || lastpart == "")
                            lastpart = " ";
                    }
                    if (lastpart != null)
                    {
                        txtOutput.AppendText(lastpart);
                    }
                    txtOutput.AppendText(CRLF);
                    txtOutput.ScrollToCaret();
                }
            }
        }

        public void Generate(List<GenerationRequest> todo)
        {
            ShowOutputText();

            GenerateOutput.Instance().AddGenerationRequests(todo);
            GenerateOutput.Instance().AddObserver(this);

            if (! GenerateOutput.Instance().IsRunning)
                GenerateOutput.Instance().GO();
        }
        #region Observer pattern: IObserver (observeert GenerateOutput dingetjes)
        public void ProcessUpdate(object o)
        {
            if (! (o is GenerateOutput))
            {
                return;
            }
            // Moet haast iets in de status van het genereren zijn. 
            // Werk het statusveldje bij.
            GenerateOutput go = o as GenerateOutput;
            System.Threading.Thread thread = go.thread;
            if (go.FilesToDo() == 0 ||
                thread == null)
            {
                this.GenerationStatus.Text = "Ready";
                this.GenerationStatus.Visible = false;
                return;
            }

            // We zijn nog niet helemaal klaar.
            GenerationStatus.Visible = true;
            GenerationStatus.Text = "Generating... " + go.FilesToDo().ToString() + " to go";
        }
        /// <summary>
        /// Als we een error doorkrijgen, zet die dan met regelnummer en file op de output
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <param name="t"></param>
        /// <param name="templatename"></param>
        /// <param name="linenr"></param>
        public void ProcessUpdate(object o, string msg, ObserverPattern.NotificationType t, SourceCodeContext scc)
        {
            if (t == Generator.ObserverPattern.NotificationType.Erreur)
                AddToOutputText(msg, null, null, LinkInfoType.Templatefile, scc);
            else
                AddToOutputText(msg, null, null, LinkInfoType.Templatefile, null);
        }
        public void ProcessUpdate(object o, string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            // we moeten volgens mij zonder linenr kunnen als het om output gaat.
            System.Diagnostics.Debug.Assert(linenr == 0);

            // Dit moet om een gewone uitvoer gaan.
            AddToOutputText(firstpart, linkeditem, lastpart, LinkInfoType.Outputfile, new SourceCodeContext("", filename, linenr));
        }

        #endregion

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            TemplateForm f = this.ActiveMdiChild as TemplateForm;
            if (f == null)
                return;
             
            string type = f.GetDisplayedTypeName();
            string name = f.GetDisplayedName();

            string cache_type_dir = TemplateCache.Instance().GetDirectoryForType(type);
            name = "Copy of " + name;
            XmlNode typedef = null;
            string filename = Path.Combine(cache_type_dir, name + ".xmt");
            XmlNode val = TemplateCache.Instance().AddNewTemplateFile(type, name, filename, out typedef);

            XmlDocument copydoc = f.GetCleanDocument();


            string nameattribute                    = typedef.Attributes["nameattribute"].Value;

            //val.FirstChild.AppendChild(val.OwnerDocument.ImportNode(copydoc.DocumentElement.FirstChild, true));
            foreach (XmlNode xn in copydoc.DocumentElement.ChildNodes)
            {
                if (xn.Name != nameattribute)
                    val.FirstChild.AppendChild(val.OwnerDocument.ImportNode(xn, true));
            }
            string newfilename = "";
            string newname = "";
            string oldname = "";
            TemplateCache.Instance().RefreshValue(out newfilename, val.FirstChild, out oldname, out newname);

            ConceptsMenuAdd(type, name);
            //this.PropagateChanges(val, typedef, "", name);

            TemplateForm tf = new TemplateForm(name, type);
            tf.MdiParent = this;
            tf.Show();

        }
    }
    public enum LinkInfoType
    {
        Templatefile,
        Outputfile
    }
    public class LinkInfo
    {
        public string _filename;
        public  LinkInfoType _linkinfotype;
        public int _linenr;
        public int _position;
        public int _length;
        public int _outputlinenr;
             
        public LinkInfo(string filename, LinkInfoType linkinfotype, int linenr, int position, int length, int outputlinenr)
        {
            _filename = filename;
            _linkinfotype = linkinfotype;
            _linenr = linenr;
            _position = position;
            _length = length;
            _outputlinenr = outputlinenr;
        }
    }
	/// <summary>
	/// Kernel functions
	/// </summary>
	public class Win32
	{
              //       Private Declare Function ShellExecute Lib "shell32.dll" Alias _
      //"ShellExecuteA" (ByVal hwnd As Long, ByVal lpszOp As _
      //String, ByVal lpszFile As String, ByVal lpszParams As String, _
      //ByVal lpszDir As String, ByVal FsShowCmd As Long) As Long

      //Private Declare Function GetDesktopWindow Lib "user32" () As Long

      //Const SW_SHOWNORMAL = 1

      //Const SE_ERR_FNF = 2&
      //Const SE_ERR_PNF = 3&
      //Const SE_ERR_ACCESSDENIED = 5&
      //Const SE_ERR_OOM = 8&
      //Const SE_ERR_DLLNOTFOUND = 32&
      //Const SE_ERR_SHARE = 26&
      //Const SE_ERR_ASSOCINCOMPLETE = 27&
      //Const SE_ERR_DDETIMEOUT = 28&
      //Const SE_ERR_DDEFAIL = 29&
      //Const SE_ERR_DDEBUSY = 30&
      //Const SE_ERR_NOASSOC = 31&
      //Const ERROR_BAD_FORMAT = 11&

      //Function OpenDocument(ByVal DocName As String) As Long
      //    Dim Scr_hDC As Long
      //    'Scr_hDC = GetDesktopWindow()
      //    OpenDocument = ShellExecute(Me.hwnd, "Open", DocName, _
      //    "", "C:\", SW_SHOWNORMAL)
        [DllImport("shell32.dll")]
        public static extern int ShellExecute(int hwnd, string option, string filename, string param, string dir, int showcmd);

		/// <summary>
		/// Allocate console window
		/// </summary>
		[DllImport("kernel32.dll")]
		public static extern Boolean AllocConsole();
		/// <summary>
		/// Release console window
		/// </summary>
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeConsole();
		/// <summary>
		/// Attach existing console window
		/// </summary>
		[DllImport("kernel32.dll")]
		public static extern Boolean AttachConsole(int dwProcessId);

		private static bool _hasConsole;
		private static bool _hasConsoleChecked;
		public static bool HasConsole()
		{
			if (! _hasConsoleChecked)
			{
				_hasConsole							= CheckConsole();
				_hasConsoleChecked					= true;
			}
			/*if (_hasConsole)
				MessageBox.Show("HasConsole: true");
			else
				MessageBox.Show("HasConsole: false");*/
			return _hasConsole;
		}
		private static bool CheckConsole()
		{
			bool						check		= false;
			try
			{
				check								= AttachConsole(-1);
			}
			catch(Exception ex)
			{
				MessageBox.Show("EXCEPTION: " + ex.Message);
				return false;
			}
			/*if (check)
				MessageBox.Show("CheckConsole: true");
			else
				MessageBox.Show("CheckConsole: false");*/

			return check;
		}
		public static void StartConsole()
		{
			/*MessageBox.Show("StartConsole");*/
			if (! CheckConsole())
				AllocConsole();
		}
	}
}
