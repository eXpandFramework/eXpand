using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Security.Core{
    public class CurrentUserNameOperator : ICustomFunctionOperator{
        public const string OperatorName = "CurrentUserName";
        private static readonly CurrentUserNameOperator _instance = new CurrentUserNameOperator();

        static CurrentUserNameOperator(){
        }

        public string Name{
            get { return "CurrentUserName"; }
        }

        public static void Register(){
            CustomFunctionOperatorHelper.Register(_instance);
        }

        public object Evaluate(params object[] operands){
            return SecuritySystem.CurrentUserName;
        }

        public Type ResultType(params Type[] operands){
            return typeof (string);
        }
    }
}