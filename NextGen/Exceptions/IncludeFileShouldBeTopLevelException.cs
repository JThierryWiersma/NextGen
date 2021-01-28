using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IncludeFileShouldBeTopLevelException : SyntaxErrorException
    {
        public IncludeFileShouldBeTopLevelException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Include must be unconditional and not in function"); 
        }
  
    }
}
