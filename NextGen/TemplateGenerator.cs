/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) 2008 Thierry Wiersma													    *
*****************************************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Generator.Utility;
using Generator.Expressions;
using Generator.Exceptions;
using Generator.ObserverPattern;
using Generator.Statements;

namespace Generator
{
	/// <summary>
	/// Information necessary to start a generation 
	/// </summary>
	public class GenerationRequest
	{
		public XmlNode					m_type;
		public XmlNode					m_val;
		public XmlNode					m_definition;
		public string					m_label;
		public GenerationRequest(XmlNode type, XmlNode val, XmlNode definition, string label)
		{
			m_type									= type;
			m_val									= val;
			m_definition							= definition;
			m_label									= label;
		}
	}

	public enum InterpretationStatus
	{
		Inactive,
		Active,
		Skip
	}

	public enum Looptype
	{
		None,
		Collection,
		Attributes,
		Sets,
		Field,
        Expression
	}

	/// <summary>
	/// Summary description for TemplateGenerator.
	/// </summary>
	public class TemplateGenerator : ObserverPattern.ISubject, ObserverPattern.IObserver//, IExternalTokenResolver
	{
		private	Stack					m_Stack;
		private	Hashtable				m_VariableInfo;
		private	string					m_outputfilename;
		private	string					m_inputfilename;
		
		private	XmlNode					m_templateDefinition;
		private	Log						m_log;
		public int						m_errors;
		private	CurrentInfo				m_current;
        private int                     m_currentlinenr;

		private	StringCollection		m_source;
		private	StringCollection		m_target;
		//private	ExpressionEvaluator		m_evaluator;

		public const char	DEFAULT_FIELD_SEPERATOR = '|';
		public const char	DEFAULT_PART_SEPERATOR	= '=';

		public TemplateGenerator(XmlNode type, XmlNode generate_start, XmlNode template_definition)
		{
			m_current								= new CurrentInfo();
			m_current.state							= InterpretationStatus.Active;

			m_current.type							= type;
			m_current.val							= generate_start;
			m_templateDefinition					= template_definition;
		}
        public string GetOutputfilename()
        {
            return m_outputfilename;
        }
        private string GetXmlValue(XmlNode x, string defaultvalue)
        {
            if (x == null)
                return defaultvalue;
            else
                return x.InnerText;
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
            Notify(firstpart,linkeditem, lastpart, filename, linenr);
        }

        #endregion

        public void Generate()
		{
			m_VariableInfo							= new Hashtable();
			m_Stack									= new Stack();
			m_errors								= 0;

			try
			{
				//m_evaluator							= new ExpressionEvaluator(this);
				
				m_log								= new Log("startup " + DateTime.Now.ToString("MM-dd hhmmss ")	+ Math.Round((new Random().NextDouble()	* 9000)	+ 1000,	0).ToString() +	".log");
				ConstructInputOutputFilenames();
				MakeSureDirectoryExistsFor(m_outputfilename);

				m_log								= new Log(Path.GetFileName(m_outputfilename) + ".log");

				// Check to see if only a filecopy has to be done.
				XmlNode					copyonly	= m_templateDefinition.SelectSingleNode("copyonly");
				if (copyonly != null && copyonly.InnerText == "1")
				{
					// Ok copy only. No template interpretation.
					File.Copy(m_inputfilename, m_outputfilename, true);
					return;
				}

				// Check to see if generate-once is set, and output already available.
				XmlNode					generateonce= m_templateDefinition.SelectSingleNode("generateonce");
				if (generateonce != null && generateonce.InnerText == "1")
				{
					if (File.Exists(m_outputfilename))
						return;
				}

				InitializeKeepInfo();
				m_source							= LoadTemplateSource();
				m_target							= new StringCollection();
                m_log.Write(0, InterpretationStatus.Active, "Template: " + m_inputfilename);
                m_log.Write(0, InterpretationStatus.Active, "Outputfile: " + m_outputfilename);

                LineProcessor lp = new LineProcessor(m_current, new Hashtable(), new Hashtable(), m_source, m_target, m_log, m_KeepInfo, m_FunctionInfo, GetCurrentTemplatename(), null);
                lp.AddObserver(this);
                m_errors = lp.Generate();

			}
            catch (SyntaxErrorException ex)
            {
                WriteError(ex);
                //GenerateOutput.OutputToInstance(CurrentContext() + ex.Message);
                m_errors++;
                WriteOutputfile(m_target);
                return;
            }
            catch (Exception ex)
			{
                if (ex.InnerException != null && ex.InnerException is SyntaxErrorException)
                    WriteError(ex.InnerException as SyntaxErrorException);
                else
                    WriteError(ex.Message);

				m_errors++;
                WriteOutputfile(m_target);
                return;
			}

			WriteOutputfile(m_target);

			if (m_log != null)
				m_log.Close();

			return;
		}

        /// <summary>
        /// Inspect the template to process and construct the input and output file
        /// names from it. Together with the info from the datatype to process.
        /// </summary>
        private void ConstructInputOutputFilenames()
        {
            string                      templatefilename
                                                    = GetXmlValue(m_templateDefinition.SelectSingleNode("templatefilename"), "");
			if (templatefilename == "")
				throw new ApplicationException("templatefilename is mandatory in template definitions file");

            string                      directory_expression
                                                    = "";

            // We maken een LineProcessor voor het interpreteren van de tekst van de directory en filenaam.
            LineProcessor lp = new LineProcessor(m_current, new Hashtable(), new Hashtable(), new StringCollection(), new StringCollection(), m_log, m_KeepInfo, m_FunctionInfo, GetCurrentTemplatename(), null);
            lp.AddObserver(this);

			try
			{
				directory_expression				= m_templateDefinition.SelectSingleNode("directory").InnerText;
                if (directory_expression.Contains("@"))
                {
                    m_outputfilename = lp.ProcessLine(directory_expression);
                }
                else
                {
                    m_outputfilename = directory_expression;
                }
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Directory expression '" + directory_expression + "' results in error: " + ex.Message);
			}
            
			if (m_outputfilename == "")
				throw new ApplicationException("Directory expression '" + directory_expression + "' results in empty directoryname for template: " + m_templateDefinition.SelectSingleNode("name").InnerText);

			if (! m_outputfilename.EndsWith(@"\"))
				m_outputfilename					+= @"\";

            string                      filename_expression
                                                    = m_templateDefinition.SelectSingleNode("filename").InnerText;
			string						filename	= "";
			try
			{
				filename							= lp.ProcessLine(filename_expression);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Filename expression '" + filename_expression + "' results in error: " + ex.Message );
			}
			if (filename == "")
				throw new ApplicationException("Filename expression '" + filename_expression + "' results in empty filename for template: " + m_templateDefinition.SelectSingleNode("name").InnerText);

			m_outputfilename						+= filename;

			m_inputfilename							= TemplateCache.Instance().SolutionLocation + @"\Template\" + m_templateDefinition.SelectSingleNode("templatefilename").InnerText;
        }

		private StringCollection LoadTemplateSource()
		{
			XmlNode						src			= m_templateDefinition.ParentNode.LastChild;
			TextReader					r			= null;
            if (! (src is XmlCDataSection))
            {
				r									= new StreamReader(m_inputfilename);
            }
            else
            {
				XmlCDataSection			source		= (src as XmlCDataSection);
				r									= new StringReader(source.Data);
            }
            String                      conceptname = m_templateDefinition.SelectSingleNode("name").InnerText;
            StringCollection            files       = new StringCollection();
            files.Add(m_inputfilename);
            StringCollection			sc			= LoadSourceFromReader(m_inputfilename, r, files, m_FunctionInfo, false, conceptname);
            return sc;
        }

        /// <summary>
        /// Laad het bestand uit de reader in. Noteer voor functies in welk bestand ze zaten.
        /// Let op niet nog een keer hetzelfde bestand te gaan laden. Voor je het weet zit je in een loop.
        /// En een loop mag niet. 
        /// Een includefile mag geen code opleveren. Alleen: lege regels, regels met commentaar, en regels met alleen een enkele '@'
        /// </summary>
        /// <param name="filename">Naam van de file die we nu doen</param>
        /// <param name="reader">De textlezer</param>
        /// <param name="loadedfiles">Lijst files die we al gehad hebben, inclusief de 'filename'</param>
        /// <param name="functioninfo">Lijst te vullen functies</param>
        /// <param name="includefile">Als deze true is gaat het om een includefile</param>
        /// <param name="conceptname">Als geen includefile, dan is conceptname gevuld met de naam van het type waarvoor gegenereerd is. 
        /// De file zelf kan hergebruikt worden in principe meerdere concepten, mits ze gelijke gegevens gebruiken natuurlijk</param>
        /// <returns>De programmacode die niet in functies zat, elke regel voorafgegaan door het regelnummer</returns>
        private StringCollection LoadSourceFromReader(String filename, TextReader reader, StringCollection loadedfiles, Hashtable functioninfo, bool includefile, string conceptname)
        {
			StringCollection			sc			= new StringCollection();
            int                         linenr      = 1;  //linenr update
			string						s			= reader.ReadLine();
			while (s != null)
			{
                if (s.TrimStart().StartsWith("@"))
                {
                    string sLineTrim = s.TrimStart().Substring(1).TrimStart();
                    if (sLineTrim.StartsWith("Function "))
                    {
                        SourceCodeContext fncontext = new SourceCodeContext(conceptname, filename, linenr++);
                        FunctionInfo    fi          = new FunctionInfo(fncontext, s);
                        s                           = reader.ReadLine();
                        while (s != null) // of bij EndFunction eruit gebroken...
                        {
                            sLineTrim               = s.TrimStart();
                            if (sLineTrim.StartsWith("@"))
                            {
                                sLineTrim           = sLineTrim.Substring(1).TrimStart() + " ";
                                if (sLineTrim.StartsWith("EndFunction "))
                                {
                                    linenr++;
                                    break;
                                }
                            }
                            fi.Add(linenr++.ToString() + ":" + s);
                            s                       = reader.ReadLine();
                        }
                        if (s == null)
                        {
                            throw new EndFunctionMissingException(fncontext, fi.Name);
                        }
                        if (functioninfo.Contains(fi.Name))
                        {
                            FunctionInfo otherone   = functioninfo[fi.Name] as FunctionInfo;
                            throw new DuplicateFunctionDefinitionException(fncontext, fi.Name, otherone.DeclaredAt);
                        }
                        functioninfo.Add(fi.Name, fi);
                        s                           = reader.ReadLine();
                        continue;
                    }
                    if (sLineTrim.StartsWith("Include "))
                    {
                        // Verzamel eerst de handel in het aangegeven bestand.
                        // Merge de resultaten met de functioninfo van 'ons'.(check dat er geen dubbele functies in zitten.
                        // En ga verder met de rest.
                        String     includefilename  = sLineTrim.Substring(8).Trim();
                        if (loadedfiles.Contains(includefilename))
                            throw new IncludeFileLoopException(new SourceCodeContext(conceptname, filename, linenr), includefilename);

                        String     includefilepath  = TemplateUtil.Instance().CombineAndCompact(Path.GetDirectoryName(filename), includefilename);
                        if (!File.Exists(includefilepath))
                            throw new IncludeFileNotFoundException(new SourceCodeContext(conceptname, filename, linenr), includefilepath);

                        TextReader          r       = new StreamReader(includefilepath);
                        loadedfiles.Add(includefilename);
                        StringCollection    files   = new StringCollection();
                        foreach (string s1 in loadedfiles)
                            files.Add(s1);
                        Hashtable  includedfncs     = new Hashtable();
                        LoadSourceFromReader(includefilename, r, files, includedfncs, true, "");
                        // Neem nu de gelezen functies over in onze eigen lijst, check op dubbelen.
                        foreach (FunctionInfo fi in includedfncs.Values)
                        {
                            if (functioninfo.ContainsKey(fi.Name))
                            {
                                FunctionInfo otherone = functioninfo[fi.Name] as FunctionInfo;
                                throw new DuplicateFunctionDefinitionException(fi.DeclaredAt, fi.Name, otherone.DeclaredAt);
                            }
                            functioninfo.Add(fi.Name, fi);
                        }
                        //.. en alweer klaar.
                        linenr++;
                        s                           = reader.ReadLine();
                        continue;
                    }
                    if (sLineTrim.StartsWith("--") || sLineTrim == "")
                    {
                        linenr++;
                        s                           = reader.ReadLine();
                        continue;
                    }
                }
                if (includefile)
                {
                    if (s.Trim() != "")
                        throw new IncludeFilesCanOnlyContainFunctionsException(new SourceCodeContext(conceptname, filename, linenr));
                }

                sc.Add(linenr.ToString() + ":" + s);
				s									= reader.ReadLine();
                linenr++;                       
			}
            reader.Close();
			return sc;
		}

		private void MakeSureDirectoryExistsFor(string filename)
		{
            string                      outputdirectory
                                                    = Path.GetDirectoryName(filename);
			m_log.WriteInfo("Output to directory: " + outputdirectory);

			if (!Directory.Exists(outputdirectory))
			{
				m_log.WriteInfo("Directory created");
				Directory.CreateDirectory(outputdirectory);
			}
		}

		private void WriteOutputfile(StringCollection result)
		{
			// Make sure the directory exists, otherwise create it.
			MakeSureDirectoryExistsFor(m_outputfilename);

			// Check if there's something to write.
            bool                        anyContentsToWrite
                                                    = false;
            if (result != null)
            {
                foreach (string s in result)
                {
                    if (s.Trim() != "")
                    {
                        anyContentsToWrite = true;
                        break;
                    }
                }
            }

			if (anyContentsToWrite)
			{
				m_log.WriteInfo("Output to file: " + m_outputfilename + " ... ");
				FileStream				f			= new FileStream(m_outputfilename, FileMode.Create,	FileAccess.Write, FileShare.ReadWrite);
				StreamWriter			o			= new StreamWriter(f);
				foreach (string s in result)
					o.WriteLine(s);
				o.Close();
				f.Close();
				m_log.WriteInfo("... output written");
			}
			else
			{
				m_log.WriteInfo("No output to write");
				// Check if file already exists, and remove it.
				if (File.Exists(m_outputfilename))
				{
					m_log.WriteInfo("Delete output file");
					File.Delete(m_outputfilename);
				}
			}
		}

		/// <summary>
		/// KEEP info
		/// </summary>
		#region Keep area
		private	Hashtable				m_KeepInfo;
		private const string KEEP_START		= "@Keep:";
		private const int	 KEEP_START_LEN	= 6;
		private const string KEEP_END		= "@EndKeep@";

		private string GetKeepKey(string line, int pos)
		{
			string						key			= line.Substring(pos + KEEP_START_LEN);
			//keepupdate
			//key = key.Substring(0, key.LastIndexOf("@"));
			key										= key.Substring(0, key.IndexOf("@"));
			return key;
		}
		/// <summary>
		/// Read the output file to get all codes that must be kept as is.
		/// If output file does not exist, this is easy.
		/// </summary>
		private void InitializeKeepInfo()
		{
			m_KeepInfo								= new Hashtable();
			if (! File.Exists(m_outputfilename))
				return;

			StreamReader				o			= new StreamReader(m_outputfilename);
			string						line		= o.ReadLine();
			while (line != null)
			{
				int						keeppos		= line.IndexOf(KEEP_START);
				if (keeppos >= 0)
				{
					string				key			= GetKeepKey(line, keeppos);
					ArrayList			keeps		= new ArrayList();
					// Build array of lines to keep
					//updatekeep
					/*
					keeps.Add(line);
					do 
					{
						line						= o.ReadLine();
						keeps.Add(line);
					} while (line != null && line.IndexOf(KEEP_END) < 0);
					*/
					line							= line.Substring(line.IndexOf(KEEP_START));
					while (line != null && (keeppos = line.IndexOf(KEEP_END)) < 0) 
					{
						keeps.Add(line);
						line						= o.ReadLine();
					} 

					if (line != null)
					{
						keeps.Add(line.Substring(0, keeppos + KEEP_END.Length));
						line						= line.Substring(keeppos + KEEP_END.Length);
					}

					// Add the array as a string[] in the Keepinfo hashtable
					if (! m_KeepInfo.ContainsKey(key))
						m_KeepInfo.Add(key, keeps.ToArray(typeof(string)));

					if (line != null)
					{
						continue; // process remainder of the line
					}
				}
				if (line != null)
					line							= o.ReadLine();
			}
			o.Close();
		}

        //private bool ProcessKeepInfo(string line)
        //{
        //    if (m_current.state != InterpretationStatus.Active)
        //        return false;

        //    int							iKeepStart	= line.IndexOf(KEEP_START);
        //    if (iKeepStart >= 0)
        //    {
        //        string					key			= GetKeepKey(line, iKeepStart);
        //        int						iKeyStart	= line.IndexOf(key);
        //        //key = this.ProcessLine(key);
        //        key									= m_evaluator.Evaluate(key).ToString();

        //        if (m_KeepInfo.ContainsKey(key))
        //        {
        //            string[]			kept		= (m_KeepInfo[key] as string[]);
        //            // insert the prologue of the keep before the kept text
        //            if (iKeepStart > 0)
        //                kept[0] = line.Substring(0, iKeepStart) + kept[0];
					
        //            // Remove all lines from the template
        //            int					iKeepEnd	= -1;
        //            while (m_source.Count > 0 && (iKeepEnd = line.IndexOf(KEEP_END)) < 0)
        //            {
        //                line						= m_source[0];
        //                m_source.RemoveAt(0);
        //            }
        //            if (iKeepEnd >= 0 && iKeepEnd + KEEP_END.Length < line.Length)
        //            {
        //                kept[kept.Length - 1] += line.Substring(iKeepEnd + KEEP_END.Length);
        //            }
					
        //            m_target.AddRange(kept);
        //            foreach (string s in kept)
        //                m_log.WriteKept(s);
        //        }
        //        else
        //        {
        //            // Not in our kept cache. Process as normal.
        //            // but output start keep with interpreted key.
        //            string				oline		= line.Substring(0,	iKeyStart) + key + line.Substring(line.LastIndexOf("@"));
        //            m_target.Add(oline);
        //            m_log.WriteOutput(oline);
        //        }
        //        return true;

        //    }
        //    if (line.IndexOf(KEEP_END) > 0)
        //    {
        //        m_target.Add(line);
        //        m_log.WriteOutput(line);
        //        return true;
        //    }

        //    return false;
        //}
        #endregion

        //#region Macros area
        //private	Hashtable				m_MacroInfo;

        //public string ProcessMacro(string line)
        //{
        //    string						key;
        //    string						val;

        //    if (m_MacroInfo == null)
        //        m_MacroInfo							= new Hashtable();

        //    int							i			= line.IndexOf(":",	8);
        //    if (i < 0)
        //    {
        //        key									= line.Substring(7).Trim('@').Trim();
        //        if (m_MacroInfo.ContainsKey(key))
        //            m_MacroInfo.Remove(key);
        //    }
        //    else
        //    {
        //        string[]				vals		= new String[2];
        //        key									= line.Substring(7,	i -	7);
        //        val									= line.Substring(i + 1).Trim('@').Trim();
        //        i									= val.IndexOf("==>");
        //        if (i <= 0)
        //        {
        //            if (m_MacroInfo.ContainsKey(key))
        //                m_MacroInfo.Remove(key);
        //        }
        //        else
        //        {
        //            vals[0] = val.Substring(0, i).Trim();
        //            vals[1] = val.Substring(i + 3).Trim();
        //            if (m_MacroInfo.ContainsKey(key))
        //                m_MacroInfo[key] = vals;
        //            else
        //                m_MacroInfo.Add(key, vals);
        //        }
        //    }
        //    return "";
        //}
        //public string ProcessMacros(string line)
        //{
        //    if (m_MacroInfo == null)
        //        return line;

        //    foreach (string[] s in m_MacroInfo.Values)
        //        line								= line.Replace(s[0], s[1]);

        //    return line;
        //}
        //#endregion

        #region Functions area
        private Hashtable m_FunctionInfo = new Hashtable();

        #endregion

        private string GetCurrentTemplatename()
        {
            string result = m_templateDefinition.SelectSingleNode("name").InnerText;
            return result;
        }
        private void WriteError(SyntaxErrorException sex)
        {
            Notify(sex.Message, NotificationType.Erreur, sex.Context);
            m_log.WriteError(sex.Message);
        }

        private void WriteError(string s)
        {

            Notify(s, NotificationType.Erreur, new SourceCodeContext(GetCurrentTemplatename(), m_inputfilename, m_currentlinenr));
            m_log.WriteError(s);
        }
        private void WriteInfo(string s)
        {
            Notify(s, NotificationType.Info, new SourceCodeContext(GetCurrentTemplatename(), m_inputfilename, m_currentlinenr));
            m_log.WriteInfo(s);
        }
    }

	public class CurrentInfo
	{
		public CurrentInfo()
		{
			state									= InterpretationStatus.Active;
		}
        public CurrentInfo(CurrentInfo other)
        {
            state                                   = InterpretationStatus.Active;
            type                                    = other.type;
            val                                     = other.val;
            typetype                                = other.typetype;
        }
		public InterpretationStatus		state;
		
		public XmlNode					type;
		public XmlNode					val;
		public string					typetype;

	}
	public class LoopIterator : IEnumerator
	{
		private	XmlNodeList				m_list;
		private	string					m_orderby;
		private	object					m_current;
		private	ArrayList				m_remainder;

		/// <summary>
		/// Create a new iterator over the list, in the order
		/// as given by the values in the 'orderby' element.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="orderby"></param>
		public LoopIterator(XmlNodeList list, string orderby)
		{
			m_list									= list;
			m_orderby								= orderby;
			Reset();
		}

		/// <summary>
		/// Start again
		/// </summary>
		public void Reset()
		{
			m_current								= null;
			m_remainder								= new ArrayList();
			foreach (XmlNode x in m_list)
				m_remainder.Add(x);
		}

		/// <summary>
		/// Return the current item in the list
		/// </summary>
		public object Current
		{
			get
			{
				if (m_current == null)
					throw new InvalidOperationException("Loopiterator not initialised. Current not set");
				return m_current;
			}
		}
		/// <summary>
		/// Find the XmlNode with the smallest 'orderby' element.
		/// Get the next when 'orderby' is empty.
		/// The one without the orderby element or with illegal 
		/// value in the orderby are considered last
		/// </summary>
		/// <returns>true on succes</returns>
		public bool MoveNext()
		{
			int							minvalue	= int.MaxValue;
			m_current								= null;

			if (m_orderby == "" || m_remainder.Count <= 1)
			{
				if (m_remainder.Count >= 1)
					m_current						= m_remainder[0];
			}
			else
			{
				for (int i = 0; i < m_remainder.Count; i++)
				{
					XmlNode				orderbynode	= (m_remainder[i] as XmlNode).SelectSingleNode(m_orderby);
					if (orderbynode == null)
					{
						m_current					= m_remainder[i];
                        continue;
					}
					string				orderbyvalue= orderbynode.InnerText;
					try
					{
						int				currentvalue= int.Parse(orderbyvalue);
						if (currentvalue < minvalue)
						{
							m_current				= m_remainder[i];
							minvalue				= currentvalue;
						}
					}
					catch
					{
						m_current					= m_remainder[i];
					}
				}
			}
			if (m_current == null)
				return false;
			else
			{
				m_remainder.Remove(m_current);
				return true;
			}
		}
	}
	public class LoopInfo
	{
		public Looptype					type;
		public XmlNodeList				todolist;
		public IEnumerator				iterator;
        
        // For While loops evaluate use the expression
        public string expression; 

        private	StringCollection		m_loopsource;
		public StringCollection	loopsource
		{
			get
			{
				StringCollection		sc			= new StringCollection();
				foreach (string s in m_loopsource)
					sc.Add(String.Copy(s));
				return sc;
			}
			set
			{
				m_loopsource						= new StringCollection();
				foreach (string s in value)
					m_loopsource.Add(String.Copy(s));

			}
		}
		public LoopInfo(Looptype t, XmlNodeList tlist, string orderby, StringCollection source)
		{
			type									= t;
			todolist								= tlist;
			if (todolist == null)
				iterator							= null;
			else
				iterator							= new LoopIterator(tlist, orderby);
			loopsource								= source;
		}
        public LoopInfo(Looptype t, string anExpression, StringCollection source)
        {
            type = t;
            todolist = null;
            iterator = null;
            expression = anExpression;
            loopsource = source;
        }


	}

}
