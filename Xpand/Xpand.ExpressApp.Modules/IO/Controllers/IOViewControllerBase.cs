using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.Base.General;

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
            
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            View view;
            if (ObjectSpace.FindObject(groupObjectType, null) == null) {
                IObjectSpace objectSpace = Application.CreateObjectSpace(XafTypesInfo.Instance.FindBussinessObjectType<ISerializationConfigurationGroup>()) ;                
                var objectFromInterface = objectSpace.CreateObjectFromInterface<ISerializationConfigurationGroup>();
                view = Application.CreateDetailView(objectSpace, objectFromInterface);
                
            }
            else {
                IObjectSpace objectSpace = Application.CreateObjectSpace(groupObjectType);
                view = Application.CreateListView(objectSpace, groupObjectType, true);
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
            Export(((SimpleActionExecuteEventArgs)actionBaseEventArgs).CurrentObject);
            ((ActionBase)sender).Model.SetValue("IsPostBackRequired", false);
        }

        public virtual void Export(object selectedObject) {
            if (selectedObject != null)
            {
                var exportEngine = new ExportEngine();
                var document = exportEngine.Export(View.SelectedObjects.OfType<XPBaseObject>(), ObjectSpace.GetObject((ISerializationConfigurationGroup)selectedObject));
                Save(document);
            }
        }

        protected abstract void Save(XDocument document);

        protected virtual void Import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = ((XPObjectSpace)Application.CreateObjectSpace(TypesInfo.Instance.XmlFileChooserType));
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            var detailView = Application.CreateDetailView(objectSpace, o);
            detailView.ViewEditMode=ViewEditMode.Edit;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            var dialogController = new DialogController { SaveOnAccept = false };
            dialogController.AcceptAction.Execute += (sender1, args) => {
                var memoryStream = new MemoryStream();
                var xmlFileChooser = ((IXmlFileChooser)args.CurrentObject);
                xmlFileChooser.FileData.SaveToStream(memoryStream);
                new ImportEngine(xmlFileChooser.ErrorHandling).ImportObjects(memoryStream, new UnitOfWork(objectSpace.Session.DataLayer));
            };


            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }
    }
}