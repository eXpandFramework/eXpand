using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public class CreateMembersFromInterfacesController : MasterObjectViewController<IInterfaceInfo,IPersistentClassInfo> {
        IPersistentClassInfo _masterObject;
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<LinkUnlinkController>().LinkAction.ExecuteCompleted+=LinkActionOnExecuteCompleted;
        }

        void LinkActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _masterObject.CreateMembersFromInterfaces();
        }

        protected override void UpdateMasterObject(IPersistentClassInfo masterObject) {
            _masterObject = masterObject;
        }
    }
}