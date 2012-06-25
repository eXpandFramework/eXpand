using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using System.Linq;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public class MapController : ViewController<DetailView> {
        readonly IPersistentAssemblyInfo _assemblyInfo;

        public MapController()
            : this(false, null) {
        }

        public MapController(bool active, IPersistentAssemblyInfo assemblyInfo) {
            _assemblyInfo = assemblyInfo;
            Active[""] = active;
            TargetObjectType = typeof(LogonObject);
        }
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            var objectSpace = new XPObjectSpace(XafTypesInfo.Instance, XpandModuleBase.XpoTypeInfoSource, () => new UnitOfWork(((XPObjectSpace)ObjectSpace).Session.DataLayer));
            Frame nestedFrame = View.GetItems<ListPropertyEditor>().Single().Frame;
            var listEditor = ((ListView)nestedFrame.View).Editor;
            string[] selectedTables = listEditor.GetSelectedObjects().OfType<DataTable>().Select(table => table.Name).ToArray();
            CreateMappedAssemblyInfo(objectSpace, _assemblyInfo, (LogonObject)View.CurrentObject, selectedTables);
        }
        void CreateMappedAssemblyInfo(XPObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, LogonObject logonObject, string[] selectedTables) {
            var assemblyInfo = objectSpace.GetObject(persistentAssemblyInfo);
            new AssemblyGenerator(logonObject, assemblyInfo, selectedTables).Create();
            objectSpace.CommitChanges();
        }

    }
    public class MapperLogonController : ViewController<DetailView> {
        public MapperLogonController() {
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
                ObjectSpace.CommitChanges();
                var space = Application.CreateObjectSpace();
                var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                showViewParameters.CreatedView = Application.CreateDetailView(space, space.CreateObject(typeof(LogonObject)));
                var dialogController = new DialogController { SaveOnAccept = true };
                dialogController.AcceptAction.Execute += AcceptActionOnExecute;
                showViewParameters.Controllers.Add(dialogController);
                showViewParameters.Controllers.Add(new MapController(true, (IPersistentAssemblyInfo)View.CurrentObject));
            }
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            ObjectSpace.Refresh();
        }
    }
}
