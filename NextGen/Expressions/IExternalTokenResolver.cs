using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Generator.Expressions
{
    public interface IExternalTokenResolver
    {
        Var TokenEvaluator(String token);
        Var TokenEvaluator(StringVar token);
        FunctionInfo FunctionFinder(String token);
        Var FunctionEvaluator(String name, ArrayList parameters);
    }
}
