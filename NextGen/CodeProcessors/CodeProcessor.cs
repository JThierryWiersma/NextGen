using System;
using System.Collections.Generic;
using System.Text;
using Generator.VisitorPattern;
using Generator.Utility;
using Generator.Statements;

namespace Generator.CodeProcessors
{
    public abstract class CodeProcessor 
    {
        public CodeProcessor()
        {
        }

        protected bool IsClassnameKnown(string name)
        {
            if (name == "Decimal" ||
                name == "String" ||
                name == "Boolean")
            {
                return true;
            }
            if (Array.IndexOf(TemplateCache.Instance().GetTypenamesList("TypeDefs"), name) >= 0)
            {
                return true;
            }
            return false;

        }
    }
}
