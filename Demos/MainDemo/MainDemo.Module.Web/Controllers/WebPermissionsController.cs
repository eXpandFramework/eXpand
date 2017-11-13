using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;

namespace MainDemo.Module.Web.Controllers {
    public partial class WebPermissionsController : ViewController {
        public WebPermissionsController() {
            TargetViewType = ViewType.Any;
            TargetViewNesting = Nesting.Any;
            TargetObjectType = typeof(IPersistentPermission);
        }
        protected override void OnActivated() {
            base.OnActivated();

            PermissionsController permissionsController = Frame.GetController<PermissionsController>();
            permissionsController.CollectDescendantPermissionTypes += permissionsController_CollectCreatablePermissionTypes;
        }

        void permissionsController_CollectCreatablePermissionTypes(object sender, CollectTypesEventArgs e) {
            List<Type> typesToRemove = new List<Type>();

            foreach(Type creatableType in e.Types) {
                if(creatableType.IsAssignableFrom(typeof(EditModelPermission))) {
                    typesToRemove.Add(creatableType);
                }
            }

            foreach(Type type in typesToRemove) {
                e.Types.Remove(type);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();

            PermissionsController permissionsController = Frame.GetController<PermissionsController>();
            permissionsController.CollectDescendantPermissionTypes -= permissionsController_CollectCreatablePermissionTypes;
        }
    }
}
