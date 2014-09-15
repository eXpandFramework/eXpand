using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.ExpressApp.MasterDetail.Logic;

namespace Xpand.ExpressApp.MasterDetail.Security.Improved {
    [System.ComponentModel.DisplayName("MasterDetail")]
    public class MasterDetailOperationPermissionData : LogicRuleOperationPermissionData, IContextMasterDetailRule {
        public MasterDetailOperationPermissionData(Session session)
            : base(session) {
        }

        public string ChildListView { get; set; }

        public string CollectionMember { get; set; }

        IModelListView IMasterDetailRule.ChildListView {
            get { return (IModelListView) CaptionHelper.ApplicationModel.Views[ChildListView]; }
            set { throw new NotImplementedException(); }
        }

        IModelMember IMasterDetailRule.CollectionMember {
            get{
                return string.IsNullOrEmpty(CollectionMember) ? null : CaptionHelper.ApplicationModel.BOModel.GetClass(ObjectTypeData).FindMember(CollectionMember);
            }
            set { throw new NotImplementedException(); }
        }
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new MasterDetailPermission(this) };
        }
    }
}