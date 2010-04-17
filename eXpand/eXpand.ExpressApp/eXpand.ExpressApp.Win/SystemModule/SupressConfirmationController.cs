using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelViewSupressConfirmation : IModelNode
    {
        bool SupressConfirmation { get; set; }
    }

    public class SupressConfirmationController : BaseViewController
    {
        private WinDetailViewController winDetailViewController;

        public SupressConfirmationController(){}

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

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelView, IModelViewSupressConfirmation>();
        }
    }
}