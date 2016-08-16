using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.MasterDetail;

namespace Xpand.ExpressApp.MasterDetail.Security.Improved {
    public class MasterDetailPermission : LogicRulePermission, IContextMasterDetailRule {
        public const string OperationName = "MasterDetail";

        public MasterDetailPermission(IContextMasterDetailRule logicRule)
            : base(OperationName, logicRule) {
            var modelApplication = ApplicationHelper.Instance.Application.Model;
            ChildListView = (IModelListView) modelApplication.Views[(string) logicRule.GetPropertyValue(nameof(IMasterDetailOperationPermissionData.ChildListView))];
            var modelClass = modelApplication.BOModel.GetClass(logicRule.TypeInfo.Type);
            var collectionMember = (string)logicRule.GetPropertyValue(nameof(IMasterDetailOperationPermissionData.CollectionMember));
            if (collectionMember != null)
                CollectionMember = modelClass.FindMember(collectionMember);
        }
        
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        public IModelListView ChildListView { get; set; }
        public IModelMember CollectionMember { get; set; }
    }
}
