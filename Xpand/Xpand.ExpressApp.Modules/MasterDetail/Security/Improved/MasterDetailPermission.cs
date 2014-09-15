using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.MasterDetail.Security.Improved {
    public class MasterDetailPermission : LogicRulePermission, IContextMasterDetailRule {
        public const string OperationName = "MasterDetail";

        public MasterDetailPermission(MasterDetailOperationPermissionData logicRule)
            : base(OperationName, logicRule) {
            var modelApplication = ApplicationHelper.Instance.Application.Model;
            ChildListView = (IModelListView) modelApplication.Views[logicRule.ChildListView];
            var modelClass = modelApplication.BOModel.GetClass(((ILogicRule) logicRule).TypeInfo.Type);
            if (logicRule.CollectionMember != null)
                CollectionMember = modelClass.FindMember(logicRule.CollectionMember);
        }
        
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        public IModelListView ChildListView { get; set; }
        public IModelMember CollectionMember { get; set; }
    }
}
