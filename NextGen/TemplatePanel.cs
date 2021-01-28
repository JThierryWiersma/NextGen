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
	/// A TemplatePanel contains one or more panels on which controls are
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
	public class TemplatePanel
	{
		private	int						m_panelrowcount;
		private	int						m_panelheight;			// height of one panel.
		private	int						m_currenttop;
		private	XmlNode					m_values;
		private	XmlNode					m_definitions;
		private	int						m_starttop	= 20;
		private	int						m_definitions_fixed;
		private	int						m_definitions_count;
		private	string					m_elementname;
		// one for each attribute
		// on position x the values for panel named x
		private	ArrayList				m_NameControls;	
		private	ArrayList				m_OrderControls;	
		private	ArrayList				m_Value;				// XmlNode with	the	value
		// one for each attributedefinition
		private	ArrayList				m_LabelControls;
		private	ArrayList				m_AttributeControls;

		private	TemplatePanel			m_attributespanel;
		private	TemplatePanel			m_setattributespanel;
		private	TemplateForm			m_owner;

		private	Panel					m_panel;
		private	ScrollBar				m_HScroll;
		private	ScrollBar				m_VScroll;
		private	Panel					m_previous_entered_panel;
		private	Color					m_previous_entered_color;
		private	ToolTip					m_tt;

		public TemplatePanel(TemplateForm owner, string elementname, XmlNode definitions, XmlNode values, Panel panel, ScrollBar h, ScrollBar v)
			: this(owner, elementname, definitions, values, panel, h, v, null, null)
		{
		}
		public TemplatePanel(TemplateForm owner, string elementname, XmlNode definitions, XmlNode values, Panel panel, ScrollBar h, ScrollBar v, 
			TemplatePanel setattributespanel, TemplatePanel attributespanel)
		{
			m_owner									= owner;
			m_definitions							= definitions;
			m_values								= values;
			m_panel									= panel;
			m_HScroll								= h;
			m_VScroll								= v;
			m_NameControls							= new ArrayList();
			m_OrderControls							= new ArrayList();
			m_Value									= new ArrayList();
			m_LabelControls							= new ArrayList();
			m_AttributeControls						= new ArrayList();
			m_elementname							= elementname;
			m_setattributespanel					= setattributespanel;
			m_attributespanel						= attributespanel ?? this; // in poging om attributescombo op attributes panel te krijgen
			m_tt									= new ToolTip();

			m_panel.DragDrop						+= new System.Windows.Forms.DragEventHandler(this.pnlPanel_DragDrop);
			// use same enter for attribute and panel.
			m_panel.DragEnter						+= new System.Windows.Forms.DragEventHandler(this.pnlAttr_DragEnter);
			m_panel.AllowDrop						= true;
		}
	
		public ArrayList NameControls
		{
			get
			{
				return m_NameControls;
			}
		}
/*		public XmlNode values
		{
			get
			{
				return m_values;
			}
		} */
		public XmlNode definitions
		{
			get
			{
				return m_definitions;
			}
		}
		public void Fill(XmlNode value)
		{
			m_values								= value;
			System.Diagnostics.Debug.Assert(m_values != null,"Shit m_values is empty");
			Fill();
		}

		public void Fill()
		{
			m_currenttop							= m_starttop;
			m_panelrowcount							= 0;
			
			m_panel.Controls.Clear();
			m_NameControls.Clear();
			m_AttributeControls.Clear();
			m_LabelControls.Clear();
			m_OrderControls.Clear();
			m_Value.Clear();

			m_definitions_count						= m_definitions.ChildNodes.Count;
			XmlNode						fixedattr	= m_definitions.Attributes["fixed"];
			if (fixedattr != null)
				m_definitions_fixed					= int.Parse(fixedattr.Value);
			else
				m_definitions_fixed					= 2;

			foreach (XmlNode n in m_values.SelectNodes(m_elementname))
			{
				FillOne(n);
				m_panelrowcount++;
			}
			CreateEmptyRow();
			CreateLabels();

			OrderControls(null);			
			RecalcHScrollbar();

			// If we have attributesets, we need to initialize its values
			// with the ids from the attributes.
			if (m_setattributespanel != null)
			{
				InitializeImmutableNameIdsForSets();
			}

//			pnlAttr_Enter(TemplateUtil.FindControl("0", m_panel.Controls), null);
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
			pnlAttr_Enter(FindControl("0", m_panel.Controls), null);
		}
		/// <summary>
		/// Create a panel with the contents filled from the given Value XmlNode
		/// The name of the panel is the sequence-number of the panel 
		/// (derived from the number of panels current in this form).
		/// The name will be fixed and will never change.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		private Panel FillOne(XmlNode Value)
		{
			// Keep 2 pixels on the left free, and keep track of the maximum
			// height of all controls on the panel, to be able to size the panel
			// right.
			int							left		= 2;
			int							maxheight	= 0;

			// Create the panel and give it its name.
			Panel						pnlAttr		= new Panel();
			pnlAttr.Name							= m_panelrowcount.ToString();
			pnlAttr.TabIndex						= m_panelrowcount;

			// Top will be calculated on basis of ordering and current position
			// of the scroll bar. Initially is it the currenttop
			pnlAttr.Top								= m_currenttop;
			pnlAttr.Left							= 0;
			// Width can increase when more space is needed, depending on the
			// number of elements defined per item.
			pnlAttr.Width							= 100;

			// On entering, eventually attributesets initialisation, 
			// and on leaving storing the results in the value.
			pnlAttr.Enter							+= new System.EventHandler(this.pnlAttr_Enter);
			pnlAttr.Leave							+= new System.EventHandler(this.pnlAttr_Leave);
			pnlAttr.MouseDown						+= new System.Windows.Forms.MouseEventHandler(this.pnlAttr_MouseDown);
			pnlAttr.DragEnter						+= new System.Windows.Forms.DragEventHandler(this.pnlAttr_DragEnter);
			pnlAttr.DragLeave						+= new System.EventHandler(this.pnlAttr_DragLeave);
			pnlAttr.DragDrop						+= new System.Windows.Forms.DragEventHandler(this.pnlAttr_DragDrop);
			pnlAttr.AllowDrop						= true;

			//pnlAttr.BorderStyle = BorderStyle.FixedSingle;

			// Store the value we're displaying on this panel on the x-th element
			// in the m_Value list.
			m_Value.Add(Value);
			int							defindex	= 0;

			foreach (XmlNode element in m_definitions.SelectNodes("element"))
			{
				XmlAttribute			name		= element.Attributes["name"];
				XmlAttribute			lines		= element.Attributes["lines"];
				XmlAttribute			size		= element.Attributes["size"];
				XmlAttribute			length		= element.Attributes["length"];
				XmlAttribute			type		= element.Attributes["type"];
				XmlAttribute			def_val		= element.Attributes["default"];

				if (name == null) // || name.InnerText == "name")
					continue;

				Control					c			= null;

				// Get the current string value, if available.
				string					stringValue	= "";
				XmlNode					valueNode	= Value.SelectSingleNode(name.Value);
				if (valueNode != null)
					stringValue						= valueNode.InnerText;
				else if (def_val != null)
					stringValue						= def_val.InnerText;

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
					t.Text							= stringValue;
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
							ttt						= String.Join(partsep.ToString(), ttstrings);
						}
						else if (ttparts.Count > 0)
						{
							ttt						= ttparts[0].ToString();
						}
                        else
                        {
                            ttt = "";
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
					NumericUpDown		t			= new NumericUpDown();
					t.Maximum						= 999;
					c								= t;
					t.TextAlign						= HorizontalAlignment.Right;
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
					t.Leave							+= new System.EventHandler(this.txtOrder_Leave);
					t.ValueChanged					+= new EventHandler(m_owner.Mark_Dirty);
					m_OrderControls.Add(c);
				}
				else if (type.Value	== "Checkbox")
				{
					CheckBox			t			= new CheckBox();
					t.ThreeState					= false;
					t.Checked						= (stringValue == "1");
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
					t.Text							= stringValue;
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
					t.Text							= stringValue;
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
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

					XmlAttribute		id			= Value.Attributes["refid"];
					if (id == null)
					{
						id							= Value.OwnerDocument.CreateAttribute("refid");
						Value.Attributes.Append(id);
					}
					//t.Items.Add("");
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

					t.Text							= stringValue;
					c								= t;
					if (m_attributespanel != this)
					{
						m_NameControls.Add(t);
						t.SelectedIndexChanged += new System.EventHandler(this.txtName_TextLeave);
						t.Leave += new System.EventHandler(this.txtName_UpdateTextId);
					}
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
				}
				else if (type.Value == "AttributeSet")
				{
					// Keep in attributesets[i] an element 
					// where the attributes in the specific set
					// are kept. Keep in that element not the names of the
					// attributes but the number of the panel (the immutable id for
					// this session) to avoid renaming over and over again.
					XmlNode				x			= null;
					if (Value != null)
						x							= Value.SelectSingleNode(name.Value);
					if (x == null)
					{
						x							= Value.OwnerDocument.CreateElement(name.Value);
						Value.AppendChild(x);
					}
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
					t.Text							= stringValue;

					TemplateMain.Instance().CreateLinkContextMenu(t, type.Value);
					c								= t;
					t.SelectedValueChanged			+= new EventHandler(m_owner.Mark_Dirty);
				}

				if (defindex >= m_AttributeControls.Count)
					m_AttributeControls.Add(new ArrayList());

				if (c != null)
				{
					if (m_LabelControls.Count > defindex)
					{
						Label			label		= m_LabelControls[defindex]	as Label;
						if (label != null)
							left					= label.Left;
					}

					c.Name							= name.Value;
					c.Top							= 2;
					c.Left							= left;
					int					sizeval		= 10;
					if (size != null)
						sizeval						= int.Parse(size.Value);
					else if (length != null)
						sizeval						= int.Parse(length.Value);

					c.Width							= sizeval *	8 +	4;
					pnlAttr.Controls.Add(c);
					ArrayList			al			= m_AttributeControls[defindex]	as ArrayList;
					al.Add(c);
					if (al.Count > m_definitions_fixed && 
						m_HScroll != null &&
						al.Count - m_definitions_fixed <= m_HScroll.Value)
						c.Visible					= false;
					else
						left						+= 4 + c.Width;

					if (c.Top + c.Height > maxheight)
						maxheight					= c.Top	+ c.Height;
				}

				// Make sure it is wide enough
				if (pnlAttr.Width < left)
					pnlAttr.Width					= left + 2;
				// increase index of processed attribute definition
				defindex++;
			}

			pnlAttr.Height							= maxheight	+ 2;
			pnlAttr.Top								= m_currenttop;// -	(maxheight + 2)	* (m_VScroll.Value);

			// Empty <attributes/> element does not count too, so test for > 1
			SetPanelVisible(pnlAttr, (Value.ChildNodes.Count > 1));
			m_panel.Controls.Add(pnlAttr);

			m_currenttop							+= pnlAttr.Height;

			return pnlAttr;
		}

		public void InitializeImmutableNameIdsForSets()
		{
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
			}
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
				result[pidx] = t.Text.Trim();
			}
			return result;
		}

		public void SetPanelVisible(Panel p, bool visible)
		{
			int							index		= 0;
			if (m_HScroll != null)
				index								= m_HScroll.Value;
			
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
		private void CreateLabels()
		{
			int							adefidx		= 0;
            
            foreach (XmlNode a in m_definitions.SelectNodes("element"))
			{
				string					strCaption;

				if (a.Attributes["caption"] != null)
					strCaption						= a.Attributes["caption"].InnerText;
				else
					strCaption						= TemplateUtil.Instance().InventCaptionForName(a.Attributes["name"].InnerText);

				Label					lblName		= new Label();
				lblName.Text						= strCaption;
				lblName.Height						= m_starttop - 2;
                
				string					strToolTip	= "";
				if (a.Attributes["tooltip"] != null)
					strToolTip						= a.Attributes["tooltip"].Value;
				strToolTip							+= "(" + a.Attributes["name"].Value	+ ")";
                m_tt.SetToolTip(lblName, strToolTip);

				// Walk all controls created for the attributedefinition
				// and find out the leftmost position and rightmost position.
				// Position label to leftmost, and let it extend to rightmost.
				int						leftmost	= int.MaxValue;
				int						rightmost	= 0;
				if (m_AttributeControls.Count > adefidx)
				{
					foreach (Control c in (m_AttributeControls[adefidx] as ArrayList))
					{
						if (leftmost > c.Left)
							leftmost				= c.Left;
						if (rightmost < c.Left + c.Width)
							rightmost				= c.Left + c.Width;
					}
				}
				if (rightmost > 0) // any control found?
				{
					lblName.Name					= "label_" + a.Attributes["name"].InnerText.Trim();
					lblName.Left					= leftmost;	
					lblName.Width					= rightmost	- leftmost;
					m_panel.Controls.Add(lblName);
					m_LabelControls.Add(lblName);
				}
				else
				{
					// make sure that label numbering conforms to definition number.
					m_LabelControls.Add(null);
				}
				adefidx++;
			}

		}
		private void CreateEmptyRow()
		{
			// And add an empty line
			XmlNode						newvalue	= m_values.OwnerDocument.CreateElement(m_elementname);
			
			System.Diagnostics.Debug.Assert(newvalue != null, "Shit newvalue is empty");

			m_values.AppendChild(newvalue);
			Panel						newpanel	= FillOne(newvalue);
			m_panelrowcount++;
			m_panelheight							= newpanel.Height;
			m_panel.Height							= m_starttop + m_panelrowcount * m_panelheight;

			if (m_panel.Width < newpanel.Width)
				m_panel.Width						= newpanel.Width;

			RecalcVScrollbar();
		}

		/// <summary>
		/// position the 'index' attributedefinition to the leftmost position next to the
		/// name control. Place the higher controls to the right, and make the lower controls
		/// invisible.
		/// </summary>
		/// <param name="index"></param>
		public void HScrollTo(int index)
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
				left								= label.Left + label.Width + 4;
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
						left						+= label.Width + 4;
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
		}

		private void txtName_TextLeave(object sender, System.EventArgs e)
		{
			Control						t			= sender as	Control;
			SetPanelVisible(t.Parent as Panel, t.Text.Trim() != "");
			// When last row on the list gets a new, add an empty one below.
			if (t.Text.Trim() != "")
			{
				NumericUpDown			n			= m_OrderControls[int.Parse(t.Parent.Name)]	as NumericUpDown;
				// ordering control found, and contains last number... 
				// set focus to previously invisible field
				// ... add new row
				if (n != null && n.Value == m_panelrowcount)
				{
					CreateEmptyRow();
				}
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
                string[]                definitive_names
                                                    = m_attributespanel.GetNamePanels();
				foreach (XmlNode x in m_values.SelectNodes(m_elementname + "/attributes/attribute"))
				{
					XmlAttribute		a			= x.Attributes["refid"];
					if (a == null)
						continue;
					if (a.Value == "")
					{
						x.ParentNode.RemoveChild(x);
						continue;
					}

					XmlNode				n			= x.SelectSingleNode("name");
					if (n == null)
					{
						n							= x.OwnerDocument.CreateElement("name");
						x.AppendChild(n);
					}
					n.InnerText						= definitive_names[int.Parse(a.Value)];
				}
			} */
			m_values								= null;
		}

		/// <summary>
		/// Methode alleen gemaakt voor de inventlabels functie die de labels moet bewaren 
		/// </summary>
		public void ReadAllPanels()
		{
			foreach (Control c in m_panel.Controls)
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
		private void pnlAttr_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Panel						p			= sender as	Panel;
			p.DoDragDrop(p, DragDropEffects.Copy | DragDropEffects.Move);
		}
		private void pnlAttr_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			const int CONTROL_KEY = 8;

			Panel						p			= sender as	Panel;
			/*if (! (e.Data is Panel))
			{
				e.Effect							= DragDropEffects.None;
				return;
			}*/

			if ((e.KeyState & CONTROL_KEY) != 0)
				e.Effect							= DragDropEffects.Copy;
			else //if ((e.KeyState & Keys.Shift) == Keys.Shift)
				e.Effect							= DragDropEffects.Move;
			
			if (m_previous_entered_panel != null)
				m_previous_entered_panel.BackColor	= m_previous_entered_color;

			m_previous_entered_panel				= p;
			m_previous_entered_color				= p.BackColor;
			p.BackColor								= SystemColors.Highlight;
		}

		private void pnlAttr_DragLeave(object sender, System.EventArgs e)
		{
			Panel						p			= sender as	Panel;

			if (m_previous_entered_panel != null)
				m_previous_entered_panel.BackColor	= m_previous_entered_color;
		}

		private void pnlAttr_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			/*if (! (e.Data is Panel))
			{
				e.Effect							= DragDropEffects.None;
				return;
			}*/

			Panel						p			= sender as	Panel;
			//MessageBox.Show("Drop !");
		}
		private void pnlPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			/*if (! (e.Data is Panel))
			{
				e.Effect							= DragDropEffects.None;
				return;
			}*/

			Panel						p			= sender as	Panel;
			
			//MessageBox.Show("Drop on panel ! on y loc: " + e.Y.ToString());
		}

		private void pnlAttr_Enter(object sender, System.EventArgs e)
		{
			Panel						p			= sender as	Panel;
			
			if (m_previous_entered_panel != null)
				m_previous_entered_panel.BackColor	= SystemColors.Control;
			p.BackColor								= SystemColors.Highlight;
			m_previous_entered_panel				= p;

			if (m_setattributespanel == null)
				return;

			// Cleanup the previous setting of the set attributes.
			m_setattributespanel.CleanupValues();

			XmlNode						val			= m_Value[int.Parse(p.Name)] as	XmlNode;
			XmlNode						selattrs	= val.SelectSingleNode("attributes");

			System.Diagnostics.Debug.Assert(selattrs != null,"Shit selectedattributes is empty");

			m_setattributespanel.Fill(selattrs);
		}

		private void txtOrder_Leave(object sender, System.EventArgs e)
		{
			NumericUpDown				curOrder	= sender as	NumericUpDown;

			bool						emptyValue	= curOrder.Text	== "";
/*			try
			{
				Decimal					val			= curOrder.Value;
				emptyValue							= false;
			}
			catch
			{
				emptyValue							= true;
			}
*/
//not sure : SetPanelVisible(curOrder.Parent as Panel, !emptyValue);
			// Renumber controls with numbers higher than this one
			// if an order control with the same number exists.
			OrderControls(curOrder);

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
			int							currenttop	= m_starttop;
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
						if (n.Value < min_val && n != curOrder) 
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
				ReadControl(min_ctr, m_Value[int.Parse(min_ctr.Parent.Name)] as XmlNode);

			}
		}

		/// <summary>
		/// Process resizing of the container panel. 
		/// Recalculate the available scrollbars.
		/// </summary>
		public void Resize()
		{
			if (m_VScroll != null)
			{
				RecalcVScrollbar();
				VScrollTo(m_VScroll.Value);
			}
			if (m_HScroll != null)
				RecalcHScrollbar();
			
			
		}

		/// <summary>
		/// Make the 'pos' row(panel) the topmost one.
		/// </summary>
		/// <param name="pos"></param>
		public void VScrollTo(int pos)
		{
			m_panel.Top								= -pos;
		}

		private void RecalcVScrollbar()
		{
			if (m_VScroll == null)
				return;

			// VScrollbar recalculation
			if (m_panelrowcount > 5)
			{
				m_VScroll.Minimum					= 0;
				m_VScroll.Maximum					= m_panel.Height;//	- this.m_VScroll.Height; //m_panelrowcount - 1;
				m_VScroll.Enabled					= true;
				m_VScroll.SmallChange				= m_panelheight;
				m_VScroll.LargeChange				= this.m_VScroll.Height;
				if (m_VScroll.Value > m_VScroll.Maximum)
					m_VScroll.Value					= m_VScroll.Maximum;
				if (m_VScroll.Value < m_VScroll.Minimum)
					m_VScroll.Value					= m_VScroll.Minimum;

			}
			else
			{
				m_VScroll.Value						= 0;
				m_VScroll.Enabled					= false;
			}
		}

		private void RecalcHScrollbar()
		{
			if (m_HScroll == null)
				return;

			// HScrollbar recalculation
			if (m_definitions_fixed < m_definitions_count - 1)
			{
				m_HScroll.Minimum					= 0;
				m_HScroll.Maximum					= m_definitions_count -	m_definitions_fixed	- 1;
				m_HScroll.LargeChange				= 2;
				m_HScroll.SmallChange				= 1;
				m_HScroll.Enabled					= true;
			}
			else
			{
				m_HScroll.Enabled					= false;
			}
		}
		private void mnuOpenLink_Click(object sender, System.EventArgs e)
		{
		
		}
		public void PropagateChanges(XmlNode type, string oldname, string newname)
		{
			if (type == null || oldname == newname)
				return;

			string						changedtype	= type.Attributes["definition"].Value;
			foreach (XmlNode element in m_definitions.SelectNodes("element"))
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
				foreach (Control c in m_panel.Controls)
				{
					Panel				p			= c	as Panel;
					if (p == null)
						continue;

					ComboBox			combo		= FindControl(elname, p.Controls) as ComboBox;
					System.Diagnostics.Debug.Assert(combo != null, "Het is geen combobox!");
					
					bool				wasselected	= (combo.Text == oldname);
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
					{
						combo.Text					= newname;
						this.m_owner.dirty			= true;
					}
				}
			}

			/*if (m_attributespanel != null)
				m_attributespanel.PropagateChanges(type, oldname, newname);
			if (m_setattributespanel != null)
				m_setattributespanel.PropagateChanges(type, oldname, newname);*/
		}
	}
}
