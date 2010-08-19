using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRule : ConditionalLogicRule, IMasterDetailRule
    {
        public MasterDetailRule(IMasterDetailRule masterDetailRule)
            : base(masterDetailRule)
        {
            ChildListView=masterDetailRule.ChildListView;
            CollectionMember=masterDetailRule.CollectionMember;
        }

        public IModelListView ChildListView { get; set; }

        public IModelMember CollectionMember { get; set; }
    }
}