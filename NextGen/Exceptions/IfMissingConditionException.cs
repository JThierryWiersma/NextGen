using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IfMissingConditionException : SyntaxErrorException
    {
        public IfMissingConditionException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("If command requires condition");
        }
    }
}
