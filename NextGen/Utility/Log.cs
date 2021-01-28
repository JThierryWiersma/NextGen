/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) 2008  Thierry Wiersma													*
*****************************************************************************************/
using System;
using System.Configuration;
using System.IO;

namespace Generator.Utility
{
	/// <summary>
	/// Summary description for Log.
	/// </summary>
	public class Log
	{
		private	bool					m_active;
		private	string					m_logfilename;
		private	StreamWriter			m_logfile;
		public Log(string filename)
		{
            AppSettingsReader           asr         = new AppSettingsReader();
			m_active								= false;

            // Bekijk wat de logdir moet zijn.
		    String                      logdir		= @"C:\TEMP";
            try
            {
                logdir                              = (String)asr.GetValue("Logdir", typeof(String));
            }
            catch (InvalidOperationException)
            {
                // ignore;
            }
            if (logdir == "")
                logdir                              = @"C:\TEMP";

            // ... en concludeer wat de filenaam van de log moet zijn.
			m_logfilename							= Path.Combine(logdir, filename);

			// Bekijk of we wel willen loggen of niet
            string                      log         = "";
            try
            {
                log                                 = (String)asr.GetValue("Log", typeof(String));
            }
            catch (InvalidOperationException)
            {
                // ignore;
            }
            // Forceer openen van de logfile door gebruik te maken van de 'active' property
            active = (log == "1");
		}

		/// <summary>
		/// Get/set for active logging. Set only has effect if value
		/// other than previous. Active logging has an logfile open.
		/// Deactivating the logfile closes it.
		/// </summary>
		public bool active
		{
			get
			{
				return m_active;
			}
			set
			{
				if (value != m_active)
				{
					m_active						= value;
					// Active logging, open the logfile
					if (m_active)
					{
						if (!Directory.Exists(Path.GetDirectoryName(m_logfilename)))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(m_logfilename));
						}
						FileStream		f			= new FileStream(m_logfilename,	FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
						m_logfile					= new StreamWriter(f);
						m_logfile.AutoFlush			= true;
					}
					else
					{
						// Not active logging, close the logfile.
						m_logfile.Close();
						m_logfile					= null;
					}
				}
			}
		}

		/// <summary>
		/// Get/set the logfilename. Do it before activating
		/// </summary>
		public string logfilename
		{
			get
			{
				return m_logfilename;
			}
			set
			{
				m_logfilename						= value;
			}
		}
		/// <summary>
		/// Close the logfile (when active logging)
		/// </summary>
		public void Close()
		{
			if (m_active)
			{
				m_logfile.Close();
				m_active							= false;
				m_logfilename						= "";
			}
		}

		public void Write(int level, InterpretationStatus status, string line)
		{
			if (!m_active)
				return;

			string						statusstring= "";
			switch (status)
			{
				case InterpretationStatus.Active	: statusstring = "+";break;
				case InterpretationStatus.Skip		: statusstring = "o";break;
				case InterpretationStatus.Inactive	: statusstring = "-";break;
			}

			m_logfile.WriteLine("t:{0} {1} {2}", level, statusstring, line);
		}

		public void WriteOutput(string line)
		{
			if (!m_active)
				return;

			m_logfile.WriteLine("    > {0}", line);
		}
		public void WriteKept(string line)
		{
			if (!m_active)
				return;

			m_logfile.WriteLine("KEPT> {0}", line);
		}
		public void WriteError(string line)
		{
			if (!m_active)
				return;

			m_logfile.WriteLine("ERROR {0}", line);
		}
		public void WriteInfo(string line)
		{
			if (!m_active)
				return;

			m_logfile.WriteLine("INFO> {0}", line);
		}
	}
}
