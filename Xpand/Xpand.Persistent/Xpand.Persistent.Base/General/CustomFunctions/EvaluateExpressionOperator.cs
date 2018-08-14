using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils.CodeGeneration;
using Fasterflect;
using Microsoft.CSharp;

namespace Xpand.Persistent.Base.General.CustomFunctions{
    class ExpressionEvaluator{
        public static List<string> Usings = new List<string>{
            "System",
            "System.Xml",
            "System.Data",
            "System.Data.SqlClient",
            "System.Drawing",
            "System.Windows.Forms",
            "System.Text",
            "System.Text.RegularExpressions"
        };
        public static List<string> References = new List<string>{
            "System.dll",
            "System.Xml.dll",
            "System.Data.dll",
            "System.Drawing.dll",
            "System.Windows.Forms.dll"
        };
        private static readonly object Locker=new object();
        static readonly Dictionary<int, Assembly> EvaluatorAssemblies=new Dictionary<int, Assembly>();
        public static object Eval(string csCode,params string[] usings){
            lock (Locker){
                var hashCode = csCode.GetHashCode();
                if (EvaluatorAssemblies.ContainsKey(hashCode))
                    return EvalCode(EvaluatorAssemblies[hashCode]);
                var compiledAssembly = GetCompiledAssembly(csCode, usings);
                EvaluatorAssemblies.Add(hashCode,compiledAssembly);
                return EvalCode(compiledAssembly);
            }
        }

        private static Assembly GetCompiledAssembly(string csCode, string[] usings,string[] referencedAssemblies=null){
            var codeProvider = new CSharpCodeProvider();
            var compilerParameters = new CompilerParameters{
                CompilerOptions = "/t:library",
                GenerateInMemory = true
            };
            if (referencedAssemblies != null)
                References.AddRange(referencedAssemblies);
            foreach (var referencedAssembly in References.Distinct()){
                compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
            }

            var usingsCodeList = Usings.Concat(usings).Distinct().Where(s => !string.IsNullOrEmpty(s)).Select(s => $"using {s};");
            var usingsCode = string.Join(Environment.NewLine,usingsCodeList);
            string source = $"{usingsCode}" +
                            "public class CsCodeEvaluator{" +
                            "     public object EvalCode(){" +
                            $"        return {csCode};" +
                            "    }" +
                            "}";
            var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters,source);
            if (compilerResults.Errors.Count > 0){
                throw new CompilerErrorException(compilerResults, source,string.Join(Environment.NewLine, compilerResults.Errors));
            }

            return compilerResults.CompiledAssembly;
        }

        private static object EvalCode(Assembly assembly){
            object o = assembly.CreateInstance("CsCodeEvaluator") ?? throw new NullReferenceException("CsCodeEvaluator");
            return o.CallMethod("EvalCode");
            
        }

    }

    public class EvaluateExpressionOperator:ICustomFunctionOperator{
        public List<string> Usings=new List<string>();

        public const string OperatorName = "EvaluateExpression";

        public static EvaluateExpressionOperator Instance{ get; } = new EvaluateExpressionOperator();

        public Type ResultType(params Type[] operands){
            return typeof(object);
        }

        public object Evaluate(params object[] operands){
            var csCode = string.Join("",operands);
            var usings = string.Join(Environment.NewLine,Usings);
            var eval = ExpressionEvaluator.Eval(csCode, usings);
            return eval;
        }

        public string Name => OperatorName;
    }
}