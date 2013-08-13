using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.Logic.Security;

namespace Xpand.ExpressApp.Logic {
    public class FillRulesFromPermissionsController : ViewController {
        public FillRulesFromPermissionsController() {
            TargetObjectType = typeof(IPersistentPermission);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
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
            var ruleCollector = Application.Modules.FindModule<LogicModule>().LogicRuleCollector;
            LogicRuleCollector.PermissionsReloaded = false;
            ruleCollector.CollectRules(Application);
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs args) {
            CollectRules();
        }
    }
}