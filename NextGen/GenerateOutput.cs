/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) 2008  Thierry Wiersma													*
*****************************************************************************************/
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Generator.Utility;
using System.Threading;
using Generator.ObserverPattern;
using Generator.Exceptions;
using Generator.Statements;

namespace Generator
{
    //public delegate void AddToTextBox(string firstpart, string linkeditem, string lastpart, string filename, int linenr);
    
	/// <summary>
	/// Summary description for GeneratorOutput.
	/// </summary>
	public class GenerateOutput : ISubject, IObserver
	{
        private const string CRLF = "\x0D\x0A";

        // private AddToOutputTextCallback     m_textboxadddelegate;
		private	List<GenerationRequest>	    m_todolist;
        private bool                        m_ready;
    
        /*
		/// <summary>
		/// Constructor which will put output in the given textbox
		/// </summary>
		/// <param name="txt"></param>
		public GenerateOutput(AddToOutputTextCallback textboxadddelegate)
		{
            m_textboxadddelegate                    = textboxadddelegate;
			m_todolist								= null;
			cm_instance								= this;
		} */
		/// <summary>
		/// Constructor to get output to stdout.
		/// </summary>
		public GenerateOutput()
		{
			m_todolist								= null;
			cm_instance								= this;
		}

		public static GenerateOutput Instance()
		{
			if (cm_instance == null)
			{
                cm_instance = new GenerateOutput();// throw new ApplicationException("No GenerateOutput instance set!");
			}
			return cm_instance;
		}

		private static GenerateOutput cm_instance;

		public static void OutputToInstance(string line)
		{
			if (cm_instance != null)
			{
				cm_instance.Output(line);
			}
		}

		public static void OutputToInstance(string[] lines)
		{
			if (cm_instance != null)
			{
				cm_instance.Output(lines);
			}
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
        public void Notify(string msg, ObserverPattern.NotificationType t, SourceCodeContext scc)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, msg, t, scc);
        }
        public void Notify(string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            foreach (IObserver o in m_observers)
                o.ProcessUpdate(this, firstpart, linkeditem, lastpart, filename, linenr);
        }
        #endregion
        #region Observer pattern: IObserver (observeert TemplateGenerators)
        public void ProcessUpdate(object o)
        {
            // Gewoon doorsturen?
            Notify();
        }
        public void ProcessUpdate(object o, string msg, ObserverPattern.NotificationType t, SourceCodeContext scc)
        {
            Notify(msg, t, scc);
        }
        public void ProcessUpdate(object o, string firstpart, string linkeditem, string lastpart, string filename, int linenr)
        {
            Notify(firstpart, linkeditem, lastpart, filename, linenr);
        }

        #endregion

        public void AddGenerationRequests(List<GenerationRequest> todolist)
        {
            if (m_todolist == null)
                m_todolist = new List<GenerationRequest>();

            lock (m_todolist)
            {
                m_todolist.AddRange(todolist);
            }
            Notify();
        }

        public void Reset(List<GenerationRequest> todolist)
		{
			// If necessay, abort running job.
			if (m_GeneratorThread != null)
			{
				if (m_GeneratorThread.IsAlive)
				{
					m_GeneratorThread.Abort();
					m_GeneratorThread.Join();
				}
				m_GeneratorThread					= null;
			}

			// Reset controls
			/*if (txtOutput != null)
				txtOutput.Clear();
            */
            AddGenerationRequests(todolist);
		}

		private	Thread					m_GeneratorThread;
        public Thread thread
        {
            get
            {
                return m_GeneratorThread;
            }
        }
        /// <summary>
        /// Geef het aantal requests, inclusief die waar we nu aan bezig zijn.
        /// </summary>
        /// <returns></returns>
        public int FilesToDo()
        {
            return m_todolist.Count + (m_ready ? 0 : 1);
        }
        public bool IsRunning
        {
            get
            {
                return (m_GeneratorThread != null &&
                    m_GeneratorThread.IsAlive);
            }
        }

		private void Output(string firstpart, string linkeditem, string lastpart, string filename, int linenr)
		{
            Notify(firstpart, linkeditem, lastpart, filename, linenr);
 /*
            if (m_textboxadddelegate != null)
            {
                TemplateMain.Instance().Invoke(m_textboxadddelegate, new object[] { firstpart, linkeditem, lastpart, filename, linenr });
                //main invoke.m_textboxadddelegate(firstpart, linkeditem, lastpart, filename, linenr);
            }
            else
            {
                Console.WriteLine(firstpart);
            }
  * */
		}
        private void Output(string line)
        {
            Output(line, null, null, null, 0);
        }
		private void Output(string[] lines)
		{
			Output(String.Join(CRLF + "  ", lines));
		}

		public void Run()
		{
			int							successcount= 0;
			int							errorcount	= 0;
            m_ready = false;
			try
			{

				while (m_todolist.Count > 0)
				{
                    GenerationRequest t = m_todolist[0];
                    m_todolist.RemoveAt(0);

                    Notify();// TemplateMain.Instance().GeneratorUpdate();

					try
					{
						Output("Start generating " + t.m_label + ", " + t.m_definition.SelectSingleNode("name").InnerText);
                        TemplateGenerator 
										g			= new TemplateGenerator(t.m_type, t.m_val, t.m_definition);
                        g.AddObserver(this);
                        g.Generate();

						if (g.m_errors > 0)
							errorcount++;
						else
							successcount++;

                        g.RemoveObserver(this);
                        // Add link to outputfile in outputwindow.
                        Output("Generated file: ", g.GetOutputfilename(), null, g.GetOutputfilename(), 0);
						g							= null;
					}
					catch (Exception ex)
					{
						Output("Exception: " + ex.Message);
						errorcount++;
					}
				}
			}
			catch (Exception ex2)
			{
				Output("General exception: " + ex2.Message);
			}

			string sMsg = "Next Generation ready.\n" 
				+ successcount + " files successfully generated,\n"
				+ errorcount + " files reported errors.";
			Output(new string[] {
				"------------------------------- Done -------------------------------",
				"Generated, total: " + (successcount + errorcount) + " files, " + errorcount + " containing errors."
			});

            m_ready = true;
			//MessageBox.Show(txtOutput.Parent, sMsg, "Generation result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Notify();// TemplateMain.Instance().NotiGeneratorReady();
        }

		/// <summary>
		/// If generator still running, abort it, wait for it to stop.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GeneratorOutput_Closed(object sender, System.EventArgs e)
		{
			if (m_GeneratorThread != null && m_GeneratorThread.IsAlive)
			{
				m_GeneratorThread.Abort();
				m_GeneratorThread.Join();
				m_GeneratorThread					= null;
                m_ready = true;
                Notify();
			}
			cm_instance								= null;
		}

		public void GO()
		{
            //this.Run();
			m_GeneratorThread						= new Thread(new ThreadStart(this.Run));
			m_GeneratorThread.Start(); //*/
		}
		public void GOBATCH()
		{
			this.Run();
		}
	}
}

