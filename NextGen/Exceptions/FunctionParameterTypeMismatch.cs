using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class FunctionParameterTypeMismatch : SyntaxErrorException
    {
        public FunctionParameterTypeMismatch(
            SourceCodeContext linenr,
            string functionname,
            int parameterindex,
            string formaltype,
            string actualtype)
            : base(linenr)
        {
            _msg = String.Format("Function call to {3} parameter {0} type mismatch; Expected '{1}' but a '{2}' was supplied.", parameterindex, formaltype, actualtype, functionname);
        }
    }
}
