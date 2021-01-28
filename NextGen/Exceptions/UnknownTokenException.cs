using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class UnknownTokenException : SyntaxErrorException
    {
        public UnknownTokenException(SourceCodeContext linenr, string textinerror)
            : base(linenr, textinerror)
        {
        }
        public override string Message
        {
            get
            {
                return String.Format("Unknown token '{0}'", _textinerror);
            }
        }
    }
}
