using System;
using System.Collections;
using System.Text;
using Generator.Expressions;
using Generator.Statements;

namespace Generator.Exceptions
{
    class OperationNotDefinedException : SyntaxErrorException
    {
        public OperationNotDefinedException(SourceCodeContext line, string op, string op1, ArrayList arglist)
            : base(line, op)
        {
            _msg = String.Format("Operator '{0}' not defined for type '{1}'", op, op1);
            if (arglist.Count > 1)
            {
                _msg += " and type ";
                if (arglist.Count > 2)
                {
                    foreach (Var v in arglist.GetRange(1, arglist.Count - 2))
                        _msg += "'" + v.TypeName() + "',";
                }
                _msg += "'" + (arglist[arglist.Count - 1] as Var).TypeName() + "'";
            }

        }
    }
}
