using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRule : LogicRule, IMasterDetailRule {
        public MasterDetailRule(IMasterDetailRule masterDetailRule)
            : base(masterDetailRule) {
            ChildListView = masterDetailRule.ChildListView;
            CollectionMember = masterDetailRule.CollectionMember;
        }

        public IModelListView ChildListView { get; set; }

        public IModelMember CollectionMember { get; set; }
        public bool SynchronizeActions { get; set; }
    }
}