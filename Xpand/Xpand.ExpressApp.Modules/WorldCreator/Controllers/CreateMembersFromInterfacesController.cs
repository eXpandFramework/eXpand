using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public class CreateMembersFromInterfacesController : MasterObjectViewController<IPersistentClassInfo> {
        public CreateMembersFromInterfacesController() {
            TargetObjectType = typeof (IInterfaceInfo);
        }


        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<LinkUnlinkController>().LinkAction.Execute += LinkActionOnExecute;
        }


        void LinkActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            if (e.PopupWindow.View.SelectedObjects.Count > 0) {
                MasterObject.CreateMembersFromInterfaces();
            }
        }
    }
}