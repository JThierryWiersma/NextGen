using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Generator.Exceptions;

namespace Generator.Expressions
{
    public class StringVar : Var
    {
        public string v;
        public override string ToString()
        { 
            return v; 
        }
        public StringVar()
        {
            v = "";
            empty = true;
        }
        public StringVar(string s)
        {
            v = s;
            empty = (s == String.Empty);
        }
        public StringVar(StringVar s)
        {
            name = s.name;
            v = s.v;
            empty = s.empty;
        }
        public override string TypeName()
        {
            return "String";
        }

        public override void Clear()
        {
            v = "";
            base.Clear();
        }

        public override bool Equals(object o)
        { 
            return (o is StringVar && ((o as StringVar).v == v)); 
        }
        public override int GetHashCode()
        { 
            return v.GetHashCode(); 
        }
        public static StringVar operator +(StringVar a, StringVar b)
        {
            if (b.empty)
                return new StringVar(a);
            else
                return new StringVar(a.v + b.v);
        }
        public static void /*StringVar*/ op_AssignAddition(StringVar a, StringVar b)
        {
            if (!b.empty)
            {
                a.v += b.v;
                a.empty = false;
            }
            return;
        }
        public static BooleanVar operator ==(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v == b.v); 
        }
        public static BooleanVar operator !=(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v != b.v); 
        }
        public static BooleanVar op_CaseInsensitiveEquality(StringVar a, StringVar b)
        {
            return new BooleanVar(a.v.ToLower() == b.v.ToLower());
        }
        public static BooleanVar op_CaseInsensitiveInEquality(StringVar a, StringVar b)
        {
            return new BooleanVar(a.v.ToLower() != b.v.ToLower());
        }
        public static BooleanVar operator <(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v.CompareTo(b.v) < 0); 
        }
        public static BooleanVar operator >(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v.CompareTo(b.v) > 0); 
        }
        public static BooleanVar operator <=(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v.CompareTo(b.v) <= 0); 
        }
        public static BooleanVar operator >=(StringVar a, StringVar b)
        { 
            return new BooleanVar(a.v.CompareTo(b.v) >= 0); 
        }
        public static void /*StringVar*/ op_Assign(StringVar a, StringVar b)
        {
            a.v = b.v;
            a.empty = b.empty;
            return;
        }
        public static StringVar op_Id(StringVar a)
        {
            StringVar               result          = new StringVar(a);
            Regex                   r               = new Regex(@"\W");
            result.v                                = r.Replace(result.v, "_");

            return result;
        }
        public static StringVar op_ShiftLeft(StringVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'<<'-operator");

            string                  extraspace      = "";

            if ((b.v > 0) && (b.v - a.v.Length > 0))
                extraspace                          = new string(' ', (int)b.v - a.v.Length);

            return new StringVar(a.v + extraspace);
        }
        public static StringVar op_ShiftRight(StringVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'>>'-operator");

            string                  extraspace      = "";

            if ((int)b.v - a.v.Length > 0)
                extraspace                          = new string(' ', (int)b.v - a.v.Length);

            return new StringVar(extraspace + a.v);
        }
        public static StringVar op_Casing(StringVar a, StringVar b)
        {
            if (a.v == "")
            {
                return new StringVar("");
            }
            switch (b.v)
            {
                case "U": 
                    return new StringVar(a.v.ToUpper());

                case "L": 
                    return new StringVar(a.v.ToLower());

                case "C":
                case "P":
                    string          result          = "";
                    string[]        parts           = a.v.Split(new char[] { ' ', '_' });
                    foreach (string s in parts)
                    {
                        result                      += s[0].ToString().ToUpper();
                        if (s.Length > 1)
                            result                  += s.Substring(1);//.ToLower();
                    }
                    // camel case is pascal case but with lower first char
                    if (result.Length > 0 && b.v == "C")
                    {
                        result = result[0].ToString().ToLower() + result.Substring(1);
                    }
                    return new StringVar(result);
                default:
                    throw new StringCasingException(b.v);
            }
        }

        public static StringVar op_Replace(StringVar what, StringVar from, StringVar into)
        {
            return new StringVar(what.v.Replace(from.v, into.v));
        }
        public static StringVar op_Substring(StringVar what, DecimalVar from, DecimalVar into)
        {
            if (from.v + into.v <= what.v.Length)
            {
                return new StringVar(what.v.Substring((int)from.v, (int)into.v));
            }
            else if (from.v < what.v.Length)
            {
                return new StringVar(what.v.Substring((int)from.v));
            }
            else
            {
                return new StringVar();
            }

        }
        public static DecimalVar op_Length(StringVar what)
        {
            return new DecimalVar(what.v.Length);
        }
        public static DecimalVar op_In(StringVar a, StringVar b)
        {
            return (new DecimalVar(a.v.IndexOf(b.v)));
        }
        public static DecimalVar op_ToDecimal(StringVar a)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'Decimal'-function");

            DecimalVar r = new DecimalVar();
            Decimal.TryParse(a.v, out r.v);

            return r;
        }
        public static BooleanVar op_Empty(StringVar a)
        {
            return new BooleanVar(a.empty);
        }
        public override Var CreateClone()
        {
            return new StringVar(this);
        }
    }
}
