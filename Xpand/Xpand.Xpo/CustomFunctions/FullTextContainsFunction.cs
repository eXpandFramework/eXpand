using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.CustomFunctions{
    public static class CustomFunctionOperatorExtensions {
        public static void Register(this ICustomFunctionOperator customFunctionOperator) {
            ICustomFunctionOperator registeredItem = CriteriaOperator.GetCustomFunction(customFunctionOperator.Name);
            if (registeredItem != null && registeredItem != customFunctionOperator) {
                throw new InvalidOperationException();
            }
            if (registeredItem == null) {
                CriteriaOperator.RegisterCustomFunction(customFunctionOperator);
            }
        }
    }

    public class FullTextContainsFunction : ICustomFunctionOperatorFormattable {
        public const string FunctionName = "HasText";
        #region ICustomFunctionOperator Members
        public object Evaluate(params object[] operands){
            return null;
        }
        public string Name {
            get { return FunctionName; }
        }
        public Type ResultType(params Type[] operands) {
            return typeof(bool);
        }
        #endregion
        #region ICustomFunctionOperatorFormattable Members
        public string Format(Type providerType, params string[] operands) {
            if (providerType == typeof(MSSqlConnectionProvider))
                return string.Format("contains({0}, {1})", operands[0], operands[1]);
            throw new NotSupportedException(string.Concat("This provider is not supported: ",
                providerType.Name));
        }
        #endregion
    }
}