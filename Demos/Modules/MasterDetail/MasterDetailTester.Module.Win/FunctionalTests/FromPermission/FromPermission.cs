using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.MasterDetail.Security.Improved;
using Xpand.Persistent.BaseImpl.MasterDetail;

namespace MasterDetailTester.Module.Win.FunctionalTests.FromPermission {
    public class FromPermissionController:ObjectViewController<DetailView,MasterDetailOperationPermissionPolicyData>{
        public FromPermissionController(){
            var simpleAction = new SimpleAction(this, "From Permission Fields",PredefinedCategory.View);
            simpleAction.Execute+=SIMPLEActionOnExecute;
        }

        private void SIMPLEActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var policyData = ((MasterDetailOperationPermissionPolicyData) View.CurrentObject);
            policyData.CollectionMember = "Contributors";
            policyData.ChildListView = "Contributor_ListView";
        }
    }
}
