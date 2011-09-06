using System;
using DevExpress.Data.Filtering;

namespace Xpand.ExpressApp.StateMachine {
    public class StateRestrictedOperator : ICustomFunctionOperator {
        public const string OperatorName = "StateRestricted";

        public object Evaluate(params object[] operands) {
            return false;
        }
        public string Name {
            get { return OperatorName; }
        }
        public Type ResultType(params Type[] operands) {
            return typeof(bool);
        }
    }
}