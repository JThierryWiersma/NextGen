/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using Generator.Utility;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;

namespace Generator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //string[] args = Environment.GetCommandLineArgs();
            bool interactive = true;

            //Welcome w = new Welcome();
            //w.ShowDialog();
            //if (!Registry.Instance().RegistrationInfo().Valid)
            //    return;
            //
            /*
            ArrayList todo = new ArrayList();
            try
            {
                TemplateCache.Clear(true);
                // first walk all non-generate arguments to select the right project
                for (int argc = 1; argc < args.GetLength(0); argc++)
                {
                    string arg = args[argc];

                    if (arg.EndsWith(".xmp"))
                    {
                        TemplateMain.LoadProjectFile(arg);
                        args[argc] = "";
                    }
                    else if (arg.EndsWith(".xms"))
                    {
                        TemplateMain.LoadSolutionFile(arg);
                        args[argc] = "";
                    }
                }
                // If still we do not know the solution, take the default
                if (TemplateCache.Solution == "")
                {
                    string default_solution
                                                    = TemplateUtil.Instance().GetDefaultSolution();
                    TemplateMain.LoadSolutionFile(default_solution);
                }

                for (int argc = 1; argc < args.GetLength(0); argc++)
                {
                    string arg = args[argc];

                    if (arg == "")
                        continue;

                    if (arg.StartsWith("-u"))
                    {
                        Win32.StartConsole();
                        string[] p = arg.Split(':');
                        if (p.Length == 1)
                            todo.AddRange(TemplateMain.GenerateUserConcept("", ""));
                        else if (p.Length == 2)
                            todo.AddRange(TemplateMain.GenerateUserConcept(p[1], ""));
                        else if (p.Length == 3)
                            todo.AddRange(TemplateMain.GenerateUserConcept(p[1], p[2]));
                        else
                            throw new ApplicationException("-u option: pass concept name and optional instance name separated by a colon (:)");
                        interactive = false;
                    }
                    else //if (arg.StartsWith("-?"))
                    {
                        string msg = "Usage: nextgen projectfile [-?] {[-u[:concept[:instancename]]]...}";
                        if (Win32.HasConsole())
                            Console.WriteLine(msg);
                        else
                            MessageBox.Show(msg);

                        interactive = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Win32.StartConsole();
                Console.WriteLine("Error during startup: " + ex.Message);
                return;
            }
            */
            if (interactive)
            {
                TemplateMain tm = TemplateMain.Instance();
                Application.Run(tm);
            }
            /* else
            {
                GenerateOutput o = new GenerateOutput();
                o.Reset(todo.ToArray(typeof(GenerationRequest)) as GenerationRequest[]);
                o.GOBATCH();
                Console.WriteLine("Exit batch mode");
            } */

        }
    }
}