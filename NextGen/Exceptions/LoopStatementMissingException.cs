using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class LoopStatementMissingException : SyntaxErrorException
    {
        public LoopStatementMissingException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Do/While not matched with Loop"); 
        }
  
    }
}
