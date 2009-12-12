using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Controllers.ListView {
    public class InterfaceInfoNestedController : MasterObjectViewController<IPersistentClassInfo> {
        public InterfaceInfoNestedController() {
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