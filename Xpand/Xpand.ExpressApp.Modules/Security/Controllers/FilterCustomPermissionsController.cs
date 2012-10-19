using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;
using System.Linq;

namespace Xpand.ExpressApp.Security.Controllers {
    public class FilterCustomPermissionsController : ViewController<ListView> {
        public FilterCustomPermissionsController() {
            TargetObjectType = typeof(XpandPermissionData);
            TargetViewNesting = Nesting.Nested;
            TargetViewType = ViewType.ListView;
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes += OnCollectDescendantTypes;
            Frame.GetController<PermissionsController>().CollectDescendantPermissionTypes += OnCollectDescendantPermissionTypes;
        }

        void OnCollectDescendantPermissionTypes(object sender, CollectTypesEventArgs e) {

        }

        void OnCollectDescendantTypes(object sender, CollectTypesEventArgs e) {
            var objectTypeInfo = ((ViewController)sender).View.ObjectTypeInfo.Type;
            if (objectTypeInfo == typeof(XpandPermissionData)) {
                var collection = e.Types.ToList();
                for (int i = collection.Count - 1; i > -1; i--) {
                    var type = collection[i];
                    if (!typeof(XpandPermissionData).IsAssignableFrom(type))
                        e.Types.Remove(type);
                }
            }
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes -= OnCollectDescendantTypes;
            Frame.GetController<PermissionsController>().CollectDescendantPermissionTypes -= OnCollectDescendantPermissionTypes;
        }

        protected override void OnActivated() {
            base.OnActivated();
            UpdateCollectionCriteria(new List<string>());
        }

        public void UpdateCollectionCriteria(List<string> list) {
            string propertyName = PersistentBase.Fields.ObjectType.TypeName.PropertyName;
            var inOperator = new InOperator(propertyName, GetPermissionTypeNames());
            View.CollectionSource.Criteria["CustomPermisssions"] = inOperator;
        }

        protected virtual ICollection GetPermissionTypeNames() {
            var typeDescendants = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(XpandPermissionData)));
            return typeDescendants.Select(info => info.FullName).ToList();
        }
    }
}
