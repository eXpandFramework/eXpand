using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.Security;
using IOperationPermissionProvider = Xpand.ExpressApp.Security.Permissions.IOperationPermissionProvider;

namespace Xpand.Persistent.BaseImpl.Security{
    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Appearance("HideHiddenNavigationItemsForAdministrators", AppearanceItemType = nameof(ViewItem),
        TargetItems = nameof(HiddenNavigationItems), Enabled = false, Criteria = nameof(IsAdministrative))]
    public class XpandPermissionPolicyRole : PermissionPolicyRole, ISecurityPermisssionPolicyRelated,
        IXpandRoleCustomPermissions, ISupportHiddenNavigationItems{
        private string _hiddenNavigationItems;

        public XpandPermissionPolicyRole(Session session) : base(session){
        }

        [Association("XpandPermissionPolicyRole-XpandPermissionDatas"),Aggregated]
        public XPCollection<PermissionPolicyData.PermissionPolicyData> Permissions
            => GetCollection<PermissionPolicyData.PermissionPolicyData>("Permissions");

        [Size(SizeAttribute.Unlimited)]
        public string HiddenNavigationItems{
            get { return _hiddenNavigationItems; }
            set { SetPropertyValue("HiddenNavigationItems", ref _hiddenNavigationItems, value); }
        }

        IList<IOperationPermissionProvider> IXpandRoleCustomPermissions.Permissions
            => new ListConverter<IOperationPermissionProvider, PermissionPolicyData.PermissionPolicyData>(Permissions);

        public override string ToString(){
            return Name;
        }
    }
}