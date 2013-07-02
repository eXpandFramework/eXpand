using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Core {
    public class IsAllowedToRoleOperator : ICustomFunctionOperator {
        public const string OperatorName = "IsAllowedToRole";
        #region ICustomFunctionOperator Members
        public object Evaluate(params object[] operands) {
            if (!(operands != null && operands.Length == 1 && operands[0] is string)) {
                throw new ArgumentException("IsAllowedToRole operator should have one paraneter - string roleName.");
            }
            var roleName = (string)operands[0];
            bool result = false;
            var userWithRoles = SecuritySystem.CurrentUser as IUserWithRoles;
            if (userWithRoles != null && userWithRoles.Roles.Any(role => role.Name == roleName)) {
                result = true;
            }
            return result;
        }

        public string Name {
            get { return OperatorName; }
        }

        public Type ResultType(params Type[] operands) {
            return typeof(bool);
        }
        #endregion
    }
}
