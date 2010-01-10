using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.IO.Controllers
{
    public partial class ViewController : DevExpress.ExpressApp.ViewController
    {
        public ViewController()
        {
            var importXmlAction = new SimpleAction {Id = "ImportXml",Caption = "ImportXml",Category = PredefinedCategory.Export.ToString()};

            Actions.Add(importXmlAction);
            importXmlAction.Execute+=ImportXmlActionOnExecute;
            InitializeComponent();
            RegisterActions(components);
        }

        void ImportXmlActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            object o = objectSpace.CreateObject(TypesInfo.Instance.XmlFileChooserType);
            simpleActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, o);
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute+=(sender1, args) => {
                var memoryStream = new MemoryStream();
                ((IXmlFileChooser) args.CurrentObject).FileData.SaveToStream(memoryStream);
                new ImportEngine().ImportObjects(memoryStream,  new UnitOfWork(objectSpace.Session.DataLayer));
            };            
            simpleActionExecuteEventArgs.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            simpleActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

    }
}
