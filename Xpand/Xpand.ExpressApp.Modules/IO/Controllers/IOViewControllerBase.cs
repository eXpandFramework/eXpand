using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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
            var objectSpace = Application.CreateObjectSpace();
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


        void Export(object selectedObject) {
            XDocument xDocument = new ExportEngine().Export(View.SelectedObjects.OfType<XPBaseObject>(), ObjectSpace.GetObject((ISerializationConfigurationGroup)selectedObject));
            var fileName = GetFilePath();
            if (fileName != null) {
                var xmlWriterSettings = new XmlWriterSettings {
                    OmitXmlDeclaration = true, Indent = true, NewLineChars = "\r\n", CloseOutput = true,
                };
                using (XmlWriter textWriter = XmlWriter.Create(new FileStream(fileName, FileMode.Create), xmlWriterSettings)) {
                    if (xDocument.Root != null)
                        textWriter.WriteString(XmlCharacterWhitelist(xDocument.Root.Value));
                    textWriter.Close();
                }
            }
        }

        string XmlCharacterWhitelist(string in_string) {
            if (in_string == null) return null;
            var sbOutput = new StringBuilder();
            foreach (char ch in in_string) {
                if ((ch >= 0x0020 && ch <= 0xD7FF) ||
                    (ch >= 0xE000 && ch <= 0xFFFD) ||
                    ch == 0x0009 ||
                    ch == 0x000A ||
                    ch == 0x000D) {
                    sbOutput.Append(ch);
                }
            }
            return sbOutput.ToString();
        }
        protected abstract string GetFilePath();

        void Import(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, o);
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1, args) => {
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