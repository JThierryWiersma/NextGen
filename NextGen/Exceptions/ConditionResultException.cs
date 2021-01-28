using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ConditionResultException : SyntaxErrorException
    {
        public ConditionResultException(SourceCodeContext linenr, string textinerror)
            : base(linenr, textinerror)
        {
        }

        public override string Message
        {
            get
            {
                return "Condition result should be non-Empty Boolean ";
            }
        }
    }
}
