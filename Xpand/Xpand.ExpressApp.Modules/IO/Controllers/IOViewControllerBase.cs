using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.Controllers {
    public abstract class IOViewControllerBase : ViewController
    {
        
        readonly SingleChoiceAction _ioAction;

        public SingleChoiceAction IOAction
        {
            get { return _ioAction; }
        }

        protected IOViewControllerBase()
        {
            _ioAction = new SingleChoiceAction (this,"IO",PredefinedCategory.Export){ItemType = SingleChoiceActionItemType.ItemIsOperation};
            Actions.Add(_ioAction);
            _ioAction.Items.Add(new ChoiceActionItem("Export", "export"));
            _ioAction.Items.Add(new ChoiceActionItem("Import", "import"));
            _ioAction.Execute += IoActionOnExecute;
        }

        void IoActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs){
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "export")) {
                ShowSerializationView(singleChoiceActionExecuteEventArgs);
            }
            else{
                Import(singleChoiceActionExecuteEventArgs);
            }
        }

        void ShowSerializationView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var groupObjectType = XafTypesInfo.Instance.FindBussinessObjectType<ISerializationConfigurationGroup>();
            var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            var objectSpace = Application.CreateObjectSpace();
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            showViewParameters.Context = TemplateContext.View;
            showViewParameters.CreateAllControllers = true;
            View view;
            if (ObjectSpace.FindObject(groupObjectType, null) == null)
                view=Application.CreateDetailView(objectSpace,objectSpace.CreateObjectFromInterface<ISerializationConfigurationGroup>());
            else {
                view = Application.CreateListView(objectSpace, groupObjectType, true);
            }
            showViewParameters.CreatedView = view;
            AddDialogController(showViewParameters);
            
        }

        void AddDialogController(ShowViewParameters showViewParameters) {
            var dialogController = new DialogController();
            dialogController.ViewClosing += (o, eventArgs) => export(((View) o).CurrentObject);
            showViewParameters.Controllers.Add(dialogController);
        }


        void export(object selectedObject) {
            XDocument xDocument = new ExportEngine().Export(View.SelectedObjects.OfType<XPBaseObject>(), ObjectSpace.GetObject((ISerializationConfigurationGroup)selectedObject));
            var fileName = GetFilePath();
            if (fileName != null) {
                var xmlWriterSettings = new XmlWriterSettings {
                    OmitXmlDeclaration = true, Indent = true,NewLineChars = "\r\n",CloseOutput = true,};
                using (XmlWriter textWriter = XmlWriter.Create(new FileStream(fileName,FileMode.Create), xmlWriterSettings)){
                    if (textWriter!= null) {
                        xDocument.Save(textWriter);
                        textWriter.Close();
                    }
                }
            }
        }

        protected abstract string GetFilePath();

        void Import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, o);
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1, args) =>{
                var memoryStream = new MemoryStream();
                ((IXmlFileChooser)args.CurrentObject).FileData.SaveToStream(memoryStream);
                using (var unitOfWork = new UnitOfWork(objectSpace.Session.DataLayer)) {
                    new ImportEngine().ImportObjects(memoryStream, unitOfWork);
                }
            };
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }
    }
}