using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class FunctionDeclarationException : SyntaxErrorException
    {
        public FunctionDeclarationException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("Function declaration error");
        }
    }
}
