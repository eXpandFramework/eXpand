using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.Security.Permissions;
using System.Linq;

namespace eXpand.ExpressApp.Logic{
    public class FillRulesFromPermissionsController : ViewController
    {
        public FillRulesFromPermissionsController() {
            TargetObjectType = typeof(IPersistentPermission);
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.ObjectDeleted+=ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }
        private void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args) {
            var permission = (args.Object) as IPersistentPermission;
            if (permission != null && permission.Permission is LogicRulePermission) {
                CollectRules();
            }
        }

        void CollectRules() {
            var ruleCollectors = Application.Modules.OfType<IRuleCollector>();
            foreach (var ruleCollector in ruleCollectors) {
                ruleCollector.CollectRules(Application);
            }
        }

        private void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs args){
            CollectRules();
        }
    }
}