using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Ionic.Zip;
using Ionic.Zlib;
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

        protected override void OnActivated() {
            base.OnActivated();
            var bussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType(typeof(ISerializationConfigurationGroup));
            _ioAction.Active["Security"] =DataManipulationRight.CanRead(bussinessObjectType, null, null, null, ObjectSpace);
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
            AddDialogController(showViewParameters,Application);

        }

        void AddDialogController(ShowViewParameters showViewParameters, XafApplication xafApplication) {
            var dialogController = xafApplication.CreateController<DialogController>();
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
            using (var memoryStream = Save(document)){
                if (selectedObject.ZipOutput) {
                    Zip(memoryStream);
                }
                else {
                    Save(memoryStream);
                }
            }
            
            
        }

        protected abstract void Save(MemoryStream memoryStream);

        protected virtual void Zip(MemoryStream xmlStream){
            using (var zip = CreateZipFile()) {
                var xml = Encoding.UTF8.GetString(xmlStream.ToArray());
                zip.AddEntry($"{View.ObjectTypeInfo.Name}.xml", xml);
                Save(zip);
            }

        }

        protected virtual ZipFile CreateZipFile() {
            return new ZipFile(Encoding.UTF8) {
                AlternateEncoding = Encoding.UTF8, CompressionLevel = CompressionLevel.BestCompression
            };
        }

        protected abstract void Save(ZipFile zipFile);

        protected  MemoryStream Save( XDocument document) {
            var memoryStream = new MemoryStream();
            using (var textWriter = XmlWriter.Create(memoryStream, ExportEngine.GetXMLWriterSettings(document.IsMinified()))) {
                document.Save(textWriter);
                textWriter.Close();
            }

            return memoryStream;
        }

        protected  string GetExtension(bool isZipped){
            return isZipped ? "zip" : "xml";
        }

        protected virtual void Import(SingleChoiceActionExecuteEventArgs e) {
            var objectSpace = Application.CreateObjectSpace<IFileChooser>();
            object o = objectSpace.Create<IFileChooser>();
            var detailView = Application.CreateDetailView(objectSpace, o);
            detailView.ViewEditMode=ViewEditMode.Edit;
            e.ShowViewParameters.CreatedView = detailView;
            
            var dialogController = e.Action.Application.CreateController<DialogController>();
            dialogController.SaveOnAccept = true;
            dialogController.AcceptAction.Execute += (sender1, args) => {
                Stream stream = new MemoryStream();
                try {
                    var fileChooser = ((IFileChooser)args.CurrentObject);
                    fileChooser.FileData.SaveToStream(stream);
                    if (fileChooser.FileData.FileName.ToLower().EndsWith(".zip")) {
                        stream.Position = 0;
                        var zipFile = ZipFile.Read(stream);
                        stream=ExtractZip(zipFile);
                    }
                    stream.Position = 0;
                    new ImportEngine(fileChooser.ErrorHandling).ImportObjects(stream, CreateObjectSpace);
                }
                finally {
                    stream?.Dispose();    
                }
            };
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogController);
        }

        protected virtual Stream ExtractZip(ZipFile zipFile) {
            var path = Path.GetTempPath() + Guid.NewGuid();
            zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
            zipFile.ExtractAll(path);
            var fileName = Directory.GetFiles(path).First();
            var memoryStream = new MemoryStream();
            var sw = new StreamWriter(memoryStream);
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            var allLines = File.ReadAllLines(fileName);
            var firstLine = allLines.First();
            if (firstLine.StartsWith(byteOrderMarkUtf8)) {
                firstLine = firstLine.Remove(0, byteOrderMarkUtf8.Length);
            }
            sw.Write(firstLine);
            foreach (var readAllLine in allLines.Skip(1)) {
                sw.Write(readAllLine);
            }
            sw.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private IObjectSpace CreateObjectSpace(ITypeInfo typeInfo){
            var typeInfoType = typeInfo?.Type??XafTypesInfo.Instance.FindBussinessObjectType<IIOError>();
            return Application.ObjectSpaceProviders.First(
                    provider => provider.EntityStore.RegisteredEntities.Contains(typeInfoType)).CreateObjectSpace();
        }

        protected string GetFileName(bool isZipped){
            return CaptionHelper.GetClassCaption(View.ObjectTypeInfo.Type.FullName) + (isZipped ? ".zip" : ".xml");
        }
    }
}