using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    public class BreakShouldBeInLoopException : SyntaxErrorException
    {
        public BreakShouldBeInLoopException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Break statement requires a Do or While to break from");
        }
    }
}
