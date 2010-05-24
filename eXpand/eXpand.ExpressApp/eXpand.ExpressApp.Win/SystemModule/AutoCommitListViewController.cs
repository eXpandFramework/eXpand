using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassAutoCommitListView : IModelNode
    {
        [Category("eXpand")]
        [Description("Control if changes on editable listview will be autocommited")]
        bool AutoCommitListView { get; set; }
    }

    public interface IModelListViewAutoCommitListView : IModelNode
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassAutoCommitListView)ModelClass)", "AutoCommitListView")]
        [Description("Control if changes on editable listview will be autocommited")]
        bool AutoCommitListView { get; set; }
    }

    public class AutoCommitListViewController : ViewController<ListView>, IModelExtender
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            var winDetailViewController = Frame.GetController<WinDetailViewController>();
            if (winDetailViewController != null && ((IModelListViewAutoCommitListView)View.Model).AutoCommitListView)
                winDetailViewController.AutoCommitListView = true;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassAutoCommitListView>();
            extenders.Add<IModelListView, IModelListViewAutoCommitListView>();
        }
    }
}