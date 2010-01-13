using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.IO.Controllers {
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

        void IoActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs)
        {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "export")){
                export();
            }
            else{
                import(singleChoiceActionExecuteEventArgs);
            }
        }

        void export() {
            
            XDocument xDocument = new ExportEngine().Export(View.SelectedObjects.OfType<XPBaseObject>());
            var fileName = GetFilePath();
            if (fileName != null) xDocument.Save(fileName);
        }

        protected abstract string GetFilePath();

        void import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, o);
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1, args) =>{
                var memoryStream = new MemoryStream();
                ((IXmlFileChooser)args.CurrentObject).FileData.SaveToStream(memoryStream);
                new ImportEngine().ImportObjects(memoryStream, new UnitOfWork(objectSpace.Session.DataLayer));
            };
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }
    }
}