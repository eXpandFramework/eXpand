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

        string _childListView ;

        public string ChildListView {
            get{ return _childListView ; }
            set{ SetPropertyValue(nameof(ChildListView ), ref _childListView , value); }
        }

        string _collectionMember ;

        public string CollectionMember {
            get{ return _collectionMember ; }
            set{ SetPropertyValue(nameof(CollectionMember ), ref _collectionMember , value); }
        }

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