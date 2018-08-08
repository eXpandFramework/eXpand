using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace Xpand.Persistent.Base.General.CustomFunctions{
    public class EvaluateExpressionOperator:ICustomFunctionOperator{
        public List<string> Usings=new List<string>();
        public EvaluateExpressionOperator(){
            Usings.Add("using System;");
            Usings.Add("using System.Text;");
            Usings.Add("using System.Text.RegularExpressions;");
        }

        public const string OperatorName = "EvaluateExpression";

        public static EvaluateExpressionOperator Instance{ get; } = new EvaluateExpressionOperator();

        public Type ResultType(params Type[] operands){
            return typeof(object);
        }

        public object Evaluate(params object[] operands){
            var usingString=string.Join(Environment.NewLine,Usings.ToArray());
            var scriptText = $"{usingString}" +
                             "public class CSEvaluator{" +
                             "     public object Evaluate(){" +
                             $"         return {string.Join("",operands)};" +
                             "     }" +
                             "}";

            dynamic o = CSScriptLibrary.CSScript.LoadCode(scriptText).CreateObject("*");
            return o.Evaluate();
        }

        public string Name => OperatorName;
    }
}