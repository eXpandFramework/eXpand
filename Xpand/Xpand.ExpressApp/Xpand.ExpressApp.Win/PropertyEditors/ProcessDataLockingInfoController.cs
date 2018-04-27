using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    public interface IModelClassAutoMergeOnConflict:IModelNode{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description(@"When not possible to merge the Refresh\Cancel dialog will be shown")]
        bool AutoMergeOnConflict{ get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassAutoMergeOnConflict),"ModelClass")]
    public interface IModelObjectViewAutoMergeOnConflict :IModelClassAutoMergeOnConflict{
        
    }
    public class ProcessDataLockingInfoController:DevExpress.ExpressApp.SystemModule.ProcessDataLockingInfoController,IModelExtender {

        protected override void ProcessDataLockingInfo(DataLockingInfo dataLockingInfo, out bool cancelAction){
            if (ObjectSpace is IDataLockingManager objectSpace && objectSpace.IsActive && dataLockingInfo.IsLocked)
                if (dataLockingInfo.CanMerge && ((IModelObjectViewAutoMergeOnConflict) View.Model).AutoMergeOnConflict){
                    var controller = Application.CreateController<ProcessDataLockingInfoDialogController>();
                    controller.ShowMergeDialog(Application);
                    cancelAction = true;
                    return;
                }
            base.ProcessDataLockingInfo(dataLockingInfo, out cancelAction);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassAutoMergeOnConflict>();
            extenders.Add<IModelObjectView,IModelObjectViewAutoMergeOnConflict>();
        }
    }
}
