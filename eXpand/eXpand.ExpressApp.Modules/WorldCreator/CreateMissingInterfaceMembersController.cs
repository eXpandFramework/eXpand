using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator
{
    public class CreateMissingInterfaceMembersController : ViewController<ListView>
    {
        IPersistentClassInfo _persistentClassInfo;

        public CreateMissingInterfaceMembersController()
        {
            TargetObjectType = typeof (IInterfaceInfo);
            TargetViewType=ViewType.ListView;
            TargetViewNesting=Nesting.Nested;
        }

        public IPersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CollectionSource is PropertyCollectionSource) {
                var collectionSource = (PropertyCollectionSource) View.CollectionSource;
                collectionSource.MasterObjectChanged += collectionSource_MasterObjectChanged;
            }
            Frame.GetController<LinkUnlinkController>().LinkAction.Execute+=LinkActionOnExecute;
        }

        void collectionSource_MasterObjectChanged(object sender, EventArgs e) {
            _persistentClassInfo = (IPersistentClassInfo) ((PropertyCollectionSource)sender).MasterObject;
        }

        void LinkActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            if (e.PopupWindow.View.SelectedObjects.Count>0) {
                var worldCreatorModule =(WorldCreatorModule) Application.Modules.FindModule(typeof (WorldCreatorModule));
                _persistentClassInfo.CreateMembersFromInterfaces(worldCreatorModule.TypesInfo);
            }
        }


    }
}
