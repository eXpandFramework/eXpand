using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using Xpand.Persistent.Base.MasterDetail;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.MasterDetail {
    [System.ComponentModel.DisplayName("MasterDetail")]
    public class MasterDetailOperationPermissionPolicyData : LogicRuleOperationPermissionPolicyData, IContextMasterDetailRule, IMasterDetailOperationPermissionData {
        public MasterDetailOperationPermissionPolicyData(Session session)
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

        protected override Type GetPermissionType(){
            return typeof(IContextMasterDetailRule);
        }
    }
}