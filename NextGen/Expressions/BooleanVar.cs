using System;
using System.Collections.Generic;
using System.Text;
using Generator.Exceptions;

namespace Generator.Expressions
{
    public class BooleanVar : Var
    {
        public bool v;
        public override string ToString()
        {
            if (empty)
                return "";
            return v.ToString();
        }
        public BooleanVar()
        {
            v = false;
            empty = true;
        }
        public BooleanVar(bool b)
        {
            v = b;
            empty = false;
        }
        public BooleanVar(string s)
        {
            if (s.Trim() == "0" || s.Trim().ToLower() == "false" || s.Trim() == "")
                v = false;
            else
                v = true;
            empty = (s.Trim() == "");
        }
        public BooleanVar(BooleanVar s)
        {
            name = s.name;
            v = s.v;
            empty = s.empty;
        }

        public override void Clear()
        {
            v = false;
            base.Clear();
        }
        public override string TypeName()
        {
            return "Boolean";
        }
        public override bool Equals(object o)
        { 
            return (o is BooleanVar && ((o as BooleanVar).v == v)); 
        }
        public override int GetHashCode()
        { 
            return v.GetHashCode(); 
        }
        public static BooleanVar operator ==(BooleanVar a, BooleanVar b)
        { 
            return new BooleanVar((a.v == b.v) && (a.empty == b.empty)); 
        }
        public static BooleanVar operator !=(BooleanVar a, BooleanVar b)
        { 
            return new BooleanVar((a.v != b.v) || (a.empty != b.empty)); 
        }
        public static BooleanVar op_OrOperator(BooleanVar a, BooleanVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'||'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'||'-operator");

            return new BooleanVar(a.v || b.v); 
        }
        public static BooleanVar op_AndOperator(BooleanVar a, BooleanVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'&&'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'&&'-operator");

            return new BooleanVar(a.v && b.v); 
        }
        public static void /*BooleanVar*/ op_Assign(BooleanVar a, BooleanVar b)
        {
            a.v = b.v;
            a.empty = b.empty;
            return;
        }
        public static BooleanVar op_NotOperator(BooleanVar a)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'Not'-function");

            return new BooleanVar(!a.v);
        }
        public static BooleanVar op_Empty(BooleanVar a)
        {
            return new BooleanVar(a.empty);
        }
        public static StringVar op_If(BooleanVar a, StringVar b, StringVar c)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'If'-function");

            return new StringVar(a.v ? b.v : c.v); 
        }
        public static DecimalVar op_If(BooleanVar a, DecimalVar b, DecimalVar c)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'If'-function");

            return new DecimalVar(a.v ? b.v : c.v); 
        }

        public override Var CreateClone()
        {
            return new BooleanVar(this);
        }
    }
}
