using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Statements
{
    public class SourceCodeContext
    {
        string _f;
        int _l;
        string _c;

        public string Filename
        {
            get
            {
                return _f;
            }
        }
        public string Conceptname
        {
            get
            {
                return _c;
            }
        }
        public int Linenr
        {
            get
            {
                return _l;
            }
            set
            {
                _l = value;
            }
        }
        public SourceCodeContext(string c, string f, int l)
        {
            _c = c;
            _f = f;
            _l = l;
        }
    }
}
