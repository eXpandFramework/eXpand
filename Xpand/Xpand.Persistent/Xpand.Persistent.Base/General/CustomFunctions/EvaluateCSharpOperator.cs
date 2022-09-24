using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace Xpand.Persistent.Base.General.CustomFunctions{
    public class EvaluateCSharpOperator:ICustomFunctionOperator{
        public List<string> Usings=new List<string>();

        public const string OperatorName = "CsEval";

        public static EvaluateCSharpOperator Instance{ get; } = new EvaluateCSharpOperator();

        public Type ResultType(params Type[] operands){
            return typeof(object);
        }

        public object Evaluate(params object[] operands){
            // var csCode = string.Join("",operands);
            // var usings = string.Join(Environment.NewLine,Usings);
            // var eval = CSharpEvaluator.Eval(csCode, usings);
            return null;
        }

        public string Name => OperatorName;
    }
}