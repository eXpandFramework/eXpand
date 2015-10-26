using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.Security;
using IOperationPermissionProvider = Xpand.ExpressApp.Security.Permissions.IOperationPermissionProvider;

namespace Xpand.ExpressApp.Security.Core {
    public interface IXpandRoleCustomPermissions :  ISecurityRole {
        IList<IOperationPermissionProvider> Permissions { get; }
    }

    public interface ISupportHiddenNavigationItems{
        string HiddenNavigationItems { get; set; }
    }

    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Appearance("HideHiddenNavigationItemsForAdministrators", AppearanceItemType = "LayoutItem", TargetItems = "HiddenNavigationItems", Visibility = ViewItemVisibility.Hide, Criteria = "IsAdministrative")]
    public class XpandRole : SecuritySystemRole,ISecurityRelated, IXpandRoleCustomPermissions, ISupportHiddenNavigationItems{
        public XpandRole(Session session)
            : base(session) {
        }

        private string _hiddenNavigationItems;

        [Size(SizeAttribute.Unlimited)]
        public string HiddenNavigationItems {
            get { return _hiddenNavigationItems; }
            set { SetPropertyValue("HiddenNavigationItems", ref _hiddenNavigationItems, value); }
        }

        public override string ToString() {
            return Name;
        }

        [Association("XpandRole-XpandPermissionDatas")]
        public XPCollection<XpandPermissionData> Permissions{
            get { return GetCollection<XpandPermissionData>("Permissions"); }
        }

        IList<IOperationPermissionProvider> IXpandRoleCustomPermissions.Permissions{
            get { return new ListConverter<IOperationPermissionProvider,XpandPermissionData>(Permissions); }
        }

        protected override IEnumerable<IOperationPermission> GetPermissionsCore(){
            var customPermissions = this.GetCustomPermissions(base.GetPermissionsCore());
            return customPermissions.Concat(this.GetHiddenNavigationItemPermissions());
        }
    }
}
