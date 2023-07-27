/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) 2006  Thierry Wiersma													*
*****************************************************************************************/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using Generator.Utility;
using System.Diagnostics;


namespace Generator
{
	/// <summary>
	/// Summary description for TemplateForm.
	/// </summary>
	public class TemplateForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button 
										btnOK;
        private System.Windows.Forms.Button 
										btnCancel;
        private System.Windows.Forms.TabControl 
										tabControl1;
        private System.Windows.Forms.TabPage 
										tabGeneral;
        private System.Windows.Forms.TabPage 
										tabAttributes;
        private System.Windows.Forms.TabPage 
										tabSets;
        private System.Windows.Forms.Panel 
										pnlGeneral;
        private System.Windows.Forms.Panel 
										pnlAttributes;
        private System.Windows.Forms.Panel 
										pnlSets;
        private System.Windows.Forms.Panel 
										pnlAList;
        private System.Windows.Forms.VScrollBar 
										attrVScroll;
        private System.Windows.Forms.HScrollBar 
										attrHScroll;
        private System.Windows.Forms.HScrollBar 
										setHScrollSets;
        private System.Windows.Forms.VScrollBar 
										setsVScrollSets;
        private System.Windows.Forms.Panel 
										pnlSetAttributes;
        private System.Windows.Forms.VScrollBar 
										setsVScrollAttributes;
        private System.Windows.Forms.Panel 
										pnlSList;
        private System.Windows.Forms.TabPage 
										tabGenerator;
        private System.Windows.Forms.HScrollBar 
										genHScroll;
        private System.Windows.Forms.VScrollBar 
										genVScroll;
        private System.Windows.Forms.Panel 
										pnlGenerator;
        private System.Windows.Forms.Panel 
										pnlGenList;
        private System.Windows.Forms.ToolTip 
										tt;
        private System.Windows.Forms.VScrollBar 
										gVScroll;
        private System.Windows.Forms.Button 
										btnGenerate;
        private System.Windows.Forms.Button 
										btnLeftLeft;
        private System.Windows.Forms.Button 
										btnReloadCache;
        private System.Windows.Forms.Button 
										btnInventLabels;
        private System.Windows.Forms.Panel 
										pnlGenTables;
        private System.Windows.Forms.CheckedListBox 
										lstGenTableDefinitions;
        private System.Windows.Forms.Button 
										btnGenAll;
        private System.Windows.Forms.Button 
										btnGenNone;
        private System.Windows.Forms.Button 
										btnGenSwap;
        private System.Windows.Forms.Button 
										btnApply;
        private System.Windows.Forms.MainMenu 
										mainMenu;
        private System.Windows.Forms.MenuItem 
										menuItem1;
        private System.Windows.Forms.MenuItem 
										menuItem4;
        private System.ComponentModel.IContainer 
										components;

        private System.Windows.Forms.TabPage 
										tabTemplate;
        private System.Windows.Forms.HScrollBar 
										hscrOverview;
        private System.Windows.Forms.VScrollBar 
										vscrOverview;
        private System.Windows.Forms.Panel 
										pnlOverviewMain;
        private System.Windows.Forms.Panel 
										pnlOverviewParts;
        private System.Windows.Forms.Panel 
										pnlDetailMain;
        private System.Windows.Forms.Panel 
										pnlDetailParts;
        private System.Windows.Forms.VScrollBar 
										vscrDetail;
        private System.Windows.Forms.Button 
										button1;
        private System.Windows.Forms.Button 
										button2;
		private	string					m_displayedtype;

		public TemplateForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public TemplateForm(string name, string type)
		{
			InitializeComponent();
			RepositionControls();

			m_currenttype							= TemplateCache.Instance().GetValueFor(type, name).CloneNode(true);
			m_type_definition						= TemplateCache.Instance().GetTemplateType(type);
			m_filename								= m_currenttype.Attributes["sourcefile"].Value;
			m_displayedtype							= type;
		}

		public string GetDisplayedTypeName()
		{
			return m_displayedtype;
		}
		public string GetDisplayedName()
		{
			string						nameatt		= m_type_definition.Attributes["nameattribute"].Value;
			string						name		= m_currenttype.SelectSingleNode(nameatt).InnerText.Trim();

			return name;
		}

		public TemplateForm(string filename)
		{
			InitializeComponent();
			RepositionControls();

			m_filename								= filename;
			m_currenttype							= null;
			try
			{
				m_currenttype						= TemplateCache.Instance().GetFile(m_filename, out m_type_definition).CloneNode(true);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not load file.\n(file=" + m_filename + ")\n(exception=" + ex.Message + ")");
			}
		}

		private void RepositionControls()
		{
			this.tabGeneral.BorderStyle				= BorderStyle.None;
			this.pnlGeneral.BorderStyle				= BorderStyle.None;
			this.pnlGeneral.Top						= 0;
			this.pnlGeneral.Left					= 0;
			this.pnlGeneral.Size					= new System.Drawing.Size(this.tabGeneral.Width,this.tabGeneral.Height);

			// Sets
			this.tabSets.BorderStyle				= BorderStyle.None;
			this.pnlSets.BorderStyle				= BorderStyle.None;
			this.pnlSets.Top						= 0;
			this.pnlSets.Left						= 0;
			this.pnlSets.Size						= new Size(this.setsVScrollSets.Left,this.setHScrollSets.Top);
			this.pnlSList.BorderStyle				= BorderStyle.None;
			this.pnlSList.Top						= 0;
			this.pnlSList.Left						= 0;

			this.pnlSetAttributes.BorderStyle		= BorderStyle.None;
			this.pnlSetAttributes.Top				= 0;
			this.pnlSetAttributes.Left				= this.setsVScrollSets.Left	+ this.setsVScrollSets.Width;
			this.pnlSetAttributes.Width				= this.setsVScrollAttributes.Left -	this.pnlSetAttributes.Left;

			// Attributes
			this.tabAttributes.BorderStyle			= BorderStyle.None;
			this.pnlAttributes.BorderStyle			= BorderStyle.None;
			this.pnlAttributes.Left					= 0;
			this.pnlAttributes.Top					= 0;
			this.pnlAttributes.Size					= new Size(this.attrVScroll.Left, this.attrHScroll.Top);
			this.pnlAList.BorderStyle				= BorderStyle.None;
			this.pnlAList.Top						= 0;
			this.pnlAList.Left						= 0;

			// Generation panel
			this.tabGenerator.BorderStyle			= BorderStyle.None;
			this.pnlGenerator.BorderStyle			= BorderStyle.None;
			this.pnlGenerator.Left					= 0;
			this.pnlGenerator.Top					= 0;
//			this.btnGenerate.Top = genHScroll.Top - this.btnGenerate.Height - 2;
//			this.btnGenerate.Left = genVScroll.Left - this.btnGenerate.Width - 2;
//			this.pnlGenerator.Size = new Size(this.btnGenerate.Left, this.btnGenerate.Top);
			this.pnlGenerator.Size					= new Size(this.genVScroll.Left, this.genHScroll.Top);
			this.btnGenerate.Top					= pnlGenerator.Height -	btnGenerate.Height;	
			this.btnGenerate.Left					= pnlGenerator.Width - this.btnGenerate.Width;
			this.pnlGenList.BorderStyle				= BorderStyle.None;
			this.pnlGenList.Top						= 0;
			this.pnlGenList.Left					= 0;
			this.pnlGenList.Width					= pnlGenerator.Width;
			this.pnlGenList.Height					= pnlGenerator.Height;

/*			this.progress.Visible = false;
			this.progress.Top						= 0;
			this.progress.Left						= pnlGenerator.Width - progress.Width;
			this.progress.Height					= pnlGenerator.Height;

*/
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.components							= new System.ComponentModel.Container();
			this.btnOK								= new System.Windows.Forms.Button();
			this.btnCancel							= new System.Windows.Forms.Button();
			this.tabControl1						= new System.Windows.Forms.TabControl();
			this.tabGeneral							= new System.Windows.Forms.TabPage();
			this.gVScroll							= new System.Windows.Forms.VScrollBar();
			this.pnlGeneral							= new System.Windows.Forms.Panel();
			this.tabAttributes						= new System.Windows.Forms.TabPage();
			this.attrHScroll						= new System.Windows.Forms.HScrollBar();
			this.attrVScroll						= new System.Windows.Forms.VScrollBar();
			this.pnlAttributes						= new System.Windows.Forms.Panel();
			this.pnlAList							= new System.Windows.Forms.Panel();
			this.tabSets							= new System.Windows.Forms.TabPage();
			this.setsVScrollAttributes				= new System.Windows.Forms.VScrollBar();
			this.pnlSetAttributes					= new System.Windows.Forms.Panel();
			this.setsVScrollSets					= new System.Windows.Forms.VScrollBar();
			this.setHScrollSets						= new System.Windows.Forms.HScrollBar();
			this.pnlSets							= new System.Windows.Forms.Panel();
			this.pnlSList							= new System.Windows.Forms.Panel();
			this.tabGenerator						= new System.Windows.Forms.TabPage();
			this.pnlGenTables						= new System.Windows.Forms.Panel();
			this.btnGenSwap							= new System.Windows.Forms.Button();
			this.btnGenNone							= new System.Windows.Forms.Button();
			this.btnGenAll							= new System.Windows.Forms.Button();
			this.lstGenTableDefinitions				= new System.Windows.Forms.CheckedListBox();
			this.genHScroll							= new System.Windows.Forms.HScrollBar();
			this.genVScroll							= new System.Windows.Forms.VScrollBar();
			this.pnlGenerator						= new System.Windows.Forms.Panel();
			this.btnGenerate						= new System.Windows.Forms.Button();
			this.pnlGenList							= new System.Windows.Forms.Panel();
			this.tabTemplate						= new System.Windows.Forms.TabPage();
			this.vscrDetail							= new System.Windows.Forms.VScrollBar();
			this.pnlDetailMain						= new System.Windows.Forms.Panel();
			this.pnlDetailParts						= new System.Windows.Forms.Panel();
			this.hscrOverview						= new System.Windows.Forms.HScrollBar();
			this.vscrOverview						= new System.Windows.Forms.VScrollBar();
			this.pnlOverviewMain					= new System.Windows.Forms.Panel();
			this.pnlOverviewParts					= new System.Windows.Forms.Panel();
			this.tt									= new System.Windows.Forms.ToolTip(this.components);
			this.btnReloadCache						= new System.Windows.Forms.Button();
			this.btnLeftLeft						= new System.Windows.Forms.Button();
			this.btnInventLabels					= new System.Windows.Forms.Button();
			this.button1							= new System.Windows.Forms.Button();
			this.button2							= new System.Windows.Forms.Button();
			this.btnApply							= new System.Windows.Forms.Button();
			this.mainMenu							= new System.Windows.Forms.MainMenu();
			this.menuItem1							= new System.Windows.Forms.MenuItem();
			this.menuItem4							= new System.Windows.Forms.MenuItem();
			this.tabControl1.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabAttributes.SuspendLayout();
			this.pnlAttributes.SuspendLayout();
			this.tabSets.SuspendLayout();
			this.pnlSets.SuspendLayout();
			this.tabGenerator.SuspendLayout();
			this.pnlGenTables.SuspendLayout();
			this.pnlGenerator.SuspendLayout();
			this.tabTemplate.SuspendLayout();
			this.pnlDetailMain.SuspendLayout();
			this.pnlOverviewMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor						= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.btnOK.DialogResult					= System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location						= new System.Drawing.Point(360,	361);
			this.btnOK.Name							= "btnOK";
			this.btnOK.Size							= new System.Drawing.Size(72, 24);
			this.btnOK.TabIndex						= 1;
			this.btnOK.Text							= "OK";
			this.btnOK.Click						+= new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor					= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.btnCancel.CausesValidation			= false;
			this.btnCancel.DialogResult				= System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location					= new System.Drawing.Point(440,	361);
			this.btnCancel.Name						= "btnCancel";
			this.btnCancel.Size						= new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex					= 2;
			this.btnCancel.Text						= "Cancel";
			this.btnCancel.Click					+= new System.EventHandler(this.btnCancel_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tabGeneral,
																					  this.tabAttributes,
																					  this.tabSets,
																					  this.tabGenerator,
																					  this.tabTemplate});
			this.tabControl1.Name					= "tabControl1";
			this.tabControl1.SelectedIndex			= 0;
			this.tabControl1.Size					= new System.Drawing.Size(600, 353);
			this.tabControl1.TabIndex				= 4;
			this.tabControl1.SelectedIndexChanged	+= new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabGeneral
			// 
			this.tabGeneral.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabGeneral.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.gVScroll,
																					 this.pnlGeneral});
			this.tabGeneral.Location				= new System.Drawing.Point(4, 22);
			this.tabGeneral.Name					= "tabGeneral";
			this.tabGeneral.Size					= new System.Drawing.Size(592, 327);
			this.tabGeneral.TabIndex				= 0;
			this.tabGeneral.Text					= "General";
			this.tabGeneral.Resize					+= new System.EventHandler(this.tabGeneral_Resize);
			// 
			// gVScroll
			// 
			this.gVScroll.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.gVScroll.LargeChange				= 500;
			this.gVScroll.Location					= new System.Drawing.Point(576,	0);
			this.gVScroll.Maximum					= 1200;
			this.gVScroll.Name						= "gVScroll";
			this.gVScroll.Size						= new System.Drawing.Size(16, 325);
			this.gVScroll.TabIndex					= 5;
			this.gVScroll.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.gVScroll_Scroll);
			// 
			// pnlGeneral
			// 
			this.pnlGeneral.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlGeneral.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlGeneral.Location				= new System.Drawing.Point(32, 32);
			this.pnlGeneral.Name					= "pnlGeneral";
			this.pnlGeneral.Size					= new System.Drawing.Size(416, 120);
			this.pnlGeneral.TabIndex				= 4;
			this.pnlGeneral.QueryContinueDrag		+= new System.Windows.Forms.QueryContinueDragEventHandler(this.pnlGeneral_QueryContinueDrag);
			// 
			// tabAttributes
			// 
			this.tabAttributes.BackColor			= System.Drawing.SystemColors.Control;
			this.tabAttributes.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabAttributes.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.attrHScroll,
																						this.attrVScroll,
																						this.pnlAttributes});
			this.tabAttributes.Location				= new System.Drawing.Point(4, 22);
			this.tabAttributes.Name					= "tabAttributes";
			this.tabAttributes.Size					= new System.Drawing.Size(592, 327);
			this.tabAttributes.TabIndex				= 1;
			this.tabAttributes.Text					= "Attributes";
			// 
			// attrHScroll
			// 
			this.attrHScroll.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.attrHScroll.Location				= new System.Drawing.Point(0, 310);
			this.attrHScroll.Name					= "attrHScroll";
			this.attrHScroll.Size					= new System.Drawing.Size(574, 16);
			this.attrHScroll.TabIndex				= 6;
			this.attrHScroll.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.attrHScroll_Scroll);
			// 
			// attrVScroll
			// 
			this.attrVScroll.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.attrVScroll.Location				= new System.Drawing.Point(574,	0);
			this.attrVScroll.Name					= "attrVScroll";
			this.attrVScroll.Size					= new System.Drawing.Size(16, 310);
			this.attrVScroll.TabIndex				= 2;
			this.attrVScroll.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.attrVScroll_Scroll);
			// 
			// pnlAttributes
			// 
			this.pnlAttributes.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlAttributes.BackColor			= System.Drawing.SystemColors.Control;
			this.pnlAttributes.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlAttributes.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.pnlAList});
			this.pnlAttributes.Location				= new System.Drawing.Point(8, 8);
			this.pnlAttributes.Name					= "pnlAttributes";
			this.pnlAttributes.Size					= new System.Drawing.Size(550, 295);
			this.pnlAttributes.TabIndex				= 0;
			this.pnlAttributes.Resize				+= new System.EventHandler(this.pnlAttributes_Resize);
			// 
			// pnlAList
			// 
			this.pnlAList.Anchor					= System.Windows.Forms.AnchorStyles.Left;
			this.pnlAList.BackColor					= System.Drawing.SystemColors.Control;
			this.pnlAList.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlAList.ForeColor					= System.Drawing.SystemColors.ControlText;
			this.pnlAList.Location					= new System.Drawing.Point(0, 16);
			this.pnlAList.Name						= "pnlAList";
			this.pnlAList.Size						= new System.Drawing.Size(388, 265);
			this.pnlAList.TabIndex					= 6;
			// 
			// tabSets
			// 
			this.tabSets.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabSets.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.setsVScrollAttributes,
																				  this.pnlSetAttributes,
																				  this.setsVScrollSets,
																				  this.setHScrollSets,
																				  this.pnlSets});
			this.tabSets.Location					= new System.Drawing.Point(4, 22);
			this.tabSets.Name						= "tabSets";
			this.tabSets.Size						= new System.Drawing.Size(592, 327);
			this.tabSets.TabIndex					= 2;
			this.tabSets.Text						= "Sets";
			this.tabSets.Enter						+= new System.EventHandler(this.tabSets_Enter);
			// 
			// setsVScrollAttributes
			// 
			this.setsVScrollAttributes.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.setsVScrollAttributes.Location		= new System.Drawing.Point(574,	0);
			this.setsVScrollAttributes.Name			= "setsVScrollAttributes";
			this.setsVScrollAttributes.Size			= new System.Drawing.Size(16, 326);
			this.setsVScrollAttributes.TabIndex		= 10;
			// 
			// pnlSetAttributes
			// 
			this.pnlSetAttributes.Anchor			= (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.pnlSetAttributes.BorderStyle		= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSetAttributes.Location			= new System.Drawing.Point(460,	5);
			this.pnlSetAttributes.Name				= "pnlSetAttributes";
			this.pnlSetAttributes.Size				= new System.Drawing.Size(100, 200);
			this.pnlSetAttributes.TabIndex			= 9;
			// 
			// setsVScrollSets
			// 
			this.setsVScrollSets.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.setsVScrollSets.Location			= new System.Drawing.Point(438,	0);
			this.setsVScrollSets.Name				= "setsVScrollSets";
			this.setsVScrollSets.Size				= new System.Drawing.Size(16, 310);
			this.setsVScrollSets.TabIndex			= 8;
			this.setsVScrollSets.Scroll				+= new System.Windows.Forms.ScrollEventHandler(this.setsVScrollSets_Scroll);
			// 
			// setHScrollSets
			// 
			this.setHScrollSets.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.setHScrollSets.Location			= new System.Drawing.Point(0, 310);
			this.setHScrollSets.Name				= "setHScrollSets";
			this.setHScrollSets.Size				= new System.Drawing.Size(438, 16);
			this.setHScrollSets.TabIndex			= 7;
			this.setHScrollSets.Scroll				+= new System.Windows.Forms.ScrollEventHandler(this.setHScrollSets_Scroll);
			// 
			// pnlSets
			// 
			this.pnlSets.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlSets.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSets.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.pnlSList});
			this.pnlSets.Location					= new System.Drawing.Point(8, 8);
			this.pnlSets.Name						= "pnlSets";
			this.pnlSets.Size						= new System.Drawing.Size(408, 273);
			this.pnlSets.TabIndex					= 1;
			this.pnlSets.Resize						+= new System.EventHandler(this.pnlSets_Resize);
			// 
			// pnlSList
			// 
			this.pnlSList.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSList.Location					= new System.Drawing.Point(56, 16);
			this.pnlSList.Name						= "pnlSList";
			this.pnlSList.Size						= new System.Drawing.Size(96, 160);
			this.pnlSList.TabIndex					= 0;
			// 
			// tabGenerator
			// 
			this.tabGenerator.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabGenerator.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.pnlGenTables,
																					   this.genHScroll,
																					   this.genVScroll,
																					   this.pnlGenerator});
			this.tabGenerator.Location				= new System.Drawing.Point(4, 22);
			this.tabGenerator.Name					= "tabGenerator";
			this.tabGenerator.Size					= new System.Drawing.Size(592, 327);
			this.tabGenerator.TabIndex				= 3;
			this.tabGenerator.Text					= "Generate";
			this.tabGenerator.Click					+= new System.EventHandler(this.tabGenerator_Click);
			// 
			// pnlGenTables
			// 
			this.pnlGenTables.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.pnlGenTables.BorderStyle			= System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlGenTables.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.btnGenSwap,
																					   this.btnGenNone,
																					   this.btnGenAll,
																					   this.lstGenTableDefinitions});
			this.pnlGenTables.Name					= "pnlGenTables";
			this.pnlGenTables.Size					= new System.Drawing.Size(148, 305);
			this.pnlGenTables.TabIndex				= 12;
			// 
			// btnGenSwap
			// 
			this.btnGenSwap.ImageAlign				= System.Drawing.ContentAlignment.MiddleLeft;
			this.btnGenSwap.Location				= new System.Drawing.Point(96, 0);
			this.btnGenSwap.Name					= "btnGenSwap";
			this.btnGenSwap.Size					= new System.Drawing.Size(48, 20);
			this.btnGenSwap.TabIndex				= 14;
			this.btnGenSwap.Text					= "&Swap";
			this.btnGenSwap.Click					+= new System.EventHandler(this.btnGenSwap_Click);
			// 
			// btnGenNone
			// 
			this.btnGenNone.ImageAlign				= System.Drawing.ContentAlignment.MiddleLeft;
			this.btnGenNone.Location				= new System.Drawing.Point(48, 0);
			this.btnGenNone.Name					= "btnGenNone";
			this.btnGenNone.Size					= new System.Drawing.Size(48, 20);
			this.btnGenNone.TabIndex				= 13;
			this.btnGenNone.Text					= "&None";
			this.btnGenNone.Click					+= new System.EventHandler(this.btnGenNone_Click);
			// 
			// btnGenAll
			// 
			this.btnGenAll.Name						= "btnGenAll";
			this.btnGenAll.Size						= new System.Drawing.Size(48, 20);
			this.btnGenAll.TabIndex					= 12;
			this.btnGenAll.Text						= "&All";
			this.btnGenAll.Click					+= new System.EventHandler(this.btnGenAll_Click);
			// 
			// lstGenTableDefinitions
			// 
			this.lstGenTableDefinitions.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lstGenTableDefinitions.BorderStyle	= System.Windows.Forms.BorderStyle.None;
			this.lstGenTableDefinitions.CheckOnClick= true;
            this.lstGenTableDefinitions.IntegralHeight
                                                    = false;
			this.lstGenTableDefinitions.Location	= new System.Drawing.Point(0, 24);
			this.lstGenTableDefinitions.Name		= "lstGenTableDefinitions";
			this.lstGenTableDefinitions.Size		= new System.Drawing.Size(144, 281);
			this.lstGenTableDefinitions.Sorted		= true;
			this.lstGenTableDefinitions.TabIndex	= 11;
			this.lstGenTableDefinitions.MouseDown	+= new System.Windows.Forms.MouseEventHandler(this.lstGenTableDefinitions_MouseDown);
			// 
			// genHScroll
			// 
			this.genHScroll.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.genHScroll.Location				= new System.Drawing.Point(0, 308);
			this.genHScroll.Name					= "genHScroll";
			this.genHScroll.Size					= new System.Drawing.Size(574, 16);
			this.genHScroll.TabIndex				= 9;
			this.genHScroll.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.genHScroll_Scroll);
			// 
			// genVScroll
			// 
			this.genVScroll.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.genVScroll.Location				= new System.Drawing.Point(574,	0);
			this.genVScroll.Name					= "genVScroll";
			this.genVScroll.Size					= new System.Drawing.Size(16, 307);
			this.genVScroll.TabIndex				= 8;
			this.genVScroll.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.genVScroll_Scroll);
			// 
			// pnlGenerator
			// 
			this.pnlGenerator.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlGenerator.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlGenerator.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.btnGenerate,
																					   this.pnlGenList});
			this.pnlGenerator.Location				= new System.Drawing.Point(192,	8);
			this.pnlGenerator.Name					= "pnlGenerator";
			this.pnlGenerator.Size					= new System.Drawing.Size(360, 289);
			this.pnlGenerator.TabIndex				= 7;
			this.pnlGenerator.Resize				+= new System.EventHandler(this.pnlGenerator_Resize);
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor					= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.btnGenerate.BackColor				= System.Drawing.Color.DarkGreen;
			this.btnGenerate.FlatStyle				= System.Windows.Forms.FlatStyle.Flat;
			this.btnGenerate.Font					= new System.Drawing.Font("Microsoft Sans Serif", 9.75F, (System.Drawing.FontStyle.Bold	| System.Drawing.FontStyle.Italic),	System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnGenerate.ForeColor				= System.Drawing.Color.Chartreuse;
			this.btnGenerate.Location				= new System.Drawing.Point(258,	49);
			this.btnGenerate.Name					= "btnGenerate";
			this.btnGenerate.Size					= new System.Drawing.Size(80, 32);
			this.btnGenerate.TabIndex				= 0;
			this.btnGenerate.Text					= "NextGen";
			this.tt.SetToolTip(this.btnGenerate, "Press this button to generate all templates of the checked type");
			this.btnGenerate.Click					+= new System.EventHandler(this.btnGenerate_Click);
			// 
			// pnlGenList
			// 
			this.pnlGenList.BorderStyle				= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlGenList.ForeColor				= System.Drawing.SystemColors.ControlText;
			this.pnlGenList.Location				= new System.Drawing.Point(8, 8);
			this.pnlGenList.Name					= "pnlGenList";
			this.pnlGenList.Size					= new System.Drawing.Size(184, 178);
			this.pnlGenList.TabIndex				= 6;
			// 
			// tabTemplate
			// 
			this.tabTemplate.BackColor				= System.Drawing.Color.Orange;
			this.tabTemplate.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabTemplate.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.vscrDetail,
																					  this.pnlDetailMain,
																					  this.hscrOverview,
																					  this.vscrOverview,
																					  this.pnlOverviewMain});
			this.tabTemplate.Location				= new System.Drawing.Point(4, 22);
			this.tabTemplate.Name					= "tabTemplate";
			this.tabTemplate.Size					= new System.Drawing.Size(592, 327);
			this.tabTemplate.TabIndex				= 5;
			this.tabTemplate.Text					= "TEMPLATE";
			// 
			// vscrDetail
			// 
			this.vscrDetail.Anchor					= (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom);
			this.vscrDetail.Location				= new System.Drawing.Point(440,	0);
			this.vscrDetail.Name					= "vscrDetail";
			this.vscrDetail.Size					= new System.Drawing.Size(16, 326);
			this.vscrDetail.TabIndex				= 8;
			// 
			// pnlDetailMain
			// 
			this.pnlDetailMain.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlDetailMain.BackColor			= System.Drawing.Color.PaleGreen;
			this.pnlDetailMain.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlDetailMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.pnlDetailParts});
			this.pnlDetailMain.Location				= new System.Drawing.Point(256,	8);
			this.pnlDetailMain.Name					= "pnlDetailMain";
			this.pnlDetailMain.Size					= new System.Drawing.Size(184, 295);
			this.pnlDetailMain.TabIndex				= 7;
			// 
			// pnlDetailParts
			// 
			this.pnlDetailParts.Anchor				= System.Windows.Forms.AnchorStyles.Left;
			this.pnlDetailParts.BackColor			= System.Drawing.Color.MistyRose;
			this.pnlDetailParts.BorderStyle			= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlDetailParts.ForeColor			= System.Drawing.SystemColors.ControlText;
			this.pnlDetailParts.Location			= new System.Drawing.Point(0, 16);
			this.pnlDetailParts.Name				= "pnlDetailParts";
			this.pnlDetailParts.Size				= new System.Drawing.Size(144, 265);
			this.pnlDetailParts.TabIndex			= 6;
			// 
			// hscrOverview
			// 
			this.hscrOverview.Anchor				= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.hscrOverview.Location				= new System.Drawing.Point(0, 310);
			this.hscrOverview.Name					= "hscrOverview";
			this.hscrOverview.Size					= new System.Drawing.Size(208, 16);
			this.hscrOverview.TabIndex				= 6;
			// 
			// vscrOverview
			// 
			this.vscrOverview.Anchor				= (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom);
			this.vscrOverview.Location				= new System.Drawing.Point(208,	0);
			this.vscrOverview.Name					= "vscrOverview";
			this.vscrOverview.Size					= new System.Drawing.Size(16, 310);
			this.vscrOverview.TabIndex				= 2;
			this.vscrOverview.Visible				= false;
			this.vscrOverview.Scroll				+= new System.Windows.Forms.ScrollEventHandler(this.vscrOverview_Scroll);
			// 
			// pnlOverviewMain
			// 
			this.pnlOverviewMain.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pnlOverviewMain.BackColor			= System.Drawing.Color.YellowGreen;
			this.pnlOverviewMain.BorderStyle		= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlOverviewMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																						  this.pnlOverviewParts});
			this.pnlOverviewMain.Location			= new System.Drawing.Point(8, 8);
			this.pnlOverviewMain.Name				= "pnlOverviewMain";
			this.pnlOverviewMain.Size				= new System.Drawing.Size(184, 295);
			this.pnlOverviewMain.TabIndex			= 0;
			// 
			// pnlOverviewParts
			// 
			this.pnlOverviewParts.Anchor			= System.Windows.Forms.AnchorStyles.Left;
			this.pnlOverviewParts.BackColor			= System.Drawing.Color.RosyBrown;
			this.pnlOverviewParts.BorderStyle		= System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlOverviewParts.ForeColor			= System.Drawing.SystemColors.ControlText;
			this.pnlOverviewParts.Location			= new System.Drawing.Point(0, 16);
			this.pnlOverviewParts.Name				= "pnlOverviewParts";
			this.pnlOverviewParts.Size				= new System.Drawing.Size(144, 265);
			this.pnlOverviewParts.TabIndex			= 6;
			// 
			// btnReloadCache
			// 
			this.btnReloadCache.Anchor				= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.btnReloadCache.FlatStyle			= System.Windows.Forms.FlatStyle.Flat;
			this.btnReloadCache.Font				= new System.Drawing.Font("Webdings", 15.75F, System.Drawing.FontStyle.Regular,	System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnReloadCache.ImageAlign			= System.Drawing.ContentAlignment.TopCenter;
			this.btnReloadCache.Location			= new System.Drawing.Point(32, 353);
			this.btnReloadCache.Name				= "btnReloadCache";
			this.btnReloadCache.Size				= new System.Drawing.Size(32, 24);
			this.btnReloadCache.TabIndex			= 6;
			this.btnReloadCache.Text				= "q";
			this.tt.SetToolTip(this.btnReloadCache, "Reload the cache");
			this.btnReloadCache.Visible				= false;
			this.btnReloadCache.Click				+= new System.EventHandler(this.btnReloadCache_Click);
			// 
			// btnLeftLeft
			// 
			this.btnLeftLeft.Anchor					= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.btnLeftLeft.FlatStyle				= System.Windows.Forms.FlatStyle.Flat;
			this.btnLeftLeft.Font					= new System.Drawing.Font("Webdings", 15.75F, System.Drawing.FontStyle.Regular,	System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnLeftLeft.Location				= new System.Drawing.Point(0, 353);
			this.btnLeftLeft.Name					= "btnLeftLeft";
			this.btnLeftLeft.Size					= new System.Drawing.Size(32, 24);
			this.btnLeftLeft.TabIndex				= 7;
			this.btnLeftLeft.Text					= "9";
			this.tt.SetToolTip(this.btnLeftLeft, "Move all text content on general tab to left");
			this.btnLeftLeft.Visible				= false;
			// 
			// btnInventLabels
			// 
			this.btnInventLabels.Anchor				= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.btnInventLabels.FlatStyle			= System.Windows.Forms.FlatStyle.Flat;
			this.btnInventLabels.Font				= new System.Drawing.Font("Webdings", 15.75F, System.Drawing.FontStyle.Regular,	System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnInventLabels.ImageAlign			= System.Drawing.ContentAlignment.TopCenter;
			this.btnInventLabels.Location			= new System.Drawing.Point(64, 353);
			this.btnInventLabels.Name				= "btnInventLabels";
			this.btnInventLabels.Size				= new System.Drawing.Size(32, 24);
			this.btnInventLabels.TabIndex			= 8;
			this.btnInventLabels.Text				= "2";
			this.tt.SetToolTip(this.btnInventLabels, "Invent labels for attributes (i.e. copy)");
			this.btnInventLabels.Visible			= false;
			this.btnInventLabels.Click				+= new System.EventHandler(this.btnInventLabels_Click);
			// 
			// button1
			// 
			this.button1.Anchor						= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.button1.FlatStyle					= System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font						= new System.Drawing.Font("Webdings", 15.75F, System.Drawing.FontStyle.Regular,	System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.button1.ImageAlign					= System.Drawing.ContentAlignment.TopCenter;
			this.button1.Location					= new System.Drawing.Point(104,	352);
			this.button1.Name						= "button1";
			this.button1.Size						= new System.Drawing.Size(32, 32);
			this.button1.TabIndex					= 10;
			this.button1.Text						= "z";
			this.tt.SetToolTip(this.button1, "general panel new");
			this.button1.Click						+= new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor						= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left);
			this.button2.FlatStyle					= System.Windows.Forms.FlatStyle.Flat;
			this.button2.Font						= new System.Drawing.Font("Webdings", 15.75F, System.Drawing.FontStyle.Regular,	System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.button2.ImageAlign					= System.Drawing.ContentAlignment.TopCenter;
			this.button2.Location					= new System.Drawing.Point(144,	352);
			this.button2.Name						= "button2";
			this.button2.Size						= new System.Drawing.Size(32, 32);
			this.button2.TabIndex					= 11;
			this.button2.Text						= "x";
			this.tt.SetToolTip(this.button2, "attribute panel new");
			this.button2.Click						+= new System.EventHandler(this.button2_Click);
			// 
			// btnApply
			// 
			this.btnApply.Anchor					= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.btnApply.CausesValidation			= false;
			this.btnApply.DialogResult				= System.Windows.Forms.DialogResult.OK;
			this.btnApply.Location					= new System.Drawing.Point(520,	361);
			this.btnApply.Name						= "btnApply";
			this.btnApply.Size						= new System.Drawing.Size(72, 24);
			this.btnApply.TabIndex					= 9;
			this.btnApply.Text						= "Apply";
			this.btnApply.Click						+= new System.EventHandler(this.btnApply_Click);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index					= 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4});
			this.menuItem1.MergeOrder				= 30;
			this.menuItem1.Text						= "File";
			// 
			// menuItem4
			// 
			this.menuItem4.Index					= 0;
			this.menuItem4.MergeType				= System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem4.Text						= "Delete";
			// 
			// TemplateForm
			// 
			this.AcceptButton						= this.btnOK;
			this.AutoScaleBaseSize					= new System.Drawing.Size(5, 13);
			this.CancelButton						= this.btnCancel;
			this.ClientSize							= new System.Drawing.Size(600, 390);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button2,
																		  this.button1,
																		  this.btnApply,
																		  this.btnInventLabels,
																		  this.btnLeftLeft,
																		  this.btnReloadCache,
																		  this.btnCancel,
																		  this.btnOK,
																		  this.tabControl1});
			this.Name								= "TemplateForm";
			this.ShowInTaskbar						= false;
			this.Text								= "Template	Developer";
			this.Load								+= new System.EventHandler(this.TemplateForm_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabAttributes.ResumeLayout(false);
			this.pnlAttributes.ResumeLayout(false);
			this.tabSets.ResumeLayout(false);
			this.pnlSets.ResumeLayout(false);
			this.tabGenerator.ResumeLayout(false);
			this.pnlGenTables.ResumeLayout(false);
			this.pnlGenerator.ResumeLayout(false);
			this.tabTemplate.ResumeLayout(false);
			this.pnlDetailMain.ResumeLayout(false);
			this.pnlOverviewMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Close the window, but first save (when 'save'== true) 
		/// and when reload_data is set, make sure cache is reloaded with the
		/// old version of the data.
		/// </summary>
		/// <param name="save"></param>
		/// <param name="reload_data"></param>
		public void Close(bool save, bool reload_data)
		{
			if (save)
				SaveContents();
			else if (reload_data)
			{
				XmlNode					notinterestedintype;
				TemplateCache.Instance().GetFile(m_filename, out notinterestedintype);
			}
			Close();
		}

//		public static void ErrorMessage(string msg, string[] args)
//		{
//			string m = "BaseDirectory = " + System.AppDomain.CurrentDomain.BaseDirectory + "\n" +
//				"Excutable = " + System.AppDomain.CurrentDomain.FriendlyName;
//			if (args != null && args.GetLength(0) > 0)
//			{
//				int i = 0;
//				foreach (string a in args)
//					m += "\nArg[" + (i++).ToString() + "] = " + a;
//			}
//			MessageBox.Show(msg + "\n\n" + m, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//			return;
//		}

	//	public string[] m_args; 
		private void BuildTitle()
		{
			string						title		= m_currenttype.Attributes["definition"].Value;
			if (!m_file_editable)
				title								+= " [Read-only]";
			title									+= ": "	+ Path.GetFileNameWithoutExtension(m_filename);
			this.Text								= title;
		}

		private void TemplateForm_Load(object sender, System.EventArgs e)
		{
			this.tabControl1.Controls.Remove(this.tabTemplate);
			this.button1.Visible					= false;
			this.button2.Visible					= false;

			m_file_editable							= (m_currenttype.Attributes["writable"].Value == "true");
			BuildTitle();

			// Fill attributes panel
            XmlNode                     attributedefs
                                                    = m_type_definition.SelectSingleNode("attributes");
			if (attributedefs != null)
			{
				XmlNode					attributes	= m_currenttype.SelectSingleNode("attributes");
				if (attributes == null)
				{
					attributes						= m_currenttype.OwnerDocument.CreateElement("attributes");
					m_currenttype.AppendChild(attributes);
				}
				m_attributes						= new TemplatePanel(this, "attribute", attributedefs, attributes, pnlAList,	attrHScroll, attrVScroll);
				m_attributes.Fill();
			}
			else
			{
				this.tabControl1.Controls.Remove(this.tabAttributes);
			}

			// Fill General panel
			XmlNode						elementdefs	= m_type_definition.SelectSingleNode("elements");
			if (elementdefs != null)
			{
				FillGeneralPanel(pnlGeneral, elementdefs);
				tabGeneralResize();
			}

			// Fill attributesets panel
            XmlNode                     attributesetdefs
                                                    = m_type_definition.SelectSingleNode("sets");
			if (attributesetdefs != null)
			{
				XmlNode					sets		= m_currenttype.SelectSingleNode("sets");
				if (sets == null)
				{
					sets							= m_currenttype.OwnerDocument.CreateElement("sets");
					m_currenttype.AppendChild(sets);
				}
				// Add refids to the attributes in the sets.
				// maybe remove them before saving....
				string[]				definitive_names
													= m_attributes == null ? new string[0] : m_attributes.GetNamePanels();
			
				foreach (XmlNode x in sets.SelectNodes("set/attributes/attribute"))
				{
					XmlAttribute		a			= x.Attributes["refid"];
					if (a == null)
						a							= x.Attributes.Append(x.OwnerDocument.CreateAttribute("refid"));

					XmlNode				n			= x.SelectSingleNode("name");
					if (n != null)
					{
						// Find the id belonging to this name from the attributes panel
						int				idx			= Array.IndexOf(definitive_names, n.InnerText);
						a.Value						= idx.ToString();
					}
					else
					{
						a.Value						= "";
					}
				}
				// Check to see if attribute set panel is needed (it normally is)
				XmlNode					asad		= attributesetdefs.SelectSingleNode("element[@type='AttributeSet']");
				TemplatePanel			asadPanel	= null;
				if (asad != null)
					asadPanel						= new TemplatePanel(this, "attribute", asad, null, pnlSetAttributes, null, setsVScrollAttributes, null,	m_attributes);

				m_sets								= new TemplatePanel(this, "set", attributesetdefs, sets, pnlSList, setHScrollSets, setsVScrollSets,	asadPanel, m_attributes);
				m_sets.Fill();
				// Make the size of the asadpanel to fit.
				if (asad != null)
				{
					pnlSetAttributes.Left			= setsVScrollAttributes.Left - pnlSetAttributes.Width;
					setsVScrollSets.Left			= pnlSetAttributes.Left	- setsVScrollSets.Width;
					pnlSets.Width					= setsVScrollSets.Left;
					setHScrollSets.Width			= setsVScrollSets.Left - setHScrollSets.Left;
				}
			}
			else
			{
				this.tabControl1.Controls.Remove(this.tabSets);
			}

            // Fill Generator panel
            XmlNode						generatedefs	
													= m_type_definition.SelectSingleNode("generate");
            if (generatedefs != null)
            {
				XmlNode					generate	= m_currenttype.SelectSingleNode("generate");
                if (generate == null)
                {
					generate						= m_currenttype.OwnerDocument.CreateElement("generate");
                    m_currenttype.AppendChild(generate);
                }
				m_generate							= new TemplatePanel(this, "generate", generatedefs,	generate, pnlGenList, genHScroll, genVScroll);
                m_generate.Fill();

				// bepaal typenaam van het huidige type en zoek de types die hier 
				// naar verwijzen. Haal vervolgens de instances van die types
				// op die naar deze instance verwijzen.
				string					currenttypename 
													= m_currenttype.Attributes["definition"].Value;
				XmlNodeList				childtypes	= TemplateCache.Instance().GetTypesList("TypeDefs").SelectNodes("*/template/elements/element[@type='" +	currenttypename	+ "']/../../..");
				// for projects, fill the list of tables known, otherwise hide it.
//				if (m_currenttype.Attributes["definition"].Value == "Project")
				if (childtypes.Count > 0)
				{
					string				nameatt		= m_type_definition.Attributes["nameattribute"].Value;
					string				projectname	= m_currenttype.SelectSingleNode(nameatt).InnerText.Trim();

					pnlGenList.Left					+= pnlGenTables.Right;
					pnlGenList.Width				-= pnlGenTables.Right;
					pnlGenTables.Visible			= true;

					foreach (XmlNode childtype in childtypes)
					{
                        string          elementname_to_select
                                                    = childtype.SelectSingleNode("template/elements/element[@type='" + currenttypename + "']").Attributes["name"].Value;
						//string childnameelement = child.SelectSingleNode("template").Attributes["nameattribute"].Value;
					//
					//	foreach (XmlNode x in TemplateCache.Instance().GetTypesList("TableDefinition").SelectNodes("*/*[project='" + projectname + "']"))
					//		lstGenTableDefinitions.Items.Add(x.SelectSingleNode("tablename").InnerText, false);				
					//
						//XmlNode childtype = TemplateCache.Instance().GetTemplateType(child.Attributes["name"].Value);
                        XmlNode         childinstances
                                                    = TemplateCache.Instance().GetTypesList(childtype.Attributes["name"].Value);
						foreach (XmlNode childinstance in childinstances.SelectNodes("*/*[" + elementname_to_select + "='" + projectname + "']"))
						{
							//lstGenTableDefinitions.Items.Add(x.SelectSingleNode(childnameelement).InnerText, false);
							lstGenTableDefinitions.Items.Add(new GeneratorComboItem(childtype, childinstance), false);
						}
					}
					//since it can be anything, this link would not work anymore
					//TemplateMain.Instance().CreateLinkContextMenu(lstGenTableDefinitions, "TableDefinition");
				}
				else
				{
					pnlGenTables.Visible			= false;
				}
            }
            else
                this.tabControl1.Controls.Remove(this.tabGenerator);
			
			this.tabControl1.SelectedIndex			= 0;
			dirty									= false;
		}

//		private void FillControlsOnPanelFrom(Panel panel, XmlNode node)
//		{
//			foreach (XmlNode element in node.ChildNodes)
//			{
//				Control c = FindControl(element.Name, panel.Controls);
//				if (c != null)
//				{
//					c.Text = element.InnerText;
//				}
//			}
//
//		}
        private Control FindControl(string name, Control.ControlCollection controls)
        {
            foreach (Control c in controls)
                if (c.Name == name)
                    return c;
            return null;
        }


		TemplatePanel					m_attributes;
		TemplatePanel					m_sets;
		TemplatePanel					m_generate;

		private void FillGeneralPanel(Panel panel, XmlNode elements)
		{
			int							currenttop	= 0;
			const int labelwidth = 150;

			foreach (XmlNode element in elements.ChildNodes)
			{
				Label					l			= new System.Windows.Forms.Label();
				XmlAttribute			caption		= element.Attributes["caption"];
				XmlAttribute			name		= element.Attributes["name"];
				XmlAttribute			lines		= element.Attributes["lines"];
				XmlAttribute			length		= element.Attributes["length"];
				XmlAttribute			type		= element.Attributes["type"];
				XmlAttribute			def_val		= element.Attributes["default"];
				XmlAttribute			tt			= element.Attributes["tooltip"];

				l.Top								= currenttop + 4;
				l.Width								= labelwidth;
				l.Name								= "lbl"	+ name.Value;

				if (caption != null)
					l.Text							= caption.Value;
				else
					l.Text							= TemplateUtil.Instance().InventCaptionForName(name.Value);

				string					strToolTip	= "";
				if (tt != null)
					strToolTip						= tt.Value;
				strToolTip							+= "(" + name.Value	+ ")";

                this.tt.SetToolTip(l, strToolTip);

				Control					c			= null;

				// Get the current string value, if available.
				XmlNode					valueNode	= m_currenttype.SelectSingleNode(name.Value);
				string					stringValue	= "";
				if (valueNode == null)
				{
					valueNode						= m_currenttype.OwnerDocument.CreateElement(name.Value);
					m_currenttype.AppendChild(valueNode);
					if (def_val != null)
						valueNode.InnerText			= def_val.InnerText;
				}
				stringValue							= valueNode.InnerText;

				if ((type		== null)	|| 
					(type.Value == "Name")  ||
					(type.Value == "Number")  ||
					(type.Value == "LoopField")  ||
					(type.Value == "Text")  )
				{
					TextBox				t			= new TextBox();
					c								= t;
					int					nLines		= stringValue.Split('\n').Length;
					if (lines != null)
						nLines						= Math.Max(Int32.Parse(lines.Value), nLines);

					t.Multiline						= (nLines != 1);
					if (t.Multiline && nLines > 3)
						t.ScrollBars				= ScrollBars.Vertical;
					t.AcceptsReturn					= t.Multiline;
					t.Height						= nLines * 13 +	8;
					if (length != null)
						t.MaxLength					= Int32.Parse(length.Value);
					t.Text							= stringValue;
					if (type != null && type.Value == "LoopField")
					{
						ArrayList		ttparts		= new ArrayList();
						foreach (XmlNode e in element.SelectNodes("element"))
							ttparts.Add(e.Attributes["name"].Value);
						string			ttt;
						if (ttparts.Count > 1)
						{
							char		partsep		= TemplateGenerator.DEFAULT_PART_SEPERATOR;
							if (element.Attributes["partsep"] != null)
								partsep				= element.Attributes["partsep"].Value[0];
							String[]	ttstrings	= (ttparts.ToArray(typeof(String)) as String[]);
								ttt					= String.Join(partsep.ToString(), ttstrings);
						}
						else
							ttt						= ttparts[0].ToString();
						char			fieldsep	= TemplateGenerator.DEFAULT_FIELD_SEPERATOR;
						if (element.Attributes["fieldsep"] != null)
							fieldsep				= element.Attributes["fieldsep"].Value[0];
						ttt							= ttt +	fieldsep.ToString()	+ ttt;
						this.tt.SetToolTip(t, ttt);
					}
					t.TextChanged					+= new EventHandler(this.Mark_Dirty);				
				}
				else if (type.Value == "Guid")
				{
					// When not set, generate new guid
					TextBox				t			= new TextBox();
					Guid				g;
					if (stringValue != "")
						g							= new Guid(stringValue);
					else
					{
						g							= Guid.NewGuid();
						stringValue					= g.ToString();
						valueNode.InnerText			= stringValue;
					}	
					t.Text							= stringValue;
					t.Enabled						= false;
					t.BackColor						= System.Drawing.SystemColors.Window;
					t.ForeColor						= System.Drawing.SystemColors.ControlText;
					c								= t;
				}
				else if (type.Value == "Combobox")
				{
					// Get the values in the list from the child element
					// and put these in the combo
					ComboBox			t			= new ComboBox();
					foreach (XmlNode x in element.SelectNodes("element"))
						t.Items.Add(x.Attributes["name"].InnerText);
					t.Text							= stringValue;
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(this.Mark_Dirty);				
				}
				else if (type.Value == "AttributeCombobox")
				{
					// Get the values in the list from all attribute panels
					// and put these in the combo. Select the right one 
					// according to the id of the attribute.
					ComboBox			t			= new ComboBox();
					t.Sorted						= true;
					t.DropDownStyle					= ComboBoxStyle.DropDownList;
					t.Items.Add("");

					XmlAttribute		id			= null;
					//stringValue = "";
					id								= valueNode.Attributes["refid"];
					if (id == null)
					{
						id							= valueNode.OwnerDocument.CreateAttribute("refid");
						valueNode.Attributes.Append(id);
					}
					foreach (Control tb in this.m_attributes.NameControls)
					{
						if (tb.Text.Trim() == "")
							continue;

						t.Items.Add(tb.Text);
						// If no id connected yet, set it when value conforms
						if (stringValue == tb.Text) 
							id.Value				= tb.Parent.Name;
					}

					t.Text							= stringValue;
					c								= t;
					t.SelectedValueChanged			+= new System.EventHandler(this.AttributeCombo_SelectionChanged);
					t.SelectedValueChanged			+= new EventHandler(this.Mark_Dirty);
				}
				else if (type.Value	== "Checkbox")
				{
					CheckBox			t			= new CheckBox();
					t.ThreeState					= false;
					t.Checked						= (stringValue == "1");
					t.Height						-= 4;
					t.CheckedChanged				+= new EventHandler(this.Mark_Dirty);
					c								= t;
				}
				else if (type.Value	== "TableCollection")
				{
					// Tablecollection value is implicit by selecting
					// the project in the tabledefinition 
					Debug.Fail("TableCollection not supported any longer");
					continue;
				}
				else if (type.Value == "UserConcept")
				{
					// A combobox with user editable type selection
					// Find the right directory
					ComboBox			t			= new ComboBox();
					t.Sorted						= true;
					t.Items.AddRange(TemplateCache.Instance().GetViewableTypenamesList(true, false));
					t.Text							= stringValue;
//					TemplateMain.Instance().CreateLinkContextMenu(t, type.Value);
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(this.Mark_Dirty);
				}
				else
				{
					// It must almost be a combobox with references to other types.
					// Find the right directory
					ComboBox			t			= new ComboBox();
					t.Sorted						= true;
					t.Items.AddRange(TemplateCache.Instance().GetTypenamesList(type.Value));
					t.Text							= stringValue;
					TemplateMain.Instance().CreateLinkContextMenu(t, type.Value);
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(this.Mark_Dirty);
				}

				c.Name								= name.Value;
				c.Top								= currenttop;
				c.Left								= labelwidth + 8;
				c.Width								= panel.Width -	c.Left;
				c.Anchor							= AnchorStyles.Left	| AnchorStyles.Right | AnchorStyles.Top;
				
				l.Height							= c.Height - 4;
				panel.Controls.Add(l);
				panel.Controls.Add(c);
				
				currenttop							+= c.Height	+ 4;
			}
			panel.Height							= currenttop;
		}

		private void tabGeneralResize()
		{
			if (tabGeneral.Height >= this.pnlGeneral.Height)
			{
				this.gVScroll.Visible				= false;
				pnlGeneral.Width					= tabGeneral.Width;
				pnlGeneral.Top						= 0;
			}
			else
			{
				this.gVScroll.Visible				= true;
				pnlGeneral.Width					= tabGeneral.Width - this.gVScroll.Width;
				this.gVScroll.Minimum				= 0;
				int						nPages		= 1	+ (int)	Math.Floor((double)(pnlGeneral.Height -	1) / tabGeneral.Height);
				this.gVScroll.Maximum				= nPages * tabGeneral.Height;
				this.gVScroll.LargeChange			= tabGeneral.Height;
				if (this.gVScroll.LargeChange > 100)
					this.gVScroll.SmallChange		= this.gVScroll.LargeChange	/ 10;
				else
					this.gVScroll.SmallChange		= 10;
				if (pnlGeneral.Top < -gVScroll.Maximum)
				{
					pnlGeneral.Top					= -gVScroll.Maximum;
				}
				
				gVScroll.Value						= -	pnlGeneral.Top;
			}
		}

		/// <summary>
		/// Refresh all combos showing attributenames with a fresh list
		/// of attributenames.
		/// </summary>
		/// <param name="panel">panel to check</param>
		/// <param name="elements">definition of the items on the panel</param>
		private void CheckPanelForAttributes(Panel panel, XmlNode elements)
		{
			foreach (XmlNode element in elements.SelectNodes("element[@type='AttributeCombobox']"))
			{
				XmlAttribute			name		= element.Attributes["name"];
				ComboBox				t			= (FindControl(name.Value, panel.Controls) as ComboBox);
				
				System.Diagnostics.Debug.Assert(t != null, "Combo shouldnt be null!");
				
				// Get the current string value, if available.
				XmlNode					valueNode	= m_currenttype.SelectSingleNode(name.Value);
				System.Diagnostics.Debug.Assert(valueNode != null, "Value shouldnt be null!");
				string					stringValue	= "";

				// Get the values in the list from all attribute panels
				// and put these in the combo. Select the right one 
				// according to the id of the attribute.
				t.Items.Clear();
				t.Items.Add("");

				XmlAttribute			id			= valueNode.Attributes["refid"];
				foreach (Control tb in this.m_attributes.NameControls)
				{
					if (tb.Text.Trim() == "")
						continue;

					t.Items.Add(tb.Text);
					// If id connected, set it to right value
					if (id.Value == tb.Parent.Name)
						stringValue					= tb.Text;
				}

				t.Text								= stringValue;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			if (dirty && m_file_editable && m_filename != "")
			{
				DialogResult r = MessageBox.Show(TemplateMain.Instance(), 
					"Lose changes?", "Changes will be lost",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				switch(r)
				{
					case DialogResult.Cancel:
						return;
					case DialogResult.Yes:
						// no need for reload, since we've use a clone of the 
						// cached contents. New form will use cache value.
						//TemplateCache.Instance().ReloadFile(m_filename);
						break;
					case DialogResult.No:
						SaveContents();
						break;
				}
			}

			// Fix the contents of the cache.
			if (dirty && !m_file_editable && m_filename != "")
				TemplateCache.Instance().ReloadFile(m_filename);
			

			this.Close();
		}

        private void CleanUnNamed(string nameelement, XmlNodeList list)
        {
            if (list == null)
				return;

            foreach (XmlElement x in list)
            {
				XmlNode					n			= x.SelectSingleNode(nameelement);
                if (n == null || n.InnerText.Trim() == "")
                    x.ParentNode.RemoveChild(x);
            }
        }

		/// <summary>
		/// Verwerk de edits in het huidige onderhanden item, en lever dit op als XML Document
		/// </summary>
        public XmlDocument GetCleanDocument()
        {
            // Read de handel
            TemplatePanel.ReadPanel(pnlGeneral, m_currenttype);

            XmlDocument                 save_document
                                                    = new XmlDocument();

            save_document.AppendChild(save_document.ImportNode(m_currenttype, true));
            
			// Clean attributes without name
			XmlNode						t			= m_type_definition.SelectSingleNode("attributes");
			if (t != null)
			{
				t									= m_type_definition.SelectSingleNode("attributes/element[@type='Name']");
				//Debug.Assert(t != null, "Type misses attributes/element with type 'Name'");
				if (t != null)
					CleanUnNamed(t.Attributes["name"].Value, save_document.SelectNodes("type/attributes/attribute"));
			}

			// Clean sets without name
			if (m_type_definition.SelectNodes("sets/element").Count != 0)
			{
				t									= m_type_definition.SelectSingleNode("sets/element[@type='Name']");
				Debug.Assert(t != null, "Type misses sets/element with type 'Name'");
				if (t != null)
					CleanUnNamed(t.Attributes["name"].Value, save_document.SelectNodes("type/sets/set"));

				if (m_type_definition.SelectNodes("sets/element/element").Count != 0)
				{
					t								= m_type_definition.SelectSingleNode("sets/element[@type='AttributeSet']/element[(@type='Name' or @type='AttributeCombobox')]");
					Debug.Assert(t != null, "Type misses sets/element[AttributeSet]/element with type 'Name' or 'AttibuteCombobox'");
					if (t != null)
						CleanUnNamed(t.Attributes["name"].Value, save_document.SelectNodes("type/sets/set/attributes/attribute"));
				}
			}

			// Clean sets without name
			t										= m_type_definition.SelectSingleNode("generate");
			if (t != null)
			{
				t									= m_type_definition.SelectSingleNode("generate/element[@type='Name']");
				Debug.Assert(t != null, "Type misses generate/element with type 'Name'");
				if (t != null)
					CleanUnNamed(t.Attributes["name"].Value, save_document.SelectNodes("type/generate/generate"));
			}

            return save_document;
        }

        private void btnOK_Click(object sender, System.EventArgs e)
		{
            try
            {
                if (m_file_editable)
                    SaveContents();
                else if (dirty)
                    TemplateCache.Instance().ReloadFile(m_filename);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

		//private XmlDocument m_document;
		private	XmlNode					m_type_definition;
		private	XmlNode					m_currenttype;
		private	string					m_filename;


		private void attrVScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				m_attributes.VScrollTo(e.NewValue);
		}

		private void attrHScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				m_attributes.HScrollTo(e.NewValue);
		}

		private void pnlAttributes_Resize(object sender, System.EventArgs e)
		{
			if (m_attributes != null)
				m_attributes.Resize();
		}

		private void setsVScrollSets_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				m_sets.VScrollTo(e.NewValue);
		}

		private void setHScrollSets_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				m_sets.HScrollTo(e.NewValue);
		}

		private void pnlSets_Resize(object sender, System.EventArgs e)
		{
			if (m_sets != null)
				m_sets.Resize();
		}

		private void tabSets_Enter(object sender, System.EventArgs e)
		{
			m_sets.Enter();

			// Make the size of the asadpanel to fit.
			pnlSetAttributes.Left					= setsVScrollAttributes.Left - pnlSetAttributes.Width;
			setsVScrollSets.Left					= pnlSetAttributes.Left	- setsVScrollSets.Width;
			setHScrollSets.Width					= setsVScrollSets.Left - setHScrollSets.Left;
			pnlSets.Width							= setsVScrollSets.Left;
		}

		/// <summary>
		/// Genereer de boel voor de huidige instance, met de eventuele onderdelen die
		/// aangekozen zijn in de lijst met onderliggende zaken
		/// </summary>
		private void btnGenerate_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.Cursor							= Cursors.WaitCursor;

                XmlNode                 currentinstance
                                                    = GetCleanDocument().SelectSingleNode("type");
                string                  currentinstancename
                                                    = currentinstance.SelectSingleNode(m_type_definition.Attributes["nameattribute"].Value).InnerText;
                string                  currentinstancetypename
                                                    = currentinstance.Attributes["definition"].Value;

				// First add all items to do to a list, then do them and present progress...
                List<GenerationRequest> todo        = new List<GenerationRequest>();

				// Process all possible files that can be used to generate
				foreach (XmlNode templatefiletype in TemplateCache.Instance().GetTypesList("__TemplateFile"))
				{
					// Get the group of templatefiles this file belongs to. If none, skip.
					XmlNode				ttype		= templatefiletype.SelectSingleNode("type/group");
					if (ttype == null)
						continue;

					// Check if the group fo this file is checked on the panel, if not, skip it.
					if (currentinstance.SelectSingleNode("generate/generate[group='" + ttype.InnerText + "' and generate='1']") == null)
						continue;
					
                    string              templatefile_applies_to
                                                    = templatefiletype.SelectSingleNode("type/appliesto").InnerText;

					// If the current type (Concept) is the type to which this templatefile is applicable, do it!
					if (currentinstancetypename.Equals(templatefile_applies_to, StringComparison.CurrentCultureIgnoreCase))
					{
						todo.Add(new GenerationRequest(
							m_type_definition, 
							currentinstance,
							templatefiletype.FirstChild, 
							currentinstancename)
							);
						
					}
					// Check all underlaying checked items, if it is of a type to which the templatefile is applicable 
					foreach (GeneratorComboItem item in lstGenTableDefinitions.CheckedItems)
					{
						if (templatefile_applies_to.Equals(item.GetTypename(), StringComparison.CurrentCultureIgnoreCase))
						{
							todo.Add(new GenerationRequest(
								item.m_type.FirstChild,
								item.GetInstance(),
								templatefiletype.FirstChild,
								item.ToString())
								);
						}
					}
					
				}

                /*
				TemplateMain			main		= this.MdiParent as	TemplateMain;
                main.StartOutput();
				GenerateOutput			o			= new GenerateOutput(main.AddToOutputText);
				o.Reset(todo.ToArray(typeof(GenerationRequest)) as GenerationRequest[]);
			
				o.GO(); */
                TemplateMain.Instance().Generate(todo);
            }
			catch (Exception ex)
			{
				MessageBox.Show("Exception: " + ex.Message);
			}
			finally
			{
				this.Cursor							= Cursors.Default;
			}
        }

        private void tabGenerator_Click(object sender, System.EventArgs e)
        {
        
        }

        private void genVScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll)
                m_generate.VScrollTo(e.NewValue);
        }

        private void genHScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll)
                m_generate.HScrollTo(e.NewValue);
        }

        private void pnlGenerator_Resize(object sender, System.EventArgs e)
        {
			if (m_generate != null)
				m_generate.Resize();
        }

		private void AttributeCombo_SelectionChanged(object sender, System.EventArgs e)
		{
			ComboBox					s			= (sender as ComboBox);
			XmlNode						valueNode	= m_currenttype.SelectSingleNode(s.Name);
			string						sel_value	= (s.SelectedItem as string).Trim();

			System.Diagnostics.Debug.Assert(valueNode != null, "Value node should be available");

			XmlAttribute				id			= null;
			id										= valueNode.Attributes["refid"];
			System.Diagnostics.Debug.Assert(id != null, "refid attribute should be set");

			id.Value								= "";
			if (sel_value != "")
			{
				foreach (Control tb in this.m_attributes.NameControls)
				{
					if (sel_value == tb.Text) 
						id.Value					= tb.Parent.Name;
				}
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabControl1.SelectedIndex == 0)
			{
				// Fill General panel
				XmlNode					elementdefs	= m_type_definition.SelectSingleNode("elements");
				if (elementdefs == null)
					return;

				CheckPanelForAttributes(pnlGeneral, elementdefs);
				
			}
		}

		private void gVScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			pnlGeneral.Top							= -	e.NewValue;
		}

		private void tabGeneral_Resize(object sender, System.EventArgs e)
		{
			tabGeneralResize();
		}

		public void MoveTextOnGeneralPanelLeft()
		{
			foreach (Control c in pnlGeneral.Controls)
			{
				if (! (c is TextBox))
					continue;
				TextBox					t			= (TextBox)	c;
				string[]				lines		= t.Text.Split('\n');
				int						minimumspace= 1000;
				foreach (String s in lines)
				{
					if (s.Trim().StartsWith("@"))
						continue;
					//tel start spaties
					int					sp			= s.Length - s.TrimStart(' ').Length;
					if ((sp < minimumspace) && (s.Trim() != ""))
						minimumspace				= sp;
				}
				if (minimumspace == 1000)
					continue;
				for (int si = 0; si < lines.GetLength(0); si++)
				{
					String				s			= lines[si];
					if (s.Trim().StartsWith("@"))
						continue;
					//verwijder start spaties
					if (s.Trim() != "")
						lines[si] = s.Substring(minimumspace);
				}
				t.Text								= String.Join("\n",	lines);
			}
		}

		/// <summary>
		/// When contents have changed, save these to its file.
		/// Mark object as clean.
		/// </summary>
		public void SaveContents()
		{
			if (!m_dirty)
				return;

			XmlDocument					x			= GetCleanDocument();
			
			string						newname		= "";
			string						oldname		= "";

            // Update the data in the cache.
            TemplateCache.Instance().RefreshValue(out m_filename, x.DocumentElement, out oldname, out newname);
            m_currenttype.Attributes["sourcefile"].Value = m_filename;
            BuildTitle();

            dirty = false;

            // Tell the main window about the changes
            TemplateMain.Instance().PropagateChanges(m_currenttype, oldname, newname);
		}

        private bool m_dirty;
        private bool m_file_editable;
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
						btnApply.Enabled			= m_file_editable;
					
						if (!m_file_editable)
							MessageBox.Show("Changes will not be saved! Reload the file from a writable version to be able to save your changes!", "Read-only warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
				else
				{
					btnApply.Enabled				= false;
				}
				m_dirty								= value;
			}
		}

		public void Mark_Dirty(object sender, System.EventArgs e)
		{
			dirty									= true;
		}

		public void PropagateChanges(XmlNode type, string oldname, string newname)
		{
			if (type == null)
				return;

			if (m_currenttype == type)
			{
				System.Diagnostics.Debug.Write("Propagate changes van me eige. Kunnen we rustig ignore");
				return;
			}

			if (this.m_generate != null)
				m_generate.PropagateChanges(type, oldname, newname);
			if (this.m_attributes != null)
				m_attributes.PropagateChanges(type, oldname, newname);
			if (this.m_sets != null)
				m_sets.PropagateChanges(type, oldname, newname);
				
			string						changedtype	= type.Attributes["definition"].Value;
			bool						needupdate	= false;
			for (int i = 0; i < lstGenTableDefinitions.Items.Count; i++)
			{
				GeneratorComboItem		gci			= lstGenTableDefinitions.Items[i] as GeneratorComboItem;
				if (gci.GetTypename() != changedtype || oldname == "")
				{
					continue;
				}
				if (gci.ToString() == oldname)
				{
					gci.ChangeName(newname);
					needupdate						= true;
				}
			}
			if (needupdate)
			{
				lstGenTableDefinitions.Refresh();
			}

			System.Diagnostics.Debug.Write("Propagate changes van '" + oldname + "' heet nu '" + newname + "'. Kan ik nog ff niks mee");

 			foreach (XmlNode element in m_type_definition.SelectSingleNode("elements").SelectNodes("element"))
			{
                if (element.Attributes["type"] == null)
                {
                    continue;
                }
				string					typename	= element.Attributes["type"].Value;
				if (typename != changedtype)
				{
					continue;
				}
                
				// ! AHA! bijwerken die zaak
				string					elname		= element.Attributes["name"].Value;
				ComboBox				combo		= FindControl(elname, this.pnlGeneral.Controls)	as ComboBox;
				System.Diagnostics.Debug.Assert(combo != null, "Het is geen combobox!");
					
				bool					wasselected	= (combo.Text == oldname);
				// remove old name, if given
				if (oldname != "")
				{
					for (int i = 0; i < combo.Items.Count; i++)
					{
						if (combo.Items[i].ToString() == oldname)
						{
							combo.Items.RemoveAt(i);
							break;
						}
					}
				}
				// add the new name to the list
				if (newname != "")
					combo.Items.Add(newname);
				if (wasselected)
					combo.Text						= newname;
			}
		}

		private void btnReloadCache_Click(object sender, System.EventArgs e)
		{
			SaveContents();
            TemplateCache.Instance().Clear(true);
			this.TemplateForm_Load(sender, e);
		}

		private void btnInventLabels_Click(object sender, System.EventArgs e)
		{
			if (m_attributes == null)
				return;

			foreach (Control c in pnlAList.Controls)
			{
				Panel					p			= (c as	Panel);
				if (p == null)
					continue;
				
				Control					l			= FindControl("label", p.Controls);
				if (l == null)
					continue;
				
				Control					n			= FindControl("name", p.Controls);
				if (n == null)
					continue;

				if (l.Text == "")
					l.Text							= n.Text;
			}
			m_attributes.ReadAllPanels();
			dirty									= true;
		}

		/// <summary>
		/// Set all checks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGenAll_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < lstGenTableDefinitions.Items.Count; i++)
				lstGenTableDefinitions.SetItemChecked(i, true);
		}

		/// <summary>
		/// Clear all checks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGenNone_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < lstGenTableDefinitions.Items.Count; i++)
				lstGenTableDefinitions.SetItemChecked(i, false);
		}

		/// <summary>
		/// Swap checked state of item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGenSwap_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < lstGenTableDefinitions.Items.Count; i++)
				lstGenTableDefinitions.SetItemChecked(i, !lstGenTableDefinitions.GetItemChecked(i));
		}

		private void lstGenTableDefinitions_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				int						item		= lstGenTableDefinitions.IndexFromPoint(e.X,e.Y);
				lstGenTableDefinitions.Text			= (lstGenTableDefinitions.Items[item] as String);
				//lstGenTableDefinitions.Tag = "TableDefinition";
			}
		}

        private void btnApply_Click(object sender, System.EventArgs e)
        {
            try
            {
                SaveContents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

		private void mnuMoveLeft_Click(object sender, System.EventArgs e)
		{
			MoveTextOnGeneralPanelLeft();
		}

		private void mnuReload_Click(object sender, System.EventArgs e)
		{
			if (dirty && MessageBox.Show("Changes you made will be lost!", "Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			TemplateCache.Instance().ReloadFile(m_filename);

			m_currenttype							= null;
			m_type_definition						= null;
			try
			{
				m_currenttype						= TemplateCache.Instance().GetFile(m_filename, out m_type_definition);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not load file.\n(file=" + m_filename + ")\n(exception=" + ex.Message + ")");
			}

			// Refill the form.
			TemplateForm_Load(sender, e);			
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			// Fill attributes panel
			XmlNode						attrdefs	= m_type_definition.SelectSingleNode("attributes");
			if (attrdefs != null)
			{
				XmlNode					attributes	= m_currenttype.SelectSingleNode("attributes");
				TemplatePanel2			attrv2		= new TemplatePanel2(this, "attribute",	attrdefs, attributes);

				attrv2.Fill();
				tabControl1.Controls.Add(attrv2.page);
				attrv2.page.Text					= "Attribs";
			}

		}

		private void vscrOverview_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
		
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			// Fill attributes panel
			XmlNode						attrdefs	= m_type_definition.SelectSingleNode("elements");
			if (attrdefs != null)
			{
				XmlNode					attributes	= m_currenttype;//.SelectSingleNode("attributes");
				XmlAttribute			att			= attrdefs.OwnerDocument.CreateAttribute("repeat");
				att.Value							= "false";
				attrdefs.Attributes.Append(att);
				
				TemplatePanel2			attrv2		= new TemplatePanel2(this, ".",	attrdefs, attributes);

				attrv2.Fill();
				tabControl1.Controls.Add(attrv2.page);
				attrv2.page.Text					= "Gen";
			}
		
		}

		private void pnlGeneral_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
		{
		
		}


		///
		/////////////////////////////// Attribute Sets //////////////////////////////////
		///

	}

	/// <summary>
	/// Class contains items for the selection list on the generation panel.
	/// </summary>
	public class GeneratorComboItem
	{
		public GeneratorComboItem(XmlNode type, XmlNode instance)
		{
			m_type									= type;

			XmlAttribute				typename	= m_type.Attributes["name"];
			m_typename								= typename.InnerText;
			XmlAttribute				nameattr	= m_type.FirstChild.Attributes["nameattribute"];
			XmlNode						instname	= instance.SelectSingleNode(nameattr.Value);
			m_name									= instname.InnerText;
		}

		public XmlNode					m_type;
		// store the name of the thing, get the right fresh instance
		// when needed.
		private	string					m_name;
		private	string					m_typename;

		public XmlNode GetInstance()
		{
			return TemplateCache.Instance().GetValueFor(m_typename, m_name);
		}

		public string GetTypename()
		{
			//return m_instance.Attributes["definition"].Value;
			return m_typename;
		}
		public void ChangeName(string newname)
		{
			m_name									= newname;
		}
		public override string ToString()
		{
			return m_name;
		}
	}
}
