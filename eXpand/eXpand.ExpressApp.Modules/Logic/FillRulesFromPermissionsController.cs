using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.Logic.Security;

namespace eXpand.ExpressApp.Logic {
    public class FillRulesFromPermissionsController : ViewController {
        public FillRulesFromPermissionsController() {
            TargetObjectType = typeof (IPersistentPermission);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
        }

        protected override void OnDeactivating() {
            base.OnDeactivating();
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }

        void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args) {
            var permission = (args.Object) as IPersistentPermission;
            if (permission != null && permission.Permission is LogicRulePermission) {
                CollectRules();
            }
        }

        void CollectRules() {
            IEnumerable<IRuleCollector> ruleCollectors = Application.Modules.OfType<IRuleCollector>();
            foreach (IRuleCollector ruleCollector in ruleCollectors) {
                ruleCollector.CollectRules(Application);
            }
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs args) {
            CollectRules();
        }
    }
}