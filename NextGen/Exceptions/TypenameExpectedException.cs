using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class TypenameExpectedException : SyntaxErrorException
    {
        public TypenameExpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Typename (Decimal, String, Boolean or ' <<Concept>>') expected");
        }
    }
}
