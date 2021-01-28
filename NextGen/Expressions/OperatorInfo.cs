using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Generator.Expressions
{
    public class OperatorInfo
    {
        public OperatorInfo(string n, int p, string m)
            : this(n, p, m, false, false)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Name</param>
        /// <param name="p">Precedence (98 = function)</param>
        /// <param name="m">Methodname to use</param>
        /// <param name="u">Unary (nog niet in gebruik)</param>
        /// <param name="f">Function</param>
        public OperatorInfo(string n, int p, string m, bool u, bool f)
        {
            name = n;
            precedence = p;
            methodname = m;
            unary = u;
            function = f;
            m_isText = n == "" ? false : Char.IsLetter(n, 0);
            parameters = new List<Var>();
        }
        public string name;
        public int precedence;
        public string methodname;
        public bool unary;
        public bool function;
        private bool m_isText;
        public List<Var> parameters;

        public OperatorInfo Kloon()
        {
            OperatorInfo result = new OperatorInfo(name, precedence, methodname, unary, function);
            parameters = new List<Var>();
            return result;
        }

        public bool IsText
        {
            get
            {
                return m_isText;
            }
        }

        public static ListDictionary cvOperators;

		/// <summary>
		/// Laatste update: de prioriteiten van 40 op 35 gezet voor de vergelijkings-operatoren
        /// en ~= toegevoegd voor case-insensitive string compare
		/// </summary>
        static OperatorInfo()
		{
			cvOperators								= new ListDictionary();
			cvOperators["+="]	= new OperatorInfo(@"+=", 10, "op_AssignAddition");
			cvOperators["-="]	= new OperatorInfo(@"-=", 10, "op_AssignSubtraction");
			cvOperators["*="]	= new OperatorInfo(@"*=", 10, "op_AssignMultiply");
			cvOperators["/="]	= new OperatorInfo(@"/=", 10, "op_AssignDivision");
			cvOperators["*" ]	= new OperatorInfo(@"*" , 90, "op_Multiply");
			cvOperators["/" ]	= new OperatorInfo(@"/" , 90, "op_Division");
			cvOperators["+" ]	= new OperatorInfo(@"+" , 50, "op_Addition");
			cvOperators["-" ]	= new OperatorInfo(@"-" , 50, "op_Subtraction");
			cvOperators["=="]	= new OperatorInfo(@"==", 35, "op_Equality");
            cvOperators["~~"]   = new OperatorInfo(@"~~", 35, "op_CaseInsensitiveEquality");
            cvOperators["!~"]   = new OperatorInfo(@"!~", 35, "op_CaseInsensitiveInEquality");
            cvOperators["or"]   = new OperatorInfo(@"or", 35, "op_OrOperator");
			cvOperators["||"]	= new OperatorInfo(@"||", 35, "op_OrOperator");
			cvOperators["and"]	= new OperatorInfo(@"and", 35, "op_AndOperator");
			cvOperators["&&"]	= new OperatorInfo(@"&&", 35, "op_AndOperator");
			cvOperators["!="]	= new OperatorInfo(@"!=", 35, "op_Inequality");
			cvOperators["<="]	= new OperatorInfo(@"<=", 35, "op_LessThanOrEqual");
			cvOperators[">="]	= new OperatorInfo(@">=", 35, "op_GreaterThanOrEqual");
            cvOperators["="]	= new OperatorInfo(@"=" , 10, "op_Assign");
            cvOperators[">>"]	= new OperatorInfo(@">>", 20, "op_ShiftRight");
            cvOperators["<<"]	= new OperatorInfo(@"<<", 20, "op_ShiftLeft");
            cvOperators["^"]	= new OperatorInfo(@"^" , 20, "op_Casing");
			cvOperators["Not" ]	= new OperatorInfo(@"Not" , 98, "op_NotOperator", false, true);
			cvOperators["<" ]	= new OperatorInfo(@"<" , 40, "op_LessThan");
			cvOperators[">" ]	= new OperatorInfo(@">" , 40, "op_GreaterThan");
			cvOperators["(" ]	= new OperatorInfo(@"(" , 99, "");
			cvOperators[")" ]	= new OperatorInfo(@")" ,  5, "");
			cvOperators["Min"]	= new OperatorInfo(@"Min",98, "op_Min", false, true);
            cvOperators["Max"] = new OperatorInfo(@"Max", 98, "op_Max", false, true);
            cvOperators["If"] = new OperatorInfo(@"If", 98, "op_If", false, true);
            cvOperators["Round"] = new OperatorInfo(@"Round", 98, "op_Round", false, true);
            cvOperators["In"] = new OperatorInfo(@"In", 98, "op_In", false, true);
            cvOperators["Replace"] = new OperatorInfo(@"Replace", 98, "op_Replace", false, true);
            cvOperators["Length"] = new OperatorInfo(@"Length", 98, "op_Length", false, true);
            cvOperators["Substring"] = new OperatorInfo(@"Substring", 98, "op_Substring", false, true);
            cvOperators["Id"] = new OperatorInfo(@"Id", 98, "op_Id", false, true);
            cvOperators["String"] = new OperatorInfo(@"String", 98, "op_ToString", false, true);
            cvOperators["Decimal"] = new OperatorInfo(@"Decimal", 98, "op_ToDecimal", false, true);
            cvOperators["Empty"] = new OperatorInfo(@"Empty", 98, "op_Empty", false, true);
			cvOperators[","]	= new OperatorInfo(@"," ,  6, ",", false, false);
		}
    }
}
