using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Xpand.ExpressApp.Workflow {
    public class WorkflowServerAuthentication : AuthenticationBase {
        private readonly CriteriaOperator findUserCriteria;
        public WorkflowServerAuthentication(CriteriaOperator findUserCriteria) {
            this.findUserCriteria = findUserCriteria;
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            object user = objectSpace.FindObject(UserType, findUserCriteria);
            if (user == null) {
                throw new AuthenticationException(findUserCriteria.ToString());
            }
            return user;
        }
        public override bool IsLogoffEnabled {
            get { return false; }
        }
        public override Type UserType { get; set; }
    }
}