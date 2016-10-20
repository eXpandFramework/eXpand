using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule{
    public interface IModelOptionsOptimisticConcurrencyControl : IModelNode {

        [Category(AttributeCategoryNameProvider.Xpand)]
        bool OptimisticConcurrencyControl { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelOptionsOptimisticConcurrencyControl), "Application.Options")]
    public interface IModelObjectViewOptimisticConcurrencyControl : IModelOptionsOptimisticConcurrencyControl {
    }

    public class WebProcessDataLockingInfoController : ObjectViewController,IModelExtender{
        private ModificationsController _modificationsViewController;

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewOptimisticConcurrencyControl) View.Model).OptimisticConcurrencyControl){
                _modificationsViewController = Frame.GetController<ModificationsController>();
                if (_modificationsViewController != null){
                    _modificationsViewController.SaveAction.Executing += action_Executing;
                    _modificationsViewController.SaveAndNewAction.Executing += action_Executing;
                    _modificationsViewController.SaveAndCloseAction.Executing += action_Executing;
                }
            }
        }

        protected override void OnDeactivated(){
            if (_modificationsViewController != null){
                _modificationsViewController.SaveAction.Executing -= action_Executing;
                _modificationsViewController.SaveAndNewAction.Executing -= action_Executing;
                _modificationsViewController.SaveAndCloseAction.Executing -= action_Executing;
                _modificationsViewController = null;
            }
            base.OnDeactivated();
        }

        private void action_Executing(object sender, CancelEventArgs e){
            if (!e.Cancel){
                var manager = ObjectSpace as IDataLockingManager;
                if (manager != null && manager.IsActive){
                    var dataLockingInfo = manager.GetDataLockingInfo();
                    if (dataLockingInfo.IsLocked && dataLockingInfo.CanMerge){
                        manager.MergeData(dataLockingInfo);
                        throw new UserFriendlyException(
                            "The object you are trying to save was changed by another user. New values are loaded. Click Save to confirm your changes.");
                    }
                }
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions, IModelOptionsOptimisticConcurrencyControl>();
            extenders.Add<IModelObjectView, IModelObjectViewOptimisticConcurrencyControl>();
        }
    }
}