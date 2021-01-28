
namespace Generator.Test
{
	/// <summary>
	/// Summary description for TemplateGenerator.
	/// </summary>
    internal class Resources
    {

        public void Generate(List<GenerationRequest> todo, int Value)
        {
            v.InnerText = (c as NumericUpDown).Value.ToString();

            ShowOutputText();

            GenerateOutput.Instance().AddGenerationRequests(todo);
            GenerateOutput.Instance().AddObserver(this);

            if (!GenerateOutput.Instance().IsRunning)
                GenerateOutput.Instance().GO();
        }
        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Generator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
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
