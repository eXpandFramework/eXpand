using System.Collections.Generic;
using Common.Win.General.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;
using System.Linq;

namespace Xpand.ExpressApp.Security.Core {
    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class XpandRole : SecuritySystemRole {
        public XpandRole(Session session)
            : base(session) {
        }

        protected override IEnumerable<IOperationPermission> GetPermissionsCore() {
            var operationPermissions = base.GetPermissionsCore().Union(Permissions.SelectMany(data => data.GetPermissions()));
            var overallCustomizationAllowedPermissions = new OverallCustomizationAllowedPermission[0];
            if (ModifyLayout) {
                overallCustomizationAllowedPermissions = new[] { new OverallCustomizationAllowedPermission() };
            }
            return operationPermissions.Union(overallCustomizationAllowedPermissions);
        }
        private bool _modifyLayout;

        public bool ModifyLayout {
            get { return _modifyLayout; }
            set { SetPropertyValue("ModifyLayout", ref _modifyLayout, value); }
        }

        public override string ToString() {
            return Name;
        }
        [Association("XpandRole-XpandPermissionDatas")]
        public XPCollection<XpandPermissionData> Permissions {
            get {
                return GetCollection<XpandPermissionData>("Permissions");
            }
        }
    }
}
