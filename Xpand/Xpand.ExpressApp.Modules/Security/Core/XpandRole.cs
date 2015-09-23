using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Security.Permissions;
using System.Linq;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Core {

    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Appearance("HideHiddenNavigationItemsForAdministrators", AppearanceItemType = "LayoutItem", TargetItems = "HiddenNavigationItems", Visibility = ViewItemVisibility.Hide, Criteria = "IsAdministrative")]
    public class XpandRole : SecuritySystemRole,ISecurityRelated {
        public XpandRole(Session session)
            : base(session) {
        }

        private string _hiddenNavigationItems;
        public string HiddenNavigationItems {
            get { return _hiddenNavigationItems; }
            set { SetPropertyValue("HiddenNavigationItems", ref _hiddenNavigationItems, value); }
        }

        protected override IEnumerable<IOperationPermission> GetPermissionsCore() {
            var operationPermissions = base.GetPermissionsCore().Union(Permissions.SelectMany(data => data.GetPermissions()));
            var permissions = operationPermissions.Union(PermissionProviderStorage.Instance.SelectMany(info => info.GetPermissions(this)));
            var allpermissions = OperationPermissionCollectionMembers().Aggregate(permissions, (current, xpMemberInfo) => current.Union(this.ObjectOperationPermissions(xpMemberInfo)));
            if (!String.IsNullOrEmpty(HiddenNavigationItems)) {
                allpermissions=allpermissions.Concat(HiddenNavigationItems.Split(';', ',').Select(s => new NavigationItemPermission(s.Trim())));
            }
            return allpermissions;
        }

        IEnumerable<XPMemberInfo> OperationPermissionCollectionMembers() {
            return ClassInfo.OwnMembers.Where(info => info.IsAssociationList && info.CollectionElementType.HasAttribute(typeof(SecurityOperationsAttribute)));
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
