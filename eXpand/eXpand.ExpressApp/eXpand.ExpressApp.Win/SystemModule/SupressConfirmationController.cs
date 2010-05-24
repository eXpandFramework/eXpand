using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassSupressConfirmation : IModelNode
    {
        [Category("eXpand")]
        [Description("Suppress confirmation message when an object has been change")]
        bool SupressConfirmation { get; set; }
    }
    public interface IModelViewSupressConfirmation : IModelNode
    {
        [Category("eXpand")]
        [Description("Suppress confirmation message when an object has been change")]
        [ModelValueCalculator("((IModelClassSupressConfirmation)ModelClass)", "SupressConfirmation")]
        bool SupressConfirmation { get; set; }
    }

    public class SupressConfirmationController : ViewController, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassSupressConfirmation>();
            extenders.Add<IModelView, IModelViewSupressConfirmation>();
            
        }

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
    }
}