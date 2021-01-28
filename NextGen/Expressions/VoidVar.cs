using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Expressions
{
    class VoidVar : Var
    {
        public override string ToString()
        {
                return "";
        }
        public VoidVar()
        {
        }
        public override string TypeName()
        {
            return "Void";
        }
        public override Var CreateClone()
        {
            return new VoidVar();
        }
    }
}
