using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class VariableDeclarationException : SyntaxErrorException
    {
        public VariableDeclarationException(SourceCodeContext linenr, string textinerror)
            : base(linenr, textinerror)
        {
            _msg = String.Format("Var declaration: Var {type} {name} [= {initialisation expression}]");
        }
    }
}
