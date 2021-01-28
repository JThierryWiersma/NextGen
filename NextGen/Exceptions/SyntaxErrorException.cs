using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    public class SyntaxErrorException : ApplicationException
    {
        protected string            _textinerror;
        protected SourceCodeContext _context;
        protected string            _msg            = "Syntax error";
        protected bool              _info_only      = false;

        public SyntaxErrorException(SourceCodeContext context, string textinerror)
            : this(context)
        {
            _textinerror                            = textinerror;
            if (textinerror != null && textinerror != "")
                _msg                                += ":" + _textinerror;
        }

        public SyntaxErrorException(SourceCodeContext context)
        {
            _context                                = context;
            _msg                                    = "Syntax error";
            _textinerror                            = "";
        }
        public override string Message
        {
            get
            {
                return _msg;
            }
        }
        public int Linenr
        {
            get
            {
                return _context.Linenr;
            }
        }
        public string Filename
        {
            get
            {
                return _context.Filename;
            }
        }
        public SourceCodeContext Context
        {
            get
            {
                return _context;
            }
        }
        public bool InfoOnly
        {
            get
            {
                return _info_only;
            }
        }
    }

}
