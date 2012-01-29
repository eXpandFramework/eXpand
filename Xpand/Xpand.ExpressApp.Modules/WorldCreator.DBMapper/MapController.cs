using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using System.Linq;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public class MapController : ViewController<DetailView> {
        public MapController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);

        }
        protected override void OnActivated() {
            base.OnActivated();
            var assemblyToolsController = Frame.GetController<AssemblyToolsController>();
            if (assemblyToolsController.SingleChoiceAction.Items.Find("MapDB") == null) {
                assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Map Datastore", "MapDB"));
                assemblyToolsController.SingleChoiceAction.Execute += AssemblyToolsControllerOnToolExecuted;
            }
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if ((string)singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data == "MapDB") {
                var space = Application.CreateObjectSpace();
                var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;

                showViewParameters.CreatedView = Application.CreateDetailView(space, space.CreateObject(typeof(LogonObject)));
                var dialogController = new DialogController { SaveOnAccept = true };
                dialogController.AcceptAction.Execute += AcceptActionOnExecute;
                showViewParameters.Controllers.Add(dialogController);
            }
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var persistentAssemblyInfo = (IPersistentAssemblyInfo)View.CurrentObject;
            ObjectSpace.CommitChanges();
            var objectSpace = new ObjectSpace(new UnitOfWork(((ObjectSpace)ObjectSpace).Session.DataLayer), XafTypesInfo.Instance);
            CreateMappedAssemblyInfo(objectSpace, persistentAssemblyInfo, (LogonObject)simpleActionExecuteEventArgs.CurrentObject);
            ObjectSpace.Refresh();
            ObjectSpace.SetModified(View.CurrentObject);
        }



        void CreateMappedAssemblyInfo(ObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, LogonObject logonObject) {

            new AssemblyGenerator(logonObject, objectSpace.GetObject(persistentAssemblyInfo)).Create();
            objectSpace.CommitChanges();
        }
    }
}
