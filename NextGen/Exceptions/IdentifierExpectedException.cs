using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IdentifierExpectedException : SyntaxErrorException
    {
        public IdentifierExpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Identifier expected");
        }
    }
}
