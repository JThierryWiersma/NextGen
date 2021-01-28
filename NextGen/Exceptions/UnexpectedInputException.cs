using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    public class UnexpectedInputException : SyntaxErrorException
    {
        public UnexpectedInputException(SourceCodeContext linenr, string text)
            : base(linenr, text)
        {
            _msg = String.Format("Unexpected input found: {0}", text);
        }
    }
}
