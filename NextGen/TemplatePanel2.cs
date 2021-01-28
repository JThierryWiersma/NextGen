/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) 2006  Thierry Wiersma													*
*																						*
*	This program is free software; you can redistribute it and/or						*
*	modify it under the terms of the GNU General Public License							*
*	as published by the Free Software Foundation; either version 2						*
*	of the License, or (at your option) any later version.								*
*																						*
*	This program is distributed in the hope that it will be useful,						*
*	but WITHOUT ANY WARRANTY; without even the implied warranty of						*
*	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the						*
*	GNU General Public License for more details.										*
*																						*
*	You should have received a copy of the GNU General Public License					*
*	along with this program; if not, write to the Free Software							*
*	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA		*
*****************************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using Generator.Utility;

namespace Generator
{
	/// <summary>
	/// A TemplatePanel2 contains one or more panels on which controls are
	/// available for the user to update. 
	/// Each panel has at least a control in which the Name of the item is entered
	/// and a UpDownNumericControl in which the ordering of the panels is set.
	/// The panels are displayed in the order of the ordering control. This is
	/// done by positioning the panels right. Not by scrolling the containing panel
	/// since also labels identifying the controls are to be left in sight.
	/// 
	/// There are a lot o panels, with a name corresponding to a number 
	/// 
	/// 
	/// </summary>
	public class TemplatePanel2
	{
		private	int						m_panelrowcount;
		private	int						m_panelheight;			// height of one panel.
		private	int						m_currenttop;
		private	XmlNode					m_values;
		private	XmlNode					m_definitions;
		private	int						m_starttop	= 20;
		private	int						m_definitions_fixed;
		private	int						m_definitions_count;
		private	bool					m_repeating;			// has overview	panel
		private	bool					m_detail;				// has detail panel
		private	int						m_labelwidth;
		private	string					m_elementname;
		// one for each attribute
		// on position x the values for panel named x
		private	ArrayList				m_NameControls;	
		private	ArrayList				m_OrderControls;	
		private	ArrayList				m_Value;				// XmlNode with	the	value
		// one for each attributedefinition
		private	ArrayList				m_LabelControls;
		private	ArrayList				m_AttributeControls;

		private	TemplateForm			m_owner;
		private	TabPage					tabTemplate;
		private	Panel					pnlOverviewMain;
		private	Panel					pnlOverviewParts;
		private	Panel					pnlOverviewLabels;
		private	ScrollBar				hscrOverview;
		private	ScrollBar				vscrOverview;
		private	Panel					pnlDetailMain;
		private	Panel					pnlDetailParts;
		private	ScrollBar				vscrDetail;

		private	Panel					m_previous_entered_panel;
		private	ToolTip					m_tt;

		public TabPage page
		{
			get
			{
				return tabTemplate;
			}
		}

		private void InitializeComponent()
		{
			this.tabTemplate						= new System.Windows.Forms.TabPage();
			this.hscrOverview						= new System.Windows.Forms.HScrollBar();
			this.vscrOverview						= new System.Windows.Forms.VScrollBar();
			this.pnlOverviewMain					= new System.Windows.Forms.Panel();
			this.pnlOverviewParts					= new System.Windows.Forms.Panel();
			this.pnlOverviewLabels					= new System.Windows.Forms.Panel();
			this.pnlDetailMain						= new System.Windows.Forms.Panel();
			this.pnlDetailParts						= new System.Windows.Forms.Panel();
			this.vscrDetail							= new System.Windows.Forms.VScrollBar();
			this.tabTemplate.SuspendLayout();
			this.pnlOverviewMain.SuspendLayout();
			this.pnlDetailMain.SuspendLayout();

			// 
			// tabTemplate
			// 
			//this.tabTemplate.BackColor = System.Drawing.Color.Orange;
			this.tabTemplate.BackColor				= System.Drawing.SystemColors.Control;
			this.tabTemplate.BorderStyle			= System.Windows.Forms.BorderStyle.None; //FixedSingle;
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
			this.tabTemplate.Resize					+= new System.EventHandler(this.tabTemplate_Resize);
			// 
			// hscrOverview
			// 
			this.hscrOverview.Anchor				= (System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			this.hscrOverview.Location				= new System.Drawing.Point(0, 310);
			this.hscrOverview.Name					= "hscrOverview";
			this.hscrOverview.Size					= new System.Drawing.Size(208, 16);
			this.hscrOverview.TabIndex				= 6;
			this.hscrOverview.Scroll				+= new System.Windows.Forms.ScrollEventHandler(this.hscrOverview_Scroll);
			// 
			// vscrOverview
			// 
			this.vscrOverview.Anchor				= (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.vscrOverview.Location				= new System.Drawing.Point(208,	0);
			this.vscrOverview.Name					= "vscrOverview";
			this.vscrOverview.Size					= new System.Drawing.Size(16, 310);
			this.vscrOverview.TabIndex				= 2;
			this.vscrOverview.Scroll				+= new System.Windows.Forms.ScrollEventHandler(this.vscrOverview_Scroll);
			// 
			// pnlOverviewMain
			// 
			this.pnlOverviewMain.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			//this.pnlOverviewMain.BackColor = System.Drawing.Color.YellowGreen;
			this.pnlOverviewMain.BackColor			= System.Drawing.SystemColors.Control;
			this.pnlOverviewMain.BorderStyle		= System.Windows.Forms.BorderStyle.None;
			this.pnlOverviewMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																						  this.pnlOverviewParts,
																						  this.pnlOverviewLabels});
			this.pnlOverviewMain.Location			= new System.Drawing.Point(8, 8);
			this.pnlOverviewMain.Name				= "pnlOverviewMain";
			this.pnlOverviewMain.Size				= new System.Drawing.Size(184, 295);
			this.pnlOverviewMain.TabIndex			= 0;
			this.pnlOverviewMain.Resize				+= new System.EventHandler(this.pnlOverviewMain_Resize);
			// 
			// pnlOverviewParts
			// 
			this.pnlOverviewParts.Anchor			= System.Windows.Forms.AnchorStyles.Left;
			//this.pnlOverviewParts.BackColor = System.Drawing.Color.RosyBrown;
			this.pnlOverviewParts.BackColor			= System.Drawing.SystemColors.Control;
			this.pnlOverviewParts.BorderStyle		= System.Windows.Forms.BorderStyle.None;
			this.pnlOverviewParts.ForeColor			= System.Drawing.SystemColors.ControlText;
			this.pnlOverviewParts.Location			= new System.Drawing.Point(0, 16);
			this.pnlOverviewParts.Name				= "pnlOverviewParts";
			this.pnlOverviewParts.Size				= new System.Drawing.Size(144, 265);
			this.pnlOverviewParts.TabIndex			= 6;
			// 
			// pnlOverviewLabels
			// 
			this.pnlOverviewLabels.Anchor			= System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
			//this.pnlOverviewLabels.BackColor = System.Drawing.Color.AliceBlue;
			this.pnlOverviewLabels.BackColor		= System.Drawing.SystemColors.Control;
			this.pnlOverviewLabels.BorderStyle		= System.Windows.Forms.BorderStyle.None;
			this.pnlOverviewLabels.ForeColor		= System.Drawing.SystemColors.ControlText;
			this.pnlOverviewLabels.Location			= new System.Drawing.Point(0, 0);
			this.pnlOverviewLabels.Name				= "pnlOverviewLabels";
			this.pnlOverviewLabels.Size				= new System.Drawing.Size(144, 265);
			this.pnlOverviewLabels.TabIndex			= 7;
			// 
			// pnlDetailMain
			// 
			this.pnlDetailMain.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			//this.pnlDetailMain.BackColor = System.Drawing.Color.PaleGreen;
			this.pnlDetailMain.BackColor			= System.Drawing.SystemColors.Control;
			this.pnlDetailMain.BorderStyle			= System.Windows.Forms.BorderStyle.None;
			this.pnlDetailMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.pnlDetailParts});
			this.pnlDetailMain.Location				= new System.Drawing.Point(256,	8);
			this.pnlDetailMain.Name					= "pnlDetailMain";
			this.pnlDetailMain.Size					= new System.Drawing.Size(184, 100);
			this.pnlDetailMain.TabIndex				= 7;
			this.pnlDetailMain.Resize				+= new System.EventHandler(this.pnlDetailMain_Resize);
			// 
			// pnlDetailParts
			// 
			this.pnlDetailParts.Anchor				= System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			//this.pnlDetailParts.BackColor = System.Drawing.Color.MistyRose;
			this.pnlDetailParts.BackColor			= System.Drawing.SystemColors.Control;
			this.pnlDetailParts.BorderStyle			= System.Windows.Forms.BorderStyle.None;
			this.pnlDetailParts.ForeColor			= System.Drawing.SystemColors.ControlText;
			this.pnlDetailParts.Location			= new System.Drawing.Point(0, 16);
			this.pnlDetailParts.Name				= "pnlDetailParts";
			this.pnlDetailParts.Size				= new System.Drawing.Size(344, 100);
			this.pnlDetailParts.TabIndex			= 6;
			// 
			// vscrDetail
			// 
			this.vscrDetail.Anchor					= (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom	| System.Windows.Forms.AnchorStyles.Right);
			this.vscrDetail.Location				= new System.Drawing.Point(440,	0);
			this.vscrDetail.Name					= "vscrDetail";
			this.vscrDetail.Size					= new System.Drawing.Size(16, 326);
			this.vscrDetail.TabIndex				= 8;
			this.vscrDetail.Scroll					+= new System.Windows.Forms.ScrollEventHandler(this.vscrDetail_Scroll);

			this.tabTemplate.ResumeLayout(false);
			this.pnlOverviewMain.ResumeLayout(false);
			this.pnlDetailMain.ResumeLayout(false);

		}

		public TemplatePanel2(TemplateForm owner, string elementname, XmlNode definitions, XmlNode values)
		{
			m_owner									= owner;
			m_definitions							= definitions;
			m_values								= values;

			InitializeComponent();
			m_NameControls							= new ArrayList();
			m_OrderControls							= new ArrayList();
			m_Value									= new ArrayList();
			m_LabelControls							= new ArrayList();
			m_AttributeControls						= new ArrayList();
			m_elementname							= elementname;
			m_tt									= new ToolTip();
		}
	
		public ArrayList NameControls
		{
			get
			{
				return m_NameControls;
			}
		}
		public XmlNode definitions
		{
			get
			{
				return m_definitions;
			}
		}
		public void Fill()
		{
			System.Diagnostics.Debug.Assert(m_values != null, "Shit m_values is empty");

			m_NameControls.Clear();
			m_AttributeControls.Clear();
			m_LabelControls.Clear();
			m_OrderControls.Clear();
			m_Value.Clear();

			m_definitions_count						= m_definitions.ChildNodes.Count;
			
			m_definitions_fixed						= 2;
			XmlNode						attr		= m_definitions.Attributes["fixed"];
			if (attr != null)
				m_definitions_fixed					= int.Parse(attr.Value);
			
			m_labelwidth							= 150;
			attr									= m_definitions.Attributes["labelwidth"];
			if (attr != null)
				m_labelwidth						= int.Parse(attr.Value);

			m_repeating								= true;
			attr									= m_definitions.Attributes["repeat"];
			if (attr != null)
				m_repeating							= bool.Parse(attr.Value);

			m_detail								= !	m_repeating; //	default	not(repeating)
			attr									= m_definitions.Attributes["detail"];
			if (attr != null)
				m_detail							= bool.Parse(attr.Value);

			m_currenttop							= 0; //m_starttop;
			m_panelrowcount							= 0;

			CreateControls(false);
			CreateLabels(false);
			
			XmlNodeList					todo;
			if (m_elementname == ".")
				todo								= m_values.SelectNodes(".");
			else
				todo								= m_values.SelectNodes(m_elementname);
			foreach (XmlNode n in todo)
			{
				m_Value.Add(n);
				if (m_repeating)
				{
					Panel				p			= CreateControls(true);
					DisplayOne(true, p, n);
					m_panelrowcount++;
				}
				else
				{
					DisplayOne(false, pnlDetailParts, n);
				}
			}

			if (m_repeating)
			{
				CreateEmptyRow();
				CreateLabels(true);
			}

			if (m_detail)
			{
				vscrDetail.Left						= tabTemplate.Width	- vscrDetail.Width;
			
				if (m_repeating)
				{
					pnlDetailMain.Left				= vscrDetail.Left -	pnlDetailMain.Width;
				}
				else
				{
					pnlDetailMain.Left				= 0;
					pnlDetailMain.Width				= vscrDetail.Left;
					// let it stick to the left border.
					pnlDetailMain.Anchor			|= System.Windows.Forms.AnchorStyles.Left;
				}

				pnlDetailMain.Height				= vscrDetail.Height;
			
				pnlDetailParts.Top					= 0;
				pnlDetailParts.Left					= 0;
				pnlDetailParts.Width				= pnlDetailMain.Width;
			}
			else
			{
				vscrDetail.Visible					= false;
				pnlDetailMain.Visible				= false;
			}

			if (m_repeating)
			{
				if (m_detail)
					vscrOverview.Left				= pnlDetailMain.Left - vscrOverview.Width;
				else
					vscrOverview.Left				= tabTemplate.Width	- vscrOverview.Width;

				hscrOverview.Width					= vscrOverview.Left;

				pnlOverviewMain.Location			= new Point(hscrOverview.Location.X, vscrOverview.Location.Y);						
				pnlOverviewMain.Size				= new Size(vscrOverview.Location.X,	hscrOverview.Location.Y);				

				pnlOverviewLabels.Location			= new Point(0, 0);
				pnlOverviewLabels.Height			= m_starttop;
				pnlOverviewLabels.BringToFront();

				pnlOverviewParts.Top				= m_starttop;
				pnlOverviewParts.Left				= 0;

				OrderControls(null);			
				//OverviewHScrollRecalc();
			}
			else
			{
				vscrOverview.Visible				= false;
				hscrOverview.Visible				= false;
				pnlOverviewMain.Visible				= false;
			}
			Resize();
			// If we have attributesets, we need to initialize its values
			// with the ids from the attributes.
			/* 29/5/06 fix this in another way, not sure how to yet 
						if (m_setattributespanel != null)
						{
							InitializeImmutableNameIdsForSets();
						} */
		}

		public Control FindControl(string name, Control.ControlCollection controls)
		{
			foreach (Control c in controls)
				if (c.Name == name)
					return c;
			return null;
		}

		public void Enter()
		{
			pnlAttr_Enter(FindControl("0", pnlOverviewParts.Controls), null);
		}
		
		private void DisplayOne(bool overview, Panel p, XmlNode Value)
		{
			// For overview part, create new panel, otherwise use detailpanel.
			// Create the panel and give it its name.

			foreach (XmlNode element in m_definitions.SelectNodes("element"))
			{
				XmlAttribute			name		= element.Attributes["name"];
				XmlAttribute			lines		= element.Attributes["lines"];
				XmlAttribute			size		= element.Attributes["size"];
				XmlAttribute			length		= element.Attributes["length"];
				XmlAttribute			type		= element.Attributes["type"];
				XmlAttribute			def_val		= element.Attributes["default"];
				XmlAttribute			detail		= element.Attributes["detail"];

				if (name == null) // || name.InnerText == "name")
					continue;
				if (overview)
				{	
					if (detail != null && detail.Value == "true") 
						continue;
				}
				else if (m_repeating)
				{
					if (detail == null || detail.Value == "false")
						continue;
				}

				Control					c			= FindControl(name.Value, p.Controls);
				System.Diagnostics.Debug.Assert(c != null, "Control named '" + name.Value + "' not found");
				if (c == null)
					continue;

				// Get the current string value, if available.
				string					stringValue	= "";
				XmlNode					valueNode	= Value.SelectSingleNode(name.Value);
				if (valueNode != null)
					stringValue						= valueNode.InnerText;
				else if (def_val != null)
					stringValue						= def_val.InnerText;
				
				if ((type		== null)		|| 
					(type.Value == "Name")		||
					(type.Value == "Number")	||
					(type.Value == "LoopField") ||
					(type.Value == "Text")		||
					(type.Value == "Combobox")	||
					(type.Value == "Table") )
				{
					c.Text							= stringValue;
				} 
				else if	(type.Value == "Order") 
				{
					//*****************************************************************************//
					System.Diagnostics.Debug.Assert(overview == true, "'Order' type controls only on Overview part please....");
					//*****************************************************************************//

					NumericUpDown		t			= c	as NumericUpDown;
					if (Value.ChildNodes.Count == 0)
					{
						// Find largest order value in all ordering controls
						Decimal			maxOrder	= 0;
						foreach (NumericUpDown n in m_OrderControls)
							if (n.Value > maxOrder)
								maxOrder			= n.Value;
						t.Text						= (maxOrder+1).ToString();
					}
					else
					{
						t.Text						= stringValue;
					}
				}
				else if (type.Value	== "Checkbox")
				{
					CheckBox			t			= c	as CheckBox;
					t.Checked						= (stringValue == "1");
				}
				else if (type.Value == "Guid")
				{
					if (stringValue == "")
					{
						Guid			g			= Guid.NewGuid();
						stringValue					= g.ToString();
						if (valueNode != null)
							valueNode.InnerText		= stringValue;
					}	
					c.Text							= stringValue;
				}
				else if (type.Value == "AttributeCombobox")
				{
					// Get the values in the list from all attribute panels
					// and put these in the combo. Select the right one 
					// according to the id of the attribute.
					/*
					ComboBox			t			= c	as ComboBox;
					XmlAttribute		id			= Value.Attributes["refid"];
					if (id == null)
					{
						id							= Value.OwnerDocument.CreateAttribute("refid");
						Value.Attributes.Append(id);
					}
					
					foreach (Control tb in m_attributespanel.NameControls)
					{
						if (tb.Text.Trim() == "")
							continue;

						t.Items.Add(tb.Text);
						// If no id connected yet, set it when value conforms
						if (id.Value == tb.Parent.Name)
							stringValue				= tb.Text;
						else if (id.Value == "" && stringValue == tb.Text)
							id.Value				= tb.Parent.Name;
					} */

					c.Text							= stringValue;
				}
				else if (type.Value == "AttributeSet")
				{
					// Keep in attributesets[i] an element 
					// where the attributes in the specific set
					// are kept. Keep in that element not the names of the
					// attributes but the number of the panel (the immutable id for
					// this session) to avoid renaming over and over again.
					/* XmlNode				x			= null;
					if (Value != null)
						x							= Value.SelectSingleNode(name.Value);
					if (x == null)
					{
						x							= Value.OwnerDocument.CreateElement(name.Value);
						Value.AppendChild(x);
					}
					*/
					c								= null;
				}
				else
				{
					// It must almost be a combobox with references to other types.
					c.Text							= stringValue;
				}
			}
		}

		private Panel CreateControls(bool overview)
		{
			// Keep 2 pixels on the left free, and keep track of the maximum
			// height of all controls on the panel, to be able to size the panel
			// right.
			int							left		= 0;
			int							maxheight	= 0;

			Panel						pnlAttr		= null;

			// For overview part, create new panel, otherwise use detailpanel.
			// Create the panel and give it its name.
			if (overview)
			{
				pnlAttr								= new Panel();
				pnlAttr.Name						= m_panelrowcount.ToString();
				pnlAttr.TabIndex					= m_panelrowcount;

				// Top will be calculated on basis of ordering and current position
				// of the scroll bar. Initially is it the currenttop
				pnlAttr.Top							= m_currenttop;
				pnlAttr.Left						= 0;
				// Width can increase when more space is needed, depending on the
				// number of elements defined per item.
				pnlAttr.Width						= 100;

				// On entering, eventually attributesets initialisation, 
				// and on leaving storing the results in the value.
				pnlAttr.Enter						+= new System.EventHandler(this.pnlAttr_Enter);
				pnlAttr.Leave						+= new System.EventHandler(this.pnlAttr_Leave);

				pnlOverviewParts.Controls.Add(pnlAttr);
			}
			else
			{
				pnlAttr								= pnlDetailParts;
			}

			int							defindex	= 0;

			foreach (XmlNode element in m_definitions.SelectNodes("element"))
			{
				XmlAttribute			name		= element.Attributes["name"];
				XmlAttribute			lines		= element.Attributes["lines"];
				XmlAttribute			size		= element.Attributes["size"];
				XmlAttribute			length		= element.Attributes["length"];
				XmlAttribute			type		= element.Attributes["type"];
				XmlAttribute			def_val		= element.Attributes["default"];
				XmlAttribute			detail		= element.Attributes["detail"];

				if (name == null) // || name.InnerText == "name")
					continue;
				if (overview)
				{	
					if (detail != null && detail.Value == "true") 
						continue;
				}
				else if (m_repeating)
				{
					if (detail == null || detail.Value == "false")
						continue;
				}

				Control					c			= null;

/*				// Get the current string value, if available.
				string					stringValue	= "";
				XmlNode					valueNode	= Value.SelectSingleNode(name.Value);
				if (valueNode != null)
					stringValue						= valueNode.InnerText;
				else if (def_val != null)
					stringValue						= def_val.InnerText;
 */

				if ((type		== null)	|| 
					(type.Value == "Name")  ||
					(type.Value == "Number")  ||
					(type.Value == "LoopField") ||
					(type.Value == "Text")  )
				{
					TextBox				t			= new TextBox();
					c								= t;
					int					nLines		= 0;
					if (lines != null)
						nLines						= Int32.Parse(lines.Value);
					else
						nLines						= 1;
				
					t.Multiline						= (nLines != 1);
					t.AcceptsReturn					= t.Multiline;
					t.Height						= nLines * 13 +	8;
					if (length != null)
						t.MaxLength					= Int32.Parse(length.Value);
					//t.Text							= stringValue;
					if (type != null && type.Value == "Name")
					{
						m_NameControls.Add(t);
						t.TextChanged				+= new System.EventHandler(this.txtName_TextLeave);
					}
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
						{
							ttt						= ttparts[0].ToString();
						}
						char			fieldsep	= TemplateGenerator.DEFAULT_FIELD_SEPERATOR;
						if (element.Attributes["fieldsep"] != null)
							fieldsep				= element.Attributes["fieldsep"].Value[0];
						ttt							= ttt +	fieldsep.ToString()	+ ttt;
						m_tt.SetToolTip(t, ttt);
					}
					t.TextChanged					+= new EventHandler(m_owner.Mark_Dirty);
				} 
				else if	(type.Value == "Order") 
				{
					//*****************************************************************************//
					System.Diagnostics.Debug.Assert(overview == true, "'Order' type controls only on Overview part please....");
					//*****************************************************************************//

					NumericUpDown		t			= new NumericUpDown();
					c								= t;
					t.TextAlign						= HorizontalAlignment.Right;
					/*
 					if (Value.ChildNodes.Count == 0)
					{
						// Find largest order value in all ordering controls
						Decimal			maxOrder	= 0;
						foreach (NumericUpDown n in m_OrderControls)
							if (n.Value > maxOrder)
								maxOrder			= n.Value;
						t.Text						= (maxOrder+1).ToString();
					}
					else
					{
						t.Text						= stringValue;
					}
					*/					
					t.Leave							+= new System.EventHandler(this.txtOrder_Leave);
					t.ValueChanged					+= new EventHandler(m_owner.Mark_Dirty);
					m_OrderControls.Add(c);
				}
				else if (type.Value	== "Checkbox")
				{
					CheckBox			t			= new CheckBox();
					t.ThreeState					= false;
					//t.Checked						= (stringValue == "1");
					t.Height						-= 4;
					c								= t;
					t.CheckedChanged				+= new EventHandler(m_owner.Mark_Dirty);
				}
				else if (type.Value == "Combobox")
				{
					// Get the values in the list from the child element
					// and put these in the combo
					ComboBox			t			= new ComboBox();
					t.DropDownStyle					= ComboBoxStyle.DropDownList;
					t.Items.Add("");
					foreach (XmlNode x in element.SelectNodes("element"))
						t.Items.Add(x.Attributes["name"].InnerText);
					//t.Text							= stringValue;
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
				}
				else if (type.Value == "Table")
				{
					// Get the values in the list from the cache with all tablenames
					// and put these in the combo
					ComboBox			t			= new ComboBox();
					t.DropDownStyle					= ComboBoxStyle.DropDownList;
					t.Sorted						= true;
					t.Items.Add("");
					t.Items.AddRange(TemplateCache.Instance().GetTypenamesList("TableDefinition"));
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
				}
				else if (type.Value == "Guid")
				{
					// When not set, generate new guid
					TextBox				t			= new TextBox();
					t.Enabled						= false;
					t.BackColor						= System.Drawing.SystemColors.Window;
					t.ForeColor						= System.Drawing.SystemColors.ControlText;
					c								= t;
				}

				else if (type.Value == "AttributeCombobox")
				{
					/*
					// Get the values in the list from all attribute panels
					// and put these in the combo. Select the right one 
					// according to the id of the attribute.
					ComboBox			t			= new ComboBox();
					t.Sorted						= true;
					t.DropDownStyle					= ComboBoxStyle.DropDownList;
					t.Items.Add("");

					foreach (Control tb in m_attributespanel.NameControls)
					{
						if (tb.Text.Trim() == "")
							continue;

						t.Items.Add(tb.Text);
						// If no id connected yet, set it when value conforms
						if (id.Value == tb.Parent.Name)
							stringValue				= tb.Text;
						else if (id.Value == "" && stringValue == tb.Text)
							id.Value				= tb.Parent.Name;
					}
					//t.Text							= stringValue;
					c								= t;
					m_NameControls.Add(t);
					t.SelectedIndexChanged			+= new System.EventHandler(this.txtName_TextLeave);
					t.Leave							+= new System.EventHandler(this.txtName_UpdateTextId);
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
*/
				}
				else if (type.Value == "AttributeSet")
				{
					// Keep in attributesets[i] an element 
					// where the attributes in the specific set
					// are kept. Keep in that element not the names of the
					// attributes but the number of the panel (the immutable id for
					// this session) to avoid renaming over and over again.
					/* XmlNode				x			= null;
					if (Value != null)
						x							= Value.SelectSingleNode(name.Value);
					if (x == null)
					{
						x							= Value.OwnerDocument.CreateElement(name.Value);
						Value.AppendChild(x);
					}
					*/
					c								= null;
				}
				else
				{
					// It must almost be a combobox with references to other types.
					// Find the right directory
					ComboBox			t			= new ComboBox();
					t.DropDownStyle					= ComboBoxStyle.DropDownList;
					t.Sorted						= true;
					t.Items.Add("");
					t.Items.AddRange(TemplateCache.Instance().GetTypenamesList(type.Value));
					//t.Text							= stringValue;

					TemplateMain.Instance().CreateLinkContextMenu(t, type.Value);
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
				}

				if (defindex >= m_AttributeControls.Count)
					m_AttributeControls.Add(new ArrayList());

				if (c != null)
				{
					/*
					if (m_LabelControls.Count > defindex)
					{
						Label			label		= m_LabelControls[defindex]	as Label;
						if (label != null)
							left					= label.Left;
					}
					*/

					c.Name							= name.Value;
					ArrayList			al			= m_AttributeControls[defindex]	as ArrayList;
					al.Add(c);

					if (overview)
					{
						c.Top						= 0;//2;
						c.Left						= left;
						int				sizeval		= 10;
						if (size != null)
							sizeval					= int.Parse(size.Value);
						else if (length != null)
							sizeval					= int.Parse(length.Value);

						c.Width						= sizeval *	8 +	4;
						pnlAttr.Controls.Add(c);
						if (al.Count > m_definitions_fixed && 
							hscrOverview != null &&
							al.Count - m_definitions_fixed <= hscrOverview.Value)
							c.Visible				= false;
						else
							left					+= /*4 +*/ c.Width;

						if (c.Top + c.Height > maxheight)
							maxheight				= c.Top	+ c.Height;

						// Make sure it is wide enough
						if (pnlAttr.Width < left)
							pnlAttr.Width			= left + 2;
					}
					else
					{
						c.Top						= m_currenttop;
						c.Left						= m_labelwidth + 8;
						c.Width						= pnlAttr.Width	- c.Left;
						c.Anchor					= AnchorStyles.Left	| AnchorStyles.Right | AnchorStyles.Top;
				
						pnlAttr.Controls.Add(c);
				
						m_currenttop				+= c.Height	+ 4;
					}
				}

				// increase index of processed attribute definition
				defindex++;
			}

			if (overview)
			{
				pnlAttr.Height						= maxheight;// + 2;
				pnlAttr.Top							= m_currenttop;// -	(maxheight + 2)	* (m_VScroll.Value);

				/*
				 * 	// Empty <attributes/> element does not count too, so test for > 1
				 *  SetPanelVisible(pnlAttr, (Value.ChildNodes.Count > 1));
				 *  m_panel.Controls.Add(pnlAttr);
				 */
				m_currenttop						+= pnlAttr.Height;
			}
			else
			{
				pnlAttr.Height						= m_currenttop;
			}

			return pnlAttr;
		}

		public void InitializeImmutableNameIdsForSets()
		{	/*
			string						name_element= "name";
			string[]					names		= m_attributespanel.GetNamePanels();

			// For each element in the x, find the immutable id and
			// add that to the element.
			foreach (XmlNode x in m_values.SelectNodes("sets/attributes/attribute"))
			{
				if (x == null)
					continue;

				foreach (XmlNode el in x.SelectNodes("attribute/" + name_element))
				{
					// Find the attribute with the given name
					int					idx			= Array.IndexOf(names, el.InnerText);
					if (idx >= 0)
					{
						XmlAttribute	id			= x.Attributes["refid"];
						if (id == null)
						{
							id						= x.OwnerDocument.CreateAttribute("refid");
							x.Attributes.Append(id);
						}
						id.Value					= idx.ToString();
					}
				}
			}*/
		}
		/// <summary>
		/// Return a list with the values of the names, each index value holds
		/// the name of the panel holding the 'name'.
		/// Example: return in index 3 the value of the name control on panel named '3'.
		/// </summary>
		/// <returns>list with name values</returns>
		public string[] GetNamePanels()
		{
			string[]					result		= new string[m_panelrowcount];
			
			foreach (TextBox t in m_NameControls)
			{
				int						pidx		= int.Parse(t.Parent.Name);
				result[pidx]						= t.Text.Trim();
			}

			return result;
		}

		public void SetPanelVisible(Panel p, bool visible)
		{
			int							index		= 0;
			if (hscrOverview != null)
				index								= hscrOverview.Value;
			
			int							i;

			if (visible)
			{
				// visible all fixed position attribute definitions
				for (i = 0; i < m_definitions_fixed && i < m_AttributeControls.Count; i++)
					foreach (Control c in (m_AttributeControls[i] as ArrayList))
						if (c.Parent == p)
							c.Visible				= true;

				// visible all higher visible controls
				for (i = m_definitions_fixed + index; i < m_AttributeControls.Count; i++)
					foreach (Control c in (m_AttributeControls[i] as ArrayList))
						if (c.Parent == p)
							c.Visible				= true;
			}
			else
			{
				Control					control		= null;
				if (m_NameControls.Count > 0)
					control							= m_NameControls[int.Parse(p.Name)]	as Control;
				else
					// go looking for the index control.
					control							= m_OrderControls[int.Parse(p.Name)] as	Control;

				foreach (Control c in p.Controls)
					if (c != control)
						c.Visible					= false;
			}
		}

		/// <summary>
		/// Add captions to the attribute list panel
		/// </summary>
		private void CreateLabels(bool overview)
		{
			int							adefidx		= 0;
            
			foreach (XmlNode a in m_definitions.SelectNodes("element"))
			{
				string					strCaption;

				XmlAttribute			name		= a.Attributes["name"];
				XmlAttribute			detail		= a.Attributes["detail"];

				if (name == null) // || name.InnerText == "name")
					continue;
				if (overview)
				{	
					if (detail != null && detail.Value == "true") 
						continue;
				}
				else if (m_repeating)
				{
					if (detail == null || detail.Value == "false")
						continue;
				}

				if (a.Attributes["caption"] != null)
					strCaption						= a.Attributes["caption"].InnerText;
				else
					strCaption						= TemplateUtil.Instance().InventCaptionForName(a.Attributes["name"].InnerText);

				Label					lblName		= new Label();
				lblName.Text						= strCaption;
                
				string					strToolTip	= "";
				if (a.Attributes["tooltip"] != null)
					strToolTip						= a.Attributes["tooltip"].Value;
				strToolTip							+= "(" + a.Attributes["name"].Value	+ ")";
				m_tt.SetToolTip(lblName, strToolTip);
				lblName.Name						= "label_" + a.Attributes["name"].InnerText.Trim();

				// Walk all controls created for the attributedefinition
				// and find out the largest rectangle containing all controls
				int						leftmost	= int.MaxValue;
				int						rightmost	= 0;
				int						topmost		= int.MaxValue;
				int						bottommost	= 0;
				foreach (Control c in (m_AttributeControls[adefidx] as ArrayList))
				{
					if (leftmost > c.Left)
						leftmost					= c.Left;
					if (rightmost < c.Left + c.Width)
						rightmost					= c.Left + c.Width;
					if (topmost > c.Top)
						topmost						= c.Top;
					if (bottommost < c.Top + c.Height)	
						bottommost					= c.Top	+ c.Height;
				}
				if (rightmost > 0) // any control found?
				{
					if (overview)
					{
						lblName.Left				= leftmost;	
						lblName.Width				= rightmost	- leftmost;
						lblName.Top					= 0;
						lblName.Height				= m_starttop - 2;
						pnlOverviewLabels.Controls.Add(lblName);
						if (pnlOverviewLabels.Width < rightmost)
							pnlOverviewLabels.Width	= rightmost;
					}
					else
					{
						lblName.Left				= 0;
						lblName.Width				= m_labelwidth;
						lblName.Top					= topmost;
						lblName.Height				= bottommost - topmost;
						pnlDetailParts.Controls.Add(lblName);
					}
				}
				else
				{
					lblName							= null;	// forget it.
				}
				m_LabelControls.Add(lblName);
			
				adefidx++;
			}
		}

		private void CreateEmptyRow()
		{
			// And add an empty line
			XmlNode						newvalue	= m_values.OwnerDocument.CreateElement(m_elementname);
			
			System.Diagnostics.Debug.Assert(newvalue != null, "Shit newvalue is empty");

			m_values.AppendChild(newvalue);
			Panel						newpanel	= CreateControls(true);
			DisplayOne(true, newpanel, newvalue);
			m_panelrowcount++;
			m_panelheight							= newpanel.Height;
			pnlOverviewParts.Height					= m_starttop + m_panelrowcount * m_panelheight;

			if (pnlOverviewParts.Width < newpanel.Width)
				pnlOverviewParts.Width				= newpanel.Width;

			OverviewVScrollRecalc();
		}

		/// <summary>
		/// position the 'index' attributedefinition to the leftmost position next to the
		/// name control. Place the higher controls to the right, and make the lower controls
		/// invisible.
		/// </summary>
		/// <param name="index"></param>
		public void OverviewHScrollTo(int index)
		{
			// previous left contains previous left position of the caption
			// of the attributedef, left contains to be left position.
			// Controls for the attribute are repositioned according to the
			// difference between left and previousleft
			int							left		= 0;
			int							previous_left;
			Label						label;
			int							i;

			// skip all fixed position attribute definitions
			for (i = 0; i < m_definitions_fixed; i++)
			{
				label								= m_LabelControls[i] as	Label;
				left								= label.Left + label.Width;	// + 4;
			}

			// invisible all controls of lower attributes
			for (; i < m_definitions_fixed + index; i++)
			{
				foreach (Control c in (m_AttributeControls[i] as ArrayList))
					c.Visible						= false;
				label								= m_LabelControls[i] as	Label;
				label.Visible						= false;
			}

			// position and visible all other controls
			System.Diagnostics.Debug.Assert(m_NameControls.Count > 0, "No Name-controls on panel found!");
			if (m_NameControls.Count > 0)
			{
				for (; i < m_AttributeControls.Count; i++)
				{
					label							= m_LabelControls[i] as	Label;
					if (label != null)
					{
						previous_left				= label.Left;
						foreach (Control c in (m_AttributeControls[i] as ArrayList))
						{
							c.Left					= c.Left - previous_left + left;
							// make it visible when corresponding name-control contains text
							Control		namecontrol	= (m_NameControls[int.Parse(c.Parent.Name)]	as Control);
							c.Visible				= (namecontrol == c	|| namecontrol.Text	!= "");
						}
						label.Left					= left;
						label.Visible				= true;
						left						+= label.Width ;//+	4;
					}
				}
			}
		}

		/// <summary>
		/// Read all controls from the panel and put the values
		/// in the corresponding element under the Value element.
		/// Read the value of the control and put in the child node below the
		/// given Value. If no child node with the same name available, create it first.
		/// </summary>
		/// <param name="p">Panel to process all control of</param>
		/// <param name="Value">Compound element with values</param>
		public static void ReadPanel(Panel p, XmlNode Value)
		{
			foreach (Control c in p.Controls)
				ReadControl(c, Value);
		}
		/// <summary>
		/// Read the value of the control and put in the child node below the
		/// given Value. If no child node with the same name available, create it first.
		/// </summary>
		/// <param name="c">Control to process</param>
		/// <param name="Value">Compound element with values</param>
		public static void ReadControl(Control c, XmlNode Value)
		{
			if (c is Label)
				return;

			if (c.Name == "")
				return;

			XmlNode						v			= Value.SelectSingleNode(c.Name);
			if (v == null)						
			{
				v									= Value.OwnerDocument.CreateElement(c.Name);
				Value.AppendChild(v);
			}
			if (c is TextBox || c is ComboBox)
				v.InnerText							= c.Text;
			else if (c is CheckBox)
				v.InnerText							= ((c as CheckBox).Checked)	? "1" :	"0";
			else if (c is NumericUpDown)
				v.InnerText							= (c as	NumericUpDown).Value.ToString();
		}


		private void txtName_UpdateTextId(object sender, System.EventArgs e)
		{
			return;
/*
			Control						c			= sender as	Control;
			string						t			= c.Text;

			XmlNode						val			= m_Value[int.Parse(c.Parent.Name)]	as XmlNode;
			XmlAttribute				id			= val.Attributes["refid"];
			if (t != "")
			{
				// Find the id belonging to this name from the attributes panel
				int						idx			= Array.IndexOf(m_attributespanel.GetNamePanels(), t);
				id.Value							= idx.ToString();
			}
			else
			{
				id.Value							= "";
			}
*/			
		}

		private void txtName_TextLeave(object sender, System.EventArgs e)
		{
			Control						t			= sender as	Control;
			SetPanelVisible(t.Parent as Panel, t.Text.Trim() != "");
			// When last row on the list gets a new, add an empty one below.
			if (t.Text.Trim() == "")
				return;
			
			if (m_OrderControls.Count == 0)
				return;
			
			int							idx			= int.Parse(t.Parent.Name);
			if (m_OrderControls.Count <= idx)
				return;

			NumericUpDown				n			= m_OrderControls[idx] as NumericUpDown;
			// ordering control found, and contains last number... 
			// set focus to previously invisible field
			// ... add new row
			if (n != null && n.Value == m_panelrowcount)
			{
				CreateEmptyRow();
				/*					Control next_in_taborder = t.Parent.GetNextControl(t,true); //
										if (next_in_taborder != null)
										{
											if (next_in_taborder.Visible == false)
												HScrollTo(0);
											next_in_taborder.Focus();
										}
				}*/				
			}
		}

		/// <summary>
		/// Remove all values from the 'source' element where the Name is empty.
		/// This action will invalidate the panel and make it useless for further
		/// processing. Except by supplying new values in the Fill.
		/// </summary>
		public void CleanupValues()
		{
			for (int i = 0; i < m_NameControls.Count; i++)
			{
				Control					c			= m_NameControls[i]	as Control;
				if (c.Text.Trim() != "")
					continue;
				
				// Remove the value from its parent.
				XmlNode					val			= m_Value[i] as	XmlNode;
				val.ParentNode.RemoveChild(val);
			}

			/*			// Check all referring attributes
						if (m_attributespanel != null && m_values != null)
						{
                            string[]    definitive_names
                                                    = m_attributespanel.GetNamePanels();
							foreach (XmlNode x in m_values.SelectNodes(m_elementname + "/attributes/attribute"))
							{
                                XmlAttribute 
										a			= x.Attributes["refid"];
								if (a == null)
									continue;
								if (a.Value == "")
								{
									x.ParentNode.RemoveChild(x);
									continue;
								}

								XmlNode	n			= x.SelectSingleNode("name");
								if (n == null)
								{
									n				= x.OwnerDocument.CreateElement("name");
									x.AppendChild(n);
								}
								n.InnerText			= definitive_names[int.Parse(a.Value)];
							}
						} */
			m_values								= null;
		}

		/// <summary>
		/// Methode alleen gemaakt voor de inventlabels functie die de labels moet bewaren 
		/// </summary>
		public void ReadAllPanels()
		{
			foreach (Control c in pnlOverviewParts.Controls)
			{
				Panel					p			= c	as Panel;
				if (p != null)
					pnlAttr_Leave(p, null);
			}
		}
		private void pnlAttr_Leave(object sender, System.EventArgs e)
		{
			Panel						p			= sender as	Panel;
			XmlNode						nodeToAddTo	= m_Value[int.Parse(p.Name)] as	XmlNode;

			ReadPanel(p, nodeToAddTo);
		}

		private void pnlAttr_Enter(object sender, System.EventArgs e)
		{
			Panel						p			= sender as	Panel;
			
			if (m_previous_entered_panel != null)
				m_previous_entered_panel.BackColor	= SystemColors.Control;
			p.BackColor								= SystemColors.Highlight;
			m_previous_entered_panel				= p;

			/*			if (p.Top + p.Height > m_panel.Parent.Height)
							// Noho, it is not completely visible. Scroll to 2 lower to get context visible.
							VScrollTo(p.TabIndex);
			 */
			//if (m_setattributespanel == null)
				return;
/*
			// Cleanup the previous setting of the set attributes.
			m_setattributespanel.CleanupValues();

			XmlNode						val			= m_Value[int.Parse(p.Name)] as	XmlNode;
			XmlNode						selattrs	= val.SelectSingleNode("attributes");

			System.Diagnostics.Debug.Assert(selattrs != null,"Shit selectedattributes is empty");

			m_setattributespanel.Fill(selattrs);
*/			
		}

		private void txtOrder_Leave(object sender, System.EventArgs e)
		{
			NumericUpDown				curOrder	= sender as	NumericUpDown;

			bool						emptyValue	= curOrder.Text	== "";
			/*			try
						{
							Decimal		val			= curOrder.Value;
							emptyValue				= false;
						}
						catch
						{
							emptyValue				= true;
						}
			*/
			//not sure : SetPanelVisible(curOrder.Parent as Panel, !emptyValue);
			// Renumber controls with numbers higher than this one
			// if an order control with the same number exists.
			OrderControls(curOrder);

			// Set the panels on the right position. Depending on the value
			// of the current ordering control.
			// First assume the current panel can stay at its current position.
			/*			int							pnlheight	= curOrder.Parent.Height;

						// If its not visible, get the positioning right on the enter of another panel.
						if (curOrder.Parent.Visible)
						{
							// if the current top > where it could be, it will have to move.
							if (curOrder.Parent.TabIndex * pnlheight < curOrder.Parent.Top)
							{
					
							}
						}
						else
						{

						}
			 */

			// When last row on the list gets a new, add an empty one below.
			// if contains last number... 
			// set focus to previously invisible field
			// ... add new row
			if (!emptyValue && curOrder.Value == m_panelrowcount)
			{
				CreateEmptyRow();
			}
		}

		private void OrderControls(NumericUpDown curOrder)
		{
			// Start on the top
			int							currenttop	= 0; //m_starttop;
			int							tabindex	= 0;

			ArrayList					controlsToDo= m_OrderControls.Clone() as ArrayList;

			// Find smallest ordering ID
			while (controlsToDo.Count > 0)
			{
				Decimal					min_val		= Decimal.MaxValue;
				NumericUpDown			min_ctr		= null;

				// find order control with smallest number and
				// not yet processed. Take at least an available control
				// to be sure to process the invisible panels too.
				if (curOrder != null && tabindex + 1 == curOrder.Value)
				{
					min_ctr							= curOrder;
				}
				else
				{
					foreach (NumericUpDown n in controlsToDo)
					{
						if (n.Value < min_val && /*(n.Visible || curOrder == null) &&*/ n != curOrder) 
						{
							min_val					= n.Value;
							min_ctr					= n;
						} 
						else if (min_ctr == null && n != curOrder)
						{
							min_ctr					= n;
							min_val					= n.Value;
						}
					}
				}
				// when current control does contain a 'weird' value, select it now
				if (min_ctr == null)
					min_ctr							= curOrder;

				// Set the smallest controls position for all controls with the
				// same index number.
				min_ctr.Value						= tabindex + 1;
				min_ctr.Parent.Top					= currenttop;
				min_ctr.Parent.TabIndex				= tabindex++;
				min_ctr.Parent.Visible				= true;
				currenttop							= currenttop + min_ctr.Parent.Height;

				controlsToDo.Remove(min_ctr);

				//Update ordering controls value in the Xml
				int						idx			= int.Parse(min_ctr.Parent.Name);
				//System.Diagnostics.Debug.Assert(idx < m_Value.Count, "About to generate array out of bounds");
				if (idx < m_Value.Count)
					ReadControl(min_ctr, m_Value[idx] as XmlNode);
			}
			// Optionally update the vertical scrollbars value.
			// When the position of the panel containing the curOrder control is visible
			// update the value of the scrollbar to 0 (since we started with the 
			// panel to show, and value of scrollbar indicates top-row-shown.
			/*			if (m_VScroll.Value != 0)
							m_VScroll.Value			= 0;

						// When panel of curOrder control not visible, position it somewhere visible
						if ((curOrder != null) && (curOrder.Parent.Top + curOrder.Parent.Height > m_panel.Parent.Height))
							// Noho, it is not completely visible. Scroll to 2 lower to get context visible.
							VScrollTo((int)curOrder.Value-2);
			*/

		}

		/// <summary>
		/// Process resizing of the container panel. 
		/// Recalculate the available scrollbars.
		/// </summary>
		public void Resize()
		{
			if (m_repeating)
			{
				OverviewVScrollRecalc();
				OverviewVScrollTo(vscrOverview.Value);
				OverviewHScrollRecalc();
			}

			if (m_detail)
			{
				DetailVScrollRecalc();
				DetailVScrollTo(vscrDetail.Value);
				if (!m_repeating)
				{
					//resize the pnl to the tab. Visibility is not key, because the tab can be hidden
					pnlDetailMain.Width				= tabTemplate.Width	- (vscrDetail.Maximum != 1 ? vscrDetail.Width :	0);	
				}
			}
		}

		/// <summary>
		/// Make the 'pos' row(panel) the topmost one.
		/// </summary>
		/// <param name="pos"></param>
		public void OverviewVScrollTo(int pos)
		{
			pnlOverviewParts.Top					= -pos + m_starttop;
			/*
			foreach (Control c in m_panel.Controls)
			{
				if (c is Panel) 
				{
					if (c.TabIndex >= pos)
					{
						c.Location					= new Point(0, m_starttop +	(c.TabIndex	- pos) * c.Height);
						c.Visible					= true;
					}
					else
					{
						c.Visible					= false;
					}
				}
			}
			*/
		}
		/// <summary>
		/// Make the 'pos' row(panel) the topmost one.
		/// </summary>
		/// <param name="pos"></param>
		public void DetailVScrollTo(int pos)
		{
			pnlDetailParts.Top						= -pos;
		}

		private void OverviewVScrollRecalc()
		{
			if (vscrOverview == null)
				return;

			// VScrollbar recalculation
			if (m_panelrowcount > 5)
			{
				vscrOverview.Minimum				= 0;
				vscrOverview.Maximum				= pnlOverviewParts.Height;
				vscrOverview.Enabled				= true;
				vscrOverview.SmallChange			= m_panelheight;
				vscrOverview.LargeChange			= vscrOverview.Height;
				if (vscrOverview.Value > vscrOverview.Maximum)
					vscrOverview.Value				= vscrOverview.Maximum;
				if (vscrOverview.Value < vscrOverview.Minimum)
					vscrOverview.Value				= vscrOverview.Minimum;

			}
			else
			{
				vscrOverview.Value					= 0;
				vscrOverview.Enabled				= false;
			}
		}

		private void OverviewHScrollRecalc()
		{
			if (hscrOverview == null)
				return;

			// HScrollbar recalculation
			if (m_definitions_fixed < m_definitions_count - 1)
			{
				hscrOverview.Minimum				= 0;
				hscrOverview.Maximum				= m_definitions_count -	m_definitions_fixed	- 1;
				hscrOverview.LargeChange			= 2;
				hscrOverview.SmallChange			= 1;
				hscrOverview.Enabled				= true;
			}
			else
			{
				hscrOverview.Enabled				= false;
			}
		}
		private void DetailVScrollRecalc()
		{
			// VScrollbar recalculation
			if (pnlDetailParts.Height > pnlDetailMain.Height)
			{
				vscrDetail.Minimum					= 0;
				vscrDetail.Maximum					= pnlDetailParts.Height;
				vscrDetail.Visible					= true;
				vscrDetail.SmallChange				= 30;
				vscrDetail.LargeChange				= vscrDetail.Height;
				if (vscrDetail.Value > vscrDetail.Maximum)
					vscrDetail.Value				= vscrDetail.Maximum;
				if (vscrDetail.Value < vscrDetail.Minimum)
					vscrDetail.Value				= vscrDetail.Minimum;

			}
			else
			{
				vscrDetail.Value					= 0;
				vscrDetail.Maximum					= 1;
				vscrDetail.Visible					= false;
			}
		}

		private void mnuOpenLink_Click(object sender, System.EventArgs e)
		{
		
		}
		private void vscrOverview_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				OverviewVScrollTo(e.NewValue);
		}
		private void hscrOverview_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				OverviewHScrollTo(e.NewValue);
		}
		private void vscrDetail_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
				DetailVScrollTo(e.NewValue);
		}
		private void tabTemplate_Resize(object sender, System.EventArgs e)
		{
			this.Resize();
		}

		private void pnlOverviewMain_Resize(object sender, System.EventArgs e)
		{
		}
		private void pnlDetailMain_Resize(object sender, System.EventArgs e)
		{
			return;
/*
			if (tabTemplate.Height >= pnlDetailParts.Height)
			{
				vscrDetail.Visible					= false;
				pnlDetailMain.Top					= 0;
			}
			else
			{
				vscrDetail.Visible					= true;
//				pnlGeneral.Width = tabGeneral.Width - this.gVScroll.Width;
				vscrDetail.Minimum					= 0;
				int						nPages		= 1	+ (int)	Math.Floor((pnlDetailParts.Height -	1) / pnlDetailMain.Height);
				vscrDetail.Maximum					= nPages * pnlDetailMain.Height;
				vscrDetail.LargeChange				= pnlDetailMain.Height;
				if (vscrDetail.LargeChange > 100)
					vscrDetail.SmallChange			= vscrDetail.LargeChange / 10;
				else
					vscrDetail.SmallChange			= 10;

				if (pnlDetailParts.Top < -vscrDetail.Maximum)
				{
					pnlDetailParts.Top				= -	vscrDetail.Maximum;
				}
				
				vscrDetail.Value					= -	pnlDetailParts.Top;
			}
*/
		}

	}
}
