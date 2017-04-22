using System.ComponentModel;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Xpand.Persistent.Base.General{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public class WebGridDetailFrameSyncController : DevExpress.ExpressApp.Web.SystemModule.WebGridDetailFrameSyncController {
        private DetailViewLinkController _linkController;

        private void LinkController_ObjectReloaded(object sender, ObjectReloadedEventArgs e){
            var editor = e.ListView.Editor as ASPxGridListEditor;
            editor?.DetailFramesManager?.RefreshObjectViewFromCache(e.Obj);
        }

        protected override void OnActivated(){
            _linkController = Frame.GetController<DetailViewLinkController>();
            if (_linkController != null) _linkController.ObjectReloaded += LinkController_ObjectReloaded;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_linkController != null){
                _linkController.ObjectReloaded -= LinkController_ObjectReloaded;
                _linkController = null;
            }
        }
    }
}