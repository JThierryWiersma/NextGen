using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ParenthesisCloseExpectedException : SyntaxErrorException
    {
        public ParenthesisCloseExpectedException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("Closing parenthesis expected instead of '{0}'", line);
        }

    }
}
