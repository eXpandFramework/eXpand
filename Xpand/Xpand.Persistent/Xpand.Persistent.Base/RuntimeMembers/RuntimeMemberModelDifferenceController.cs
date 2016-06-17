using System;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.RuntimeMembers {
    public class RuntimeMemberModelDifferenceController:ObjectViewController<ObjectView,IModelDifference>{
        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
        }
    }
}
