/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/
using System;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace Generator.Utility
{
	/// <summary>
	/// Summary description for TemplateCache.
	/// </summary>
	public class TemplateCache
	{
		private	string			        m_solution	            = "";
		private	string			        m_solutionname			= "";
		private	string			        m_solutionlocation  	= "";
		private	string			        m_solutionfilename		= "";

		private	string			        m_project_directory     = "";
        private string                  m_project_file          = "";

		private	static TemplateCache	cm_instance;

		private	XmlDocument				m_cache;
		private	bool					m_solution_as_project = false;

		public static TemplateCache Instance()
		{
			if (cm_instance == null)
				cm_instance							= new TemplateCache();
			return cm_instance;
		}
		
		/// <summary>
		/// Clear the cache. Complete means complete, otherwise only clear the
		/// project specifics: types that are user-editable, project-directory.
		/// </summary>
		/// <param name="complete"></param>
		public void Clear(bool complete)
		{
			if (complete)
			{
				cm_instance							= null;
				m_solution							= "";
			}
			else if (cm_instance != null && m_project_directory != "")
			{
				// find all user-editable types, and remove these from the cache.
				string[]				vw			= cm_instance.GetViewableTypenamesList(true, false);
				foreach (string vwtype in vw)
				{
					XmlNode				vwnode		= cm_instance.m_cache.SelectSingleNode(vwtype);
					if (vwnode != null)
						vwnode.ParentNode.RemoveChild(vwnode);
				}
			}
			m_project_directory					= "";
		}

		// Force a reload of the cache but keep the same
		// context for solution and projectdirectory
		public static void Reload()
		{
			cm_instance								= null;
		}

		private TemplateCache()
		{
		}

		public XmlNode AddNewTemplateFile(string type, string name, string filename, out XmlNode type_definition)
		{
            XmlNode						cached_type	= GetTypesList(type);
			XmlNode						result		= null;
			// When no files of the type where loaded yet, it can just be loaded.
			// Check if its in the cache, otherwise add it by hand
			if (cached_type != null)
			{
				result								= cached_type.SelectSingleNode("*/*[@sourcefile=\"" + filename + "\"]");
			
				// If already existing, remove it, including parent 'n' node
				if (result != null)
					result.ParentNode.ParentNode.RemoveChild(result.ParentNode);
			
				result								= null;
			}

			type_definition							= GetTemplateType(type);
			string						nameattribute
													= type_definition.Attributes["nameattribute"].Value;

			// Create empty document with right type reference
			XmlDocument					newdoc		= new XmlDocument();
			newdoc.AppendChild(newdoc.CreateElement("type"));
			XmlAttribute				typeatt		= newdoc.CreateAttribute("definition");
			typeatt.Value							= type;
			XmlAttribute				solatt		= newdoc.CreateAttribute("solution");
			solatt.Value							= SolutionFilename;
			XmlNode						namenode	= newdoc.CreateElement(nameattribute);
			namenode.InnerText						= name;

			newdoc.DocumentElement.Attributes.Append(typeatt);
			newdoc.DocumentElement.Attributes.Append(solatt);
			newdoc.DocumentElement.AppendChild(namenode);
            TemplateUtil.Instance().MakeSureDirectoryExistsFor(filename);
			newdoc.Save(filename);

			// Use the loadfile method to get it in exactly the same format in the cache.
			result									= LoadFileIntoCache(filename, nameattribute, cached_type, type);
			
			return result;
		}



		/// <summary>
		/// get/set directory from where user-definable template files are used/saved.
		/// </summary>
		public string ProjectDirectory
		{
			set
			{
				if (value != "")
					System.Diagnostics.Debug.Assert(m_project_directory == "", "projectdirectory mag alleen gezet worden als deze nog leeg is!");
				m_project_directory				= value;
			}
			get
			{
				return m_project_directory;
			}
		}

		/// <summary>
		/// Replaces the contents in the cache for the given filename with
		/// the given contents. Asserts file is in cache.
		/// Fills oldname and newname with name values.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="val"></param>
		public void RefreshValue(out string filename, XmlNode val, out string oldname, out string newname)
		{
			XmlAttribute				sf_att		= val.Attributes["sourcefile"];
			filename								= sf_att.Value;
			XmlNode						currentcontents
													= m_cache.DocumentElement.SelectSingleNode("*/*/*[@sourcefile=\"" + filename + "\"]");
			System.Diagnostics.Debug.Assert(currentcontents != null, "Refresh file which is unknown in the cache:" + filename);
			if (currentcontents == null)
			{
				// not in cache, assertion ignored, nothing to refresh
				oldname								= "";
				newname								= "";
				filename							= "";
				return;
			}

			// keep the 'n' node, replace the child with imported val.
			XmlNode						currentparent
													= currentcontents.ParentNode;
            /* Dit stond er eerst
             * 
            // getaway with the old shit and import new shit
            currentparent.RemoveChild(currentcontents);
            XmlNode						newvalue	= m_cache.ImportNode(val, true);

            // Put it under the right parent
            currentparent.AppendChild(newvalue);

            // Derive the new name 
            oldname									= currentparent.Attributes["name"].Value;
            XmlNode						typedef		= this.GetTemplateType(val.Attributes["definition"].Value);
            XmlNode						namenode	= newvalue.SelectSingleNode(typedef.Attributes["nameattribute"].Value);
            newname									= namenode.InnerText;
            if (oldname != newname)
                currentparent.Attributes["name"].Value = newname;
			
            // Update the file on disk
            SaveFile(out filename, newvalue);
			
            PropagateChanges(val, oldname, newname);
            */

            /* En dit wil ik er van maken */
            // getaway with the old shit and import new shit
            XmlNode newvalue = m_cache.ImportNode(val, true);

            // Derive the new name 
            oldname = currentparent.Attributes["name"].Value;
            XmlNode typedef = this.GetTemplateType(val.Attributes["definition"].Value);
            XmlNode namenode = newvalue.SelectSingleNode(typedef.Attributes["nameattribute"].Value);
            newname = namenode.InnerText;
            // Update the file on disk
            // Hier kan een exception optreden, dus doe het echte werk erna.
            SaveFile(out filename, newvalue);

            // Put it under the right parent
            currentparent.RemoveChild(currentcontents);
            currentparent.AppendChild(newvalue);

            if (oldname != newname)
            {
                currentparent.Attributes["name"].Value = newname;
                currentparent.Attributes["searchname"].Value = newname.ToLower();
            }

            PropagateChanges(val, oldname, newname);
            /* tot aan hier ***************/
            return;
		}
		
		public void PropagateChanges(XmlNode type, string oldname, string newname)
		{
			if (type == null || oldname == newname)
				return;

			string						changedtype	= type.Attributes["definition"].Value;
			// Look into all element definitions where a relation to the changed type
			// is used. Where found walk all instances having this oldname as the value and
			// update these to newname.
			foreach (XmlNode element in this.GetTypesList("TypeDefs").SelectNodes("n/template/*/element[@type='" + changedtype + "']"))
			{
				string					parenttype	= element.ParentNode.Name;
				XmlNode					def			= element.ParentNode.ParentNode;
				
				// Find the instances having the value for the exact element.
				//string					usedtype	= def.Attributes["definition"].Value;
				// ! AHA! bijwerken die zaak
//				string					elname		= element.Attributes["name"].Value;
			}
		}

		public void SaveFile(out string filename, XmlNode currentvalue)
		{
			// Remove the sourcefile attribute, 
			// use it to get the proper filename, but
			// it is redundant in the saved file.
			XmlAttribute				sf_att		= currentvalue.Attributes["sourcefile"];
			System.Diagnostics.Debug.Assert(sf_att != null, "Quaoii? Bewaren maar we weten geen filename!");

			filename								= sf_att.Value;

			// Get the name of the thing itself. And change the filename accordingly
			XmlNode						type		= this.GetTemplateType(currentvalue.Attributes["definition"].Value);
			string						nameatt		= type.Attributes["nameattribute"].Value;
			string						newname		= currentvalue.SelectSingleNode(nameatt).InnerText.Trim();
			string						oldname		= System.IO.Path.GetFileNameWithoutExtension(filename);
            string filename_to_be_removed_if_save_succeeded = "";
			if (newname != oldname)
			{
                filename_to_be_removed_if_save_succeeded = filename;
				string					newfilename	= System.IO.Path.GetDirectoryName(filename)	+ @"\" + newname + System.IO.Path.GetExtension(filename);
				sf_att.Value						= newfilename;
				filename							= newfilename;
			}

			// Clone the instance to something savable
			XmlDocument					x			= new XmlDocument();
			x.AppendChild(x.ImportNode(currentvalue, true));
			sf_att									= x.DocumentElement.Attributes["sourcefile"];
			x.DocumentElement.Attributes.Remove(sf_att);

			// Remove the writable attribute, it is redundant in the saved file.
			XmlAttribute				wa			= x.DocumentElement.Attributes["writable"];
			if (wa != null)
				System.Diagnostics.Debug.Assert(wa.Value == "true", "Quaoii? Bewaren but not writable?", "Dat zal wel niet lukken");
            x.DocumentElement.Attributes.Remove(wa);
            TemplateUtil.Instance().MakeSureDirectoryExistsFor(filename);
			x.Save(filename);
            
            if (filename_to_be_removed_if_save_succeeded != "")
                System.IO.File.Delete(filename_to_be_removed_if_save_succeeded);
            return;
		}


		/// <summary>
		/// Tries to reload the file. Returns the thing if success, null otherwise.
		/// </summary>
		/// <param name="filename"></param>
		public XmlNode ReloadFile(string filename)
		{
			XmlNode						currentcontents
													= m_cache.DocumentElement.SelectSingleNode("*/*/*[@sourcefile=\"" + filename + "\"]");
			if (currentcontents == null)
				// not in cache, nothing to reload too
				return null;

			XmlNode						currentparent
													= currentcontents.ParentNode.ParentNode;
			string						type		= currentcontents.Attributes["definition"].Value;
			XmlNode						typedef		= GetTemplateType(type);
			string						nameattribute
													= typedef.Attributes["nameattribute"].Value;
			
			// getaway with the old shit
			currentparent.RemoveChild(currentcontents.ParentNode);

			// get the brandnew from disk!
			return LoadFileIntoCache(filename, nameattribute, currentparent, type);
		}

		public XmlNode GetFile(string filename, out XmlNode type)
		{
			if (m_cache == null)
			{
				m_cache								= new XmlDocument();
				m_cache.AppendChild(m_cache.CreateElement("cache"));
			}

			// We start with not knowing.
			type									= null;
			string						typename	= "";

			// Check to see if file available
			XmlNode						result		= m_cache.DocumentElement.SelectSingleNode("*/*/*[@sourcefile=\"" + filename + "\"]");
			if (result == null)
			{
				// File not yet loaded
				string[]				pathelements= filename.Split(new Char[]	{ Path.AltDirectorySeparatorChar,Path.DirectorySeparatorChar});
				if (pathelements.GetLength(0) <= 1)
					throw new ApplicationException("Getfile needs filename including path, not '" + filename + "'");

				// Check to see if not loaded standard type.
				typename							= pathelements[pathelements.GetUpperBound(0) - 1];
				type								= GetTypesList("TypeDefs").SelectSingleNode("n[@name='" + typename + "']");

				if (type == null)
				{
					// it must be a user editable type
					// check the projectdirectory.
					System.Diagnostics.Debug.Assert(m_project_directory == "" || m_project_directory == Path.GetDirectoryName(filename), "Projectfile is not equal to where this should come from, but it isnt standard");
					m_project_directory			= Path.GetDirectoryName(filename);
					XmlDocument			doc			= new XmlDocument();
					doc.Load(filename);
					typename						= doc.DocumentElement.Attributes["definition"].Value;
				}
				else
				{
					// Get the contents without the name parent
					type							= type.FirstChild;
				}
				// Yep, its a not loaded standard type thing, or user editable
				// Get all of that type, and set output type 
				result								= GetTypesList(typename).SelectSingleNode("*/*[@sourcefile=\"" + filename + "\"]");		
			}
			else
			{
				// We know the thing, get the typedefinition too
				typename							= result.Attributes["definition"].Value;
			}

			if (type == null)
				type								= GetTemplateType(typename);

			System.Diagnostics.Debug.Assert(result != null, "Probably a file created after the cache was loaded, should be automatically added. Need some more code.....");
			return result;
		}
        /// <summary>
        /// Templatesfiles are only editable when we're treating the solution
        /// as a project.
        /// </summary>
        public bool TemplatefilesEditable
        {
            get
            {
                return m_solution_as_project;
            }
        }
		/// <summary>
		/// Returns the list of typedefinitions this user can view (and edit)
		/// Include user files when 'user' is set, include other files when 'other' is set.
		/// 'Other' will not add anything when directload is not set.
		/// </summary>
		/// <returns></returns>
		public string[] GetViewableTypenamesList(bool user, bool other)
		{
			string[]					names;

			// Make sure the cache is filled for the type
			XmlNode						values		= GetTypesList("TypeDefs");

			if (user && other)
			{
				// Get all
				names								= new string[values.ChildNodes.Count];
				int						i			= 0;
				foreach (XmlNode n in values.SelectNodes("n"))
					names[i++]						= n.Attributes["name"].Value;
			}
			else 
			{
				// Only choose user or other templates
				StringCollection		namecol		= new StringCollection();
				foreach (XmlNode n in values.SelectNodes("n"))
				{
					XmlAttribute		user_att	= n.SelectSingleNode("template").Attributes["user"];
                    // Sowieso als het TemplateFile is, niet teruggeven
                    string              name        = n.Attributes["name"].Value;
                    if (name == "__TemplateFile")
                    {
                        continue;
                    }

                    // Beetje ingewikkeld.
                    // Als we een m_solution_as_project zijn werkt het net andersom
                    // Dan willen we de userfiles juist niet terug als daarom gevraagd wordt 
                    if (m_solution_as_project)
                    {
                        if ((other && user_att != null && user_att.Value == "true")
                            || (user && (user_att == null || user_att.Value != "true")))
                            namecol.Add(name);
                    }
                    else
                    {
                        if ((user && user_att != null && user_att.Value == "true")
                            || (other && (user_att == null || user_att.Value != "true")))
                            namecol.Add(name);
                    }
				}
				names								= new String[namecol.Count];
				namecol.CopyTo(names, 0);
			}
			Array.Sort(names);
			return names;
		}
		/// <summary>
		/// Retrieve the sorted list of names of items for the given type. When type
		/// equals TypeDefs the list of types is asked. 
		/// </summary>
		/// <param name="typetype"></param>
		/// <returns></returns>
		public string[] GetTypenamesList(string type)
		{
			if (m_cache == null)
			{
				m_cache								= new XmlDocument();
				m_cache.AppendChild(m_cache.CreateElement("cache"));
			}

			string						nameattribute
													= "name";

			// Check the cache, and add to it when not available.
			XmlNode						result		= m_cache.DocumentElement.SelectSingleNode(type);
            if (result == null)
            {
                string path = "";
                string mask;

                if (type == "TypeDefs")
                {
                    path = m_solutionlocation + @"\UserConcept";
                    mask = "*.xml";
                }
                else
                {
                    XmlNode typedef = GetValueFor("TypeDefs", type);
                    if (typedef == null)
                        throw new ApplicationException($"Type '{type}' not found");

                    XmlAttribute att = (typedef == null) ? null : typedef.Attributes["user"];
                    if (type == "__TemplateFile")
                    {
                        path = m_solutionlocation + @"\Template";
                    }
                    else if (att == null || att.Value != "true")
                    {
                        path = m_solutionlocation + @"\" + type;
                    }
                    else if (m_project_directory != "")
                    {
                        path = m_project_directory + @"\" + type;
                    }
                    nameattribute = typedef.Attributes["nameattribute"].Value;

                    mask = "*.xmt";
                }
                if (path != "")
                {
                    string[] files = Directory_GetFiles(path, mask);
                    string[] names = new string[files.Length];
                    int i = 0;
                    foreach (string f in files)
                        names[i++] = Path.GetFileNameWithoutExtension(f);
                    Array.Sort(names);
                    return names;
                }
                else
                {
                    return new string[0];
                }
            }
            else
            {
                // Make sure the cache is filled for the type
                XmlNode values = result;
                if (values == null)
                    return new String[0];

                string[] names = new string[values.ChildNodes.Count];
                int i = 0;
                foreach (XmlNode n in values.SelectNodes("n"))
                    names[i++] = n.Attributes["name"].Value;
                Array.Sort(names);
                return names;

            }
		}

		public XmlNode GetTemplateType(string type)
		{
			if (type.EndsWith(".xml"))
				type								= type.Substring(0,	type.Length	- 4);

			// Make sure the cache is filled for the type
			XmlNode						values		= GetTypesList("TypeDefs");
			// And now get it from the list
			XmlNode						result		= values.SelectSingleNode("n[@searchname='" + type.ToLower() + "']");
			if (result == null)
				throw new ApplicationException("Type definition not found for (searchname) '" + type + "'");

			return result.FirstChild;
		}

		/// <summary>
		/// Get the XmlNode for the requested filename containing the thing.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns>null if not in cache</returns>
		public XmlNode GetValueFor(string filename)
		{
			XmlNode						result		= m_cache.DocumentElement.SelectSingleNode("*/*/*[@sourcefile=\"" + filename + "\"]");
			return result;
		}

		/// <summary>
		/// Get the XmlNode for the requested type and name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="val"></param>
		/// <returns></returns> 
		public XmlNode GetValueFor(string type, string val)
		{
			// Make sure the cache is filled for the type
			XmlNode						values		= GetTypesList(type);
			// And now get it from the list TODO RESET : TW HIER WAT AANGEPAST 21 8 2008
			XmlNode						result		= values.SelectSingleNode("n[@searchname=\"" + val.ToLower() + "\"]");
            if (result == null)
            {
                return null;
                //throw new ApplicationException("Type '" + type + "' value '" + val + "' not found");
            }
			return result.FirstChild;
		}

		/// <summary>
		/// Get the XmlNode for the requested type and name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="val"></param>
		/// <returns></returns> 
		public bool DeleteValue(string type, string val)
		{
			
			// Make sure the cache is filled for the type
			XmlNode						value		= GetValueFor(type,	val);
			XmlAttribute				sf			= value.Attributes["sourcefile"];
			if (sf != null)
			{
				string					sourcefile	= sf.Value;
				if (value.Attributes["writable"] != null && value.Attributes["writable"].Value == "true")
					File.Delete(sourcefile);
			}
			value.ParentNode.RemoveChild(value);

			return true;
		}

		/// <summary>
		/// Cache contains for each type and for "TypeDefs" a node with
		/// all instances as "n" node children.
		/// </summary>
		/// <param name="type">The type requested</param>
		/// <returns>The node with all child instances of the requested type</returns>
		public XmlNode GetTypesList(string type)
		{
			if (m_cache == null)
			{
				m_cache								= new XmlDocument();
				m_cache.AppendChild(m_cache.CreateElement("cache"));
			}

			//string key = type;
			string						nameattribute
													= "name";

			// Check the cache, and add to it when not available.
			XmlNode						result		= m_cache.DocumentElement.SelectSingleNode(type);
			if (result == null)
			{
				string[]				paths		= new string[] { "" };
				string					mask;

				if (type == "TypeDefs")
				{
					paths							= new string[] { m_solutionlocation + @"\UserConcept", m_solutionlocation + @"\CoreConcept" };
					mask							= "*.xml";
				}
				else
				{
					XmlNode				typedef		= GetValueFor("TypeDefs", type);
                    if (typedef == null)
                        throw new ApplicationException("Type not found");

					XmlAttribute		att			= ( typedef == null ) ? null : typedef.Attributes["user"];
    				if (type == "__TemplateFile")
                    {
						paths[0]					= m_solutionlocation + @"\Template";
                    }
					else if (att == null || att.Value != "true")
					{
                        paths[0]					= m_solutionlocation + @"\" + type;
					}
                    else if (m_project_directory != "")
                    {
						paths[0]                    = m_project_directory + @"\" + type;
                    }
					nameattribute					= typedef.Attributes["nameattribute"].Value;

					mask							= "*.xmt";
				}

				if (paths[0] != "")
				{
                    XmlNode docs = m_cache.CreateElement(type);
                    m_cache.DocumentElement.AppendChild(docs);

                    foreach (string path in paths)
					{
						string[] files = Directory_GetFiles(path, mask);
						string file = "";

						foreach (string f in files)
						{
							try
							{
								file = f;
								LoadFileIntoCache(f, nameattribute, docs, type);
							}
							catch (Exception ex)
							{
								throw new ApplicationException(string.Format("Malformed type definition found in file '{0}':{1}", file, ex.Message), ex);
							}
						}
					}
					if (type == "TypeDefs")
						LoadFixedDefinitionsIntoCache(docs);

					return docs;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return result;
			}
		}

		private XmlNode LoadFixedDefinitionsIntoCache(XmlNode docs)
		{
			string TemplateFileDef = 
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" + 
				"<template>" +
				"<elements>" +
				"	<element name=\"name\" lines=\"1\" type=\"Name\"/>" +
				"	<element name=\"description\" lines=\"3\"/>" +
				"	<element name=\"group\" type=\"group\"/>" +
				"	<element name=\"appliesto\" caption=\"Applicable concept\" type=\"UserConcept\"/>" +
				"	<element name=\"templatefilename\" caption=\"Template filename\"/>" +
				"	<element name=\"directory\" caption=\"Directoryname\" type=\"Text\"/>" +
				"	<element name=\"filename\" caption=\"Filename\" type=\"Text\"/>" +
				"	<element name=\"generateonce\" caption=\"Generate once\" type=\"Checkbox\"/>" +
				"	<element name=\"copyonly\" caption=\"Copy only\" type=\"Checkbox\"/>" +
				"</elements>" +
				"</template>";
			XmlDocument					td			= new XmlDocument();
			td.LoadXml(TemplateFileDef);
			return LoadDocumentIntoCache(td, "__TemplateFile", "name", docs, "TypeDefs");
		}

		private XmlNode LoadFileIntoCache(string filename, string nameattribute, XmlNode parent, string type)
		{
			XmlDocument					td			= new XmlDocument();
			td.Load(filename);
			return LoadDocumentIntoCache(td, filename, nameattribute, parent, type);
		}

		/// <summary>
		/// Load the file with the given name under the given parent node.
		/// File must contain instance of given type.
		/// </summary>
		/// <param name="filename">of file to load</param>
		/// <param name="parent">where to place it</param>
		/// <param name="type">when non "", only load if of that type</param>
		/// <returns>'n' node appended, null if none</returns>
		private XmlNode LoadDocumentIntoCache(XmlDocument td, string filename, string nameattribute, XmlNode parent, string type)
		{
			// Skip if it refers to another type of definition
			// Store the sourcefilename in the documentelement
			XmlAttribute				a			= td.CreateAttribute("sourcefile");
			a.Value                                 = filename;
			td.DocumentElement.Attributes.Append(a);
			
			// Check available solution attribute, otherwise add it.
			a										= td.DocumentElement.Attributes["solution"];
			if (a != null && a.Value != "" && Solution != "" && a.Value != Solution)
				System.Diagnostics.Debug.WriteLine("File '" + filename + "' needs Solution of type '" + a.Value + "'. Current solution = '" + Solution + "'. Load stopped!");

			if (a == null && Solution != "")
			{
				a									= td.CreateAttribute("solution");
				a.Value								= Solution;
				td.DocumentElement.Attributes.Append(a);
			}

			// Store the 'writable' attribute in the documentelement
			a										= td.CreateAttribute("writable");
			try
			{
				System.IO.FileAttributes
										atts		= System.IO.File.GetAttributes(filename);
				if (0 == (atts & System.IO.FileAttributes.ReadOnly))
					a.Value							= "true";
				else
					a.Value							= "false";
			}
			catch
			{
				a.Value								= "false";
			}
			td.DocumentElement.Attributes.Append(a);
			
			// Store the name of the thing in the 'n' parent element
			string						name;
			// In case of a typedef, we take the name of the file as the type
			if (type == "TypeDefs")
			{
				//System.Diagnostics.Debug.Assert(filename.EndsWith(".xml"),"Type def eindigt niet op xml? Hoeft niet schadelijk te zijn, is eigelijk wel vreemd.");
				if (filename.EndsWith(".xml"))
				{
					name							= Path.GetFileNameWithoutExtension(filename);
				}
				else
				{
					name							= filename;
				}
				// Get the namegiving attribute, and add this as an attribute
				// to the type for easy lookup lateron.
				XmlNode					namegivingelement
													= td.DocumentElement.SelectSingleNode("elements/element[@type='Name']");
				System.Diagnostics.Debug.Assert(namegivingelement != null, "Type: " + name + " misses name-giving element (missing type='Name')");
				a									= td.CreateAttribute("nameattribute");
				a.Value								= namegivingelement.Attributes["name"].Value; 
				td.DocumentElement.Attributes.Append(a);			
			}
			else
			{
				name								= td.DocumentElement.SelectSingleNode(nameattribute).InnerText;
				if (name.ToLower() != Path.GetFileNameWithoutExtension(filename).ToLower())
				{
					throw new ApplicationException("Name of concept '" + name + "' is not equal to the filename. It should be. Change the filename or the name, or remove the file completely. Load stopped!");
				}
			}

			// create <n name='...'> element with file contents below it.
			XmlNode						nItem		= m_cache.CreateElement("n");
			a										= m_cache.CreateAttribute("name");
			a.Value									= name;
			nItem.Attributes.Append(a);
            a                                       = m_cache.CreateAttribute("searchname");
            a.Value                                 = name.ToLower();
            nItem.Attributes.Append(a);

    		nItem.AppendChild(m_cache.ImportNode(td.DocumentElement, true));
			parent.AppendChild(nItem);

			return nItem;
		}

		public void SetSolution(string code, string name, string location, string filename, bool solution_as_project)
		{
			m_solution								= code;
			m_solutionname							= name;
			m_solutionlocation						= Path.GetDirectoryName(location);
			m_solutionfilename						= filename;	// can include a subdirectory structure
            m_solution_as_project                   = solution_as_project;
            if (m_solution_as_project)
            {
                m_project_directory = m_solutionlocation;
            }
		}

		public string SolutionLocation
		{
			get
			{
				return m_solutionlocation;
			}
		}

		public string SolutionFilename
		{
			get
			{
				return m_solutionfilename;
			}
		}
		public string Solution
		{
			get
			{
				return m_solution;
			}
		}

        public void LoadSolutionFile(string filename, bool solution_as_project)
        {
            XmlDocument d = new XmlDocument();
            bool loaded = false;
            Exception loadex = null;
            String location = "";

            // If path has root, it must be a full path. Otherwise test userdata dir first and then appdir.
            if (Path.IsPathRooted(filename))
            {
                try
                {
                    location = filename;
                    d.Load(location);
                    loaded = true;
                }
                catch (Exception ex)
                {
                    loadex = ex;
                }
            }
            else
            {
                // try userdata directory.
                try
                {
                    location = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
                    d.Load(location);
                    loaded = true;
                }
                catch (Exception ex)
                {
                    loadex = ex;
                }
                // if no success try app path dir.
                if (!loaded)
                {
                    try
                    {
                        location = Path.Combine(TemplateUtil.Instance().AppPath(), filename);
                        d.Load(location);
                        loaded = true;
                    }
                    catch (Exception ex)
                    {
                        loadex = ex;
                    }
                }
            }
            if (!loaded)
            {
                throw new ApplicationException("Exception occured during load of solutionfile '" + filename + "'. " + loadex.Message);
            }


            try
            {
                string solution = d.SelectSingleNode("solution/code").InnerText;
                string name = d.SelectSingleNode("solution/name").InnerText;
                SetSolution(solution, name, location, filename, solution_as_project);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception occured during load of solutionfile '" + filename + "'. " + ex.Message);
            }

        }
        public void LoadProjectFile(string filename, bool solution_as_project)
        {
            XmlDocument d = new XmlDocument();
            try
            {
                d.Load(filename);
                string solution_f = "";
                XmlNode location = d.SelectSingleNode("project/solutionfilename");
                if (location != null)
                {
                    solution_f = location.InnerText;
                }
                else
                {
                    solution_f = d.SelectSingleNode("project/solution").InnerText;
                }

                string projectdir = System.IO.Path.GetDirectoryName(filename);
                if (solution_f.StartsWith("."))
                {
                    // it is a relative path, relative to the projectfile's location
                    solution_f = Utility.TemplateUtil.Instance().CombineAndCompact(projectdir, solution_f);
                }
                LoadSolutionFile(solution_f, solution_as_project);

                OptionsSettings.Instance().RegisterLastUsedProject(filename, solution_f);

                //this.Text += ": " + Path.GetFileNameWithoutExtension(filename);
                if (!solution_as_project)
                {
                    m_project_directory = projectdir;
                    m_project_file      = filename;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception occured during load of projectfile '" + filename + "'. " + ex.Message);
            }
        }
        
        public string projectfile
        {
            get
            {
                return m_project_file;
            }
        }

		/// <summary>
		/// Provide a path safe method to invoke the GetFiles method.
		/// It returns an empty list when path not exists or no file found.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public string[] Directory_GetFiles(string path, string mask)
		{
			if (!Directory.Exists(path))
				return new string[0];
			try
			{
				return Directory.GetFiles(path, mask);
			}
			catch
			{
				return new string[0];
			}
		}

        public void SaveSolutionCache(string filename)
        {


        }

        /// <summary>
        /// Geeft de directorynaam inclusief het hele pad waar exemplaren van 'type'  
        /// in opgeslagen moeten worden
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public String GetDirectoryForType(string type)
        {
            string cache_type_dir;
            if (Array.IndexOf(GetViewableTypenamesList(true, false), type) >= 0)
                // it's a user type, and should be in projectdirectory
                cache_type_dir = Path.Combine(ProjectDirectory, type);
            else
                cache_type_dir = Path.Combine(SolutionLocation, type);

            // remove any ../ from the path
            cache_type_dir = Path.GetFullPath(cache_type_dir);
            return cache_type_dir;
        }
	}
}
