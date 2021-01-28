/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/
using System.IO;
using System.Configuration;

namespace Generator.Utility
{
	/// <summary>
	/// Summary description for TemplateUtil.
	/// </summary>
	public class TemplateUtil
	{
		private	static TemplateUtil		m_instance;
		private TemplateUtil()
		{
		}

		/// <summary>
		/// Return the singular instance
		/// </summary>
		/// <returns></returns>
		public static TemplateUtil Instance()
		{
			if (m_instance == null)
				m_instance							= new TemplateUtil();
			return m_instance;
		}

		public string InventCaptionForName(string name)
		{	
			string						caption		= "";
			foreach (string p in name.Split('_'))
				caption								+= " " + p.Substring(0, 1).ToUpper() + p.Substring(1);
			return caption.TrimStart();
		}

		/// <summary>
		/// Get the name of the default solution when available. Otherwise return "".
		/// </summary>
		/// <remarks></remarks>
		/// <returns></returns>
		public string GetDefaultSolution()
		{
            AppSettingsReader asr = new AppSettingsReader();
			string						default_solution
													= (string) asr.GetValue("DefaultSolution", typeof(string));
			
			if (default_solution == null)
				default_solution					= "";

			return default_solution;
		}


		/// <summary>
		/// Return the absolute path (without last '\') given an path. The given path
		/// may be without root, in which case it is prepended with the applications
		/// directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string AbsolutePath(string path)
		{
			if (Path.IsPathRooted(path))
				return path;
			else
				return Path.Combine(AppPath(), path);
		}

		/// <summary>
		/// Combine path2 if relative with path1 to get an abolute path
		/// </summary>
		/// <param name="path1"></param>
		/// <param name="path2"></param>
		/// <returns></returns>
		public string AbsolutePath(string path1, string path2)
		{
			if (Path.IsPathRooted(path2))
				return path2;
			else
				return Path.Combine(Path.GetDirectoryName(path1), path2);
		}
		/// <summary>
		/// Return the directory where the application is stored
		/// </summary>
		/// <returns></returns>
		public string AppPath()
		{
			return System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
		}

        public void MakeSureDirectoryExistsFor(string filename)
        {
            string outputdirectory = Path.GetDirectoryName(filename);

            if (!Directory.Exists(outputdirectory))
            {
                Directory.CreateDirectory(outputdirectory);
            }
        }

        /// <summary>
        /// Combine the path1 with path2, by stepping up every subdirectory in path1
        /// for each ..\ found in path2. In the end, combine both paths using the Path.Combine.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public string CombineAndCompact(string path1, string path2)
        {
            string result = "";
            if (path2.StartsWith("..\\"))
            {
                string dir = path1;
                while (path2.StartsWith("..\\"))
                {
                    path2 = path2.Substring(3);
                    dir = Path.GetDirectoryName(dir);
                }
                result = Path.Combine(dir, path2);
            }
            else
            {
                result = Path.Combine(path1, path2);
            }
            return result;
        }
	}
}
