using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassAutoCommitListView : IModelNode
    {
        [Category("eXpand")]
        [Description("Control if changes on editable listview will be autocommited")]
        bool AutoCommitListView { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassAutoCommitListView), "ModelClass")]
    public interface IModelListViewAutoCommitListView : IModelClassAutoCommitListView
    {
    }

    public class AutoCommitListViewController : ViewController<XpandListView>, IModelExtender
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            var winDetailViewController = Frame.GetController<WinDetailViewController>();
            if (winDetailViewController != null && ((IModelListViewAutoCommitListView)View.Model).AutoCommitListView) {
                winDetailViewController.AutoCommitListView = true;
                View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;
            }
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            var winDetailViewController = Frame.GetController<WinDetailViewController>();
            if (winDetailViewController != null && ((IModelListViewAutoCommitListView)View.Model).AutoCommitListView)
            {
                winDetailViewController.AutoCommitListView = true;
                View.QueryCanChangeCurrentObject -= ViewOnQueryCanChangeCurrentObject;
            }
        }
        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            if (Frame.GetController<WinDetailViewController>().SuppressConfirmation)
                ObjectSpace.CommitChanges();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassAutoCommitListView>();
            extenders.Add<IModelListView, IModelListViewAutoCommitListView>();
        }
    }
}