/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;

namespace Generator.Expressions
{
	/// <summary>
	/// This expressionevaluator evaluates expressions.
	/// Hehe, dat is logisch toch?
	/// </summary>
	public class ExpressionEvaluator : ExpressionProcessorV2
	{
        public ExpressionEvaluator(IExternalTokenResolver tokenresolver)
            : base(tokenresolver)
        {
        }
	}
}
