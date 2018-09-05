using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.IO.Controllers {
    public abstract class IOViewControllerBase : ViewController {

        readonly SingleChoiceAction _ioAction;

        public SingleChoiceAction IOAction => _ioAction;

        protected IOViewControllerBase() {
            _ioAction = new SingleChoiceAction(this, "IO", PredefinedCategory.Export) { ItemType = SingleChoiceActionItemType.ItemIsOperation };
            Actions.Add(_ioAction);
            _ioAction.Items.Add(new ChoiceActionItem("Export", "export"));
            _ioAction.Items.Add(new ChoiceActionItem("Import", "import"));
            _ioAction.Execute += IoActionOnExecute;
        }

        void IoActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "export")) {
                ShowSerializationView(singleChoiceActionExecuteEventArgs);
            } else {
                Import(singleChoiceActionExecuteEventArgs);
            }
        }

        void ShowSerializationView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            View view;
            var objectSpace = Application.CreateObjectSpace<ISerializationConfigurationGroup>();
            if (!objectSpace.QueryObjects<ISerializationConfigurationGroup>().Any()) {                
                var serializationConfigurationGroup = objectSpace.Create<ISerializationConfigurationGroup>();
                view = Application.CreateDetailView(objectSpace, serializationConfigurationGroup);
            }
            else {
                view = Application.CreateListView<ISerializationConfigurationGroup>(objectSpace);
            }
            showViewParameters.CreatedView = view;
            AddDialogController(showViewParameters);

        }

        void AddDialogController(ShowViewParameters showViewParameters) {
            var dialogController = new DialogController();
            dialogController.AcceptAction.Model.SetValue("IsPostBackRequired", true);
            dialogController.CloseOnCurrentObjectProcessing = true;
            dialogController.AcceptAction.Executed+=AcceptActionOnExecuteCompleted;
            showViewParameters.Controllers.Add(dialogController);
        }

        void AcceptActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            Export((ISerializationConfigurationGroup) ((SimpleActionExecuteEventArgs)actionBaseEventArgs).CurrentObject);
            ((ActionBase)sender).Model.SetValue("IsPostBackRequired", false);
        }

        public virtual void Export(ISerializationConfigurationGroup selectedObject) {
            var exportEngine = new ExportEngine(selectedObject.XPObjectSpace());
            var document = exportEngine.Export(View.SelectedObjects.OfType<XPBaseObject>(), selectedObject);
            Save(document);
        }

        protected abstract void Save(XDocument document);

        protected virtual void Import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = Application.CreateObjectSpace<IFileChooser>();
            object o = objectSpace.Create<IFileChooser>();
            var detailView = Application.CreateDetailView(objectSpace, o);
            detailView.ViewEditMode=ViewEditMode.Edit;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            var dialogController = new DialogController { SaveOnAccept = true };
            dialogController.AcceptAction.Execute += (sender1, args) => {
                var memoryStream = new MemoryStream();
                var fileChooser = ((IFileChooser)args.CurrentObject);
                fileChooser.FileData.SaveToStream(memoryStream);
                memoryStream.Position = 0;
                new ImportEngine(fileChooser.ErrorHandling).ImportObjects(memoryStream, CreateObjectSpace);
            };
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        private IObjectSpace CreateObjectSpace(ITypeInfo typeInfo){
            return Application.ObjectSpaceProviders.First(
                    provider => provider.EntityStore.RegisteredEntities.Contains(typeInfo.Type)).CreateObjectSpace();
        }
    }
}