using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class UnknownCommandException : SyntaxErrorException
    {
        public UnknownCommandException(SourceCodeContext linenr, string textinerror)
            : base(linenr, textinerror)
        {
        }
        public override string Message
        {
            get
            {
                return String.Format("Unknown command '{0}'", _textinerror);
            }
        }
    }
}
