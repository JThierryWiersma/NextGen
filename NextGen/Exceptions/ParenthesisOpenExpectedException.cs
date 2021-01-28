using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ParenthesisOpenExpectedException : SyntaxErrorException
    {
        public ParenthesisOpenExpectedException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("Opening parenthesis expected instead of '{0}'", line);
        }

    
    }
}
