using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelClassModelSaving : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool HandleModelSaving { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassModelSaving), "ModelClass")]
    public interface IModelViewModelSaving : IModelClassModelSaving {
    }

    public class ModelViewSavingController:ViewController,IModelExtender{
        public ModelViewSavingController(){
            
        }

        protected override void OnActivated(){
            base.OnActivated();
            View.CustomModelSaving+=ViewOnCustomModelSaving;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.CustomModelSaving-=ViewOnCustomModelSaving;
        }

        private void ViewOnCustomModelSaving(object sender, HandledEventArgs handledEventArgs){
            handledEventArgs.Handled = ((IModelViewModelSaving) View.Model).HandleModelSaving;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassModelSaving>();
            extenders.Add<IModelDashboardView, IModelClassModelSaving>();
            extenders.Add<IModelObjectView, IModelViewModelSaving>();
        }
    }
}