using DevExpress.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class ViewEditModeController : ExpressApp.SystemModule.ViewEditModeController {
        protected override void UpdateViewEditModeState(DevExpress.ExpressApp.Editors.ViewEditMode viewEditMode){
            var editAction = Frame.GetController<WebModificationsController>().EditAction;
            if (editAction.Active&&editAction.Enabled)
                editAction.DoExecute();
        }
    }
}
