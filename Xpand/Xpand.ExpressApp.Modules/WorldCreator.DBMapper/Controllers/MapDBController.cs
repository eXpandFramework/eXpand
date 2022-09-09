using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.WorldCreator.Controllers;
using Xpand.ExpressApp.WorldCreator.DBMapper.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.Controllers {
    public class MapDBController : ViewController<DetailView> {
        public MapDBController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<AssemblyToolsController>(assemblyToolsController => {
                if (assemblyToolsController.SingleChoiceAction.Items.Find("MapDB") == null) {
                    assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Map Datastore", "MapDB"));
                    assemblyToolsController.SingleChoiceAction.Execute += AssemblyToolsControllerOnToolExecuted;
                }
            });
            
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs e) {
            if ((string)e.SelectedChoiceActionItem.Data == "MapDB") {
                var nestedObjectSpace = Application.CreateNestedObjectSpace(ObjectSpace);
                var showViewParameters = e.ShowViewParameters;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                var detailView = Application.CreateDetailView(nestedObjectSpace, nestedObjectSpace.CreateObject(typeof(DBLogonObject)));
                detailView.ViewEditMode=ViewEditMode.Edit;
                showViewParameters.CreatedView = detailView;
                var dialogController = e.Action.Application.CreateController<DialogController>();
                dialogController.AcceptAction. Execute+=OnExecute;
                showViewParameters.Controllers.Add(dialogController);
                var mapDBController = Application.CreateController<ChooseDBObjectsController>();
                mapDBController.AssemblyInfo = nestedObjectSpace.GetObject((IPersistentAssemblyInfo)View.CurrentObject);
                showViewParameters.Controllers.Add(mapDBController);
            }
        }

        private void OnExecute(object sender, ActionBaseEventArgs actionBaseEventArgs){
            actionBaseEventArgs.Action.Controller.Frame.GetController<ChooseDBObjectsController>(chooseDBObjectsController => {
                chooseDBObjectsController.View.ObjectSpace.CommitChanges();
                var dbLogonObject = ((DBLogonObject)chooseDBObjectsController.View.CurrentObject);
                var persistentAssemblyInfo = ((IPersistentAssemblyInfo)View.CurrentObject);
                ObjectSpace.Delete(persistentAssemblyInfo.PersistentClassInfos);
                ObjectSpace.Delete(persistentAssemblyInfo.Attributes.OfType<IPersistentAssemblyDataStoreAttribute>().FirstOrDefault());
                var assemblyGenerator = new AssemblyGenerator.AssemblyGenerator(dbLogonObject.ConnectionString, dbLogonObject.NavigationPath, persistentAssemblyInfo, chooseDBObjectsController.GetSelectedDBObjects());
                assemblyGenerator.Create();
            });
        }
    }

}
