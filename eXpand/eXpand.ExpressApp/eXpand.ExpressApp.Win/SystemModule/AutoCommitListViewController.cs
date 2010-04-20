using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {

    public interface IModelListViewAutoCommit : IModelNode
    {
        bool AutoCommit { get; set; }
    }

    public class AutoCommitListViewController : ViewController<ListView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            var winDetailViewController = Frame.GetController<WinDetailViewController>();
            if (winDetailViewController != null && ((IModelListViewAutoCommit)View.Model).AutoCommit)
                winDetailViewController.AutoCommitListView = true;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewAutoCommit>();
        }
    }
}