using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace WorkflowDemo.Win {
    public class WorkflowServerAuthentication : AuthenticationBase {
        private CriteriaOperator findUserCriteria;
        public WorkflowServerAuthentication(CriteriaOperator findUserCriteria) {
            this.findUserCriteria = findUserCriteria;
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            object user = objectSpace.FindObject(UserType, findUserCriteria);
            if(user == null) {
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
