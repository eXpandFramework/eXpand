using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.RuntimeMembers;

namespace Xpand.ExpressApp.SystemModule {

    public class ModelDifferenceObjectsRuntimeMembersController : ViewController {
        public ModelDifferenceObjectsRuntimeMembersController() {
            TargetObjectType = typeof(IXpoModelDifference);
        }


        protected override void OnActivated() {
            base.OnActivated();
            View.ObjectSpace.Committed += ObjectSpaceOnCommitted;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.ObjectSpace.Committed -= ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs args) {
            RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
        }
    }
}