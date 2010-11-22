using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.Controllers {
    public abstract class IOViewControllerBase : ViewController {

        readonly SingleChoiceAction _ioAction;

        public SingleChoiceAction IOAction {
            get { return _ioAction; }
        }

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
            var groupObjectType = XafTypesInfo.Instance.FindBussinessObjectType<ISerializationConfigurationGroup>();
            var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            var objectSpace = Application.CreateObjectSpace() as ObjectSpace;
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            showViewParameters.Context = TemplateContext.View;
            showViewParameters.CreateAllControllers = true;
            View view;
            if (ObjectSpace.FindObject(groupObjectType, null) == null)
                view = Application.CreateDetailView(objectSpace, objectSpace.CreateObjectFromInterface<ISerializationConfigurationGroup>());
            else {
                view = Application.CreateListView(objectSpace, groupObjectType, true);
            }
            showViewParameters.CreatedView = view;
            AddDialogController(showViewParameters);

        }

        void AddDialogController(ShowViewParameters showViewParameters) {
            var dialogController = new DialogController();
            dialogController.ViewClosing += (o, eventArgs) => Export(((View)o).CurrentObject);
            showViewParameters.Controllers.Add(dialogController);
        }


        public virtual void Export(object selectedObject) {
            var fileName = GetFilePath();
            var exportEngine = new ExportEngine();
            exportEngine.Export(View.SelectedObjects.OfType<XPBaseObject>(), ObjectSpace.GetObject((ISerializationConfigurationGroup)selectedObject), fileName);
        }

        protected abstract string GetFilePath();

        protected virtual void Import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            ObjectSpace objectSpace = Application.CreateObjectSpace() as ObjectSpace;
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, o);
            var dialogController = new DialogController { SaveOnAccept = false };
            dialogController.AcceptAction.Execute += (sender1, args) => {
                var memoryStream = new MemoryStream();
                ((IXmlFileChooser)args.CurrentObject).FileData.SaveToStream(memoryStream);
                using (var unitOfWork = new UnitOfWork(objectSpace.Session.DataLayer)) {
                    new ImportEngine().ImportObjects(memoryStream, unitOfWork);
                }
            };
            ((ISupportConfirmationRequired)Application).ConfirmationRequired += OnConfirmationRequired;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView.Closed += (sender, eventArgs) => ((ISupportConfirmationRequired)Application).ConfirmationRequired -= OnConfirmationRequired;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        void OnConfirmationRequired(object sender, CancelEventArgs cancelEventArgs) {
            cancelEventArgs.Cancel = true;
        }
    }
}