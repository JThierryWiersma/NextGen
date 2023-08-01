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

            TemplateMain tm = TemplateMain.Instance();
            Application.Run(tm);
        }
    }
}