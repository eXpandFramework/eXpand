using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.ConditionalDetailViews.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ConditionalDetailViews.Logic {
    public class ConditionalDetailViewRuleController : ConditionalLogicRuleViewController<IConditionalDetailViewRule>
    {
        IConditionalDetailViewRule _rule;

        public ConditionalDetailViewRuleController() {
            TargetViewType=ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (IsReady&&View is XpandListView)
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
        }

        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            if (_rule != null)
            {
                var objectSpace = Application.CreateObjectSpace();
                e.Handled = true;
                e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, _rule.DetailView, true, objectSpace.GetObject(e.InnerArgs.CurrentObject));
                _rule = null;
            }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (IsReady && View is XpandListView)
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
        }

        public override void ExecuteRule(LogicRuleInfo<IConditionalDetailViewRule> info, ExecutionContext executionContext) {
            if (info.Active) {
                _rule = info.Rule;
            }
        }

        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup) {
            return ((IModelApplicationConditionalDetailView)Application.Model).ConditionalDetailView.GroupContexts;
        }
    }
}