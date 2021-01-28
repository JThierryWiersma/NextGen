using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class StringTerminatorExpectedException : SyntaxErrorException
    {
        public StringTerminatorExpectedException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("String terminator expected");
        }

    }
}
