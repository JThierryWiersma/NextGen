using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class OperandExpectedException : SyntaxErrorException
    {
        public OperandExpectedException(SourceCodeContext linenr, string line)
            : base(linenr, line)
        {
            _msg = String.Format("Operand expected at '{0}'", line);
        }

    }
}
