using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class OperatorExpectedException : SyntaxErrorException
    {
        public OperatorExpectedException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("Operator expected, not '{0}'", line);
        }

    }
}
