using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    public class UnreachableCodeException : SyntaxErrorException
    {
        public UnreachableCodeException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Unreachable code detected");
            _info_only = true;
        }
    }
}