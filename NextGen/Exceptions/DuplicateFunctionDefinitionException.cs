using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class DuplicateFunctionDefinitionException: SyntaxErrorException
    {
        public DuplicateFunctionDefinitionException(SourceCodeContext linenr, string functionname, SourceCodeContext other)
            : base(linenr, functionname)
        {
            _msg = String.Format("Duplicate function definition for function {0}. Already declared at line {1} of file {2}", functionname, other.Linenr, other.Filename);
        }
    }
}
