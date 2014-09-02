using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelClassSupressConfirmation : IModelNode{
        [Category("eXpand")]
        [Description("Suppress confirmation message when an object has been change")]
        bool SupressConfirmation { get; set; }
    }

    [ModelInterfaceImplementor(typeof (IModelClassSupressConfirmation), "ModelClass")]
    public interface IModelViewSupressConfirmation : IModelClassSupressConfirmation{
    }

    public class SupressConfirmationController : ViewController<ObjectView>, IModelExtender{
        private WinModificationsController _winDetailViewController;

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass, IModelClassSupressConfirmation>();
            extenders.Add<IModelObjectView, IModelViewSupressConfirmation>();
        }

        protected override void OnActivated(){
            base.OnActivated();
            _winDetailViewController = Frame.GetController<WinModificationsController>();
            if (((IModelViewSupressConfirmation) View.Model).SupressConfirmation){
                _winDetailViewController.ModificationsHandlingMode = ModificationsHandlingMode.AutoCommit;
                if (View is DetailView && ObjectSpace.IsNewObject(View.CurrentObject)){
                    ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                    _winDetailViewController.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
                }
            }
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e){
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            _winDetailViewController.ModificationsHandlingMode = ModificationsHandlingMode.Confirmation;
        }
    }
}