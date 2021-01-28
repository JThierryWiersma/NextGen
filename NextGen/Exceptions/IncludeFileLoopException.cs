using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IncludeFileLoopException : SyntaxErrorException
    {
        public IncludeFileLoopException(SourceCodeContext line, string name)
            : base(line, name)
        {
            _msg = String.Format("Include file loop detected for file {0}", name);
        }
    }
}
