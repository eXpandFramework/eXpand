using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassSupressConfirmation : IModelNode
    {
        [Category("eXpand")]
        [Description("Suppress confirmation message when an object has been change")]
        bool SupressConfirmation { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassSupressConfirmation), "ModelClass")]
    public interface IModelViewSupressConfirmation : IModelClassSupressConfirmation
    {
        
    }

    public class SupressConfirmationController : ViewController<ObjectView>, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassSupressConfirmation>();
            extenders.Add<IModelObjectView, IModelViewSupressConfirmation>();
            
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

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            base.OnDeactivated();
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            winDetailViewController.SuppressConfirmation = false;
        }
    }
}