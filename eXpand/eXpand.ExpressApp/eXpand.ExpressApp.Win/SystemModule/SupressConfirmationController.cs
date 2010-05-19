using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelViewSupressConfirmation : IModelNode
    {
        [Category("eXpand")]
        bool SupressConfirmation { get; set; }
    }

    public class SupressConfirmationController : BaseViewController, IModelExtender
    {
        private WinDetailViewController winDetailViewController;

        protected override void OnActivated()
        {
            base.OnActivated();
            winDetailViewController = Frame.GetController<WinDetailViewController>();
            winDetailViewController.SuppressConfirmation = ((IModelViewSupressConfirmation)View.Model).SupressConfirmation;

            if (View is DetailView && ObjectSpace.IsNewObject(View.CurrentObject))
            {
                ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                winDetailViewController.SuppressConfirmation = true;
            }
        }

        protected override void OnDeactivating()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            base.OnDeactivating();
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            winDetailViewController.SuppressConfirmation = false;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelViewSupressConfirmation>();
            extenders.Add<IModelDetailView, IModelViewSupressConfirmation>();
        }
    }
}