using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
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
    public class ProcessDataLockingInfoController:DevExpress.ExpressApp.Win.SystemModule.ProcessDataLockingInfoController,IModelExtender {
        protected override ProcessDataLockingInfoDialogResult GetUserChoice(DataLockingInfo dataLockingInfo){
            var controller = Application.CreateController<ProcessDataLockingInfoDialogController>();
            return dataLockingInfo.CanMerge? (!((IModelObjectViewAutoMergeOnConflict) View).AutoMergeOnConflict
                    ? controller.ShowMergeDialog(Application): ProcessDataLockingInfoDialogResult.Merge): controller.ShowRefreshDialog(Application);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassAutoMergeOnConflict>();
            extenders.Add<IModelObjectView,IModelObjectViewAutoMergeOnConflict>();
        }
    }
}
