using System.Collections;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ImportExport.PersistentTypesHelpers;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Xpo;

namespace eXpand.ExpressApp.IO.Controllers.ListView
{
    public partial class ViewController : ViewController<DevExpress.ExpressApp.ListView>
    {
        readonly SimpleAction _exportToXmlAction;

        public ViewController()
        {
            _exportToXmlAction = new SimpleAction(Container) { Id = "ExportToXml", Caption = "ExportToXml" };
            _exportToXmlAction.Active["objectType"] = false;
            _exportToXmlAction.Category = PredefinedCategory.View.ToString();
            _exportToXmlAction.Execute += SelectExportConfiguration;
            Actions.Add(_exportToXmlAction);
            InitializeComponent();
            RegisterActions(components);

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var serializationConfigurationType = View.ObjectTypeInfo.Type;
            _exportToXmlAction.Active["objectType"] = ObjectSpace.Session.GetCount(TypesInfo.Instance.SerializationConfigurationType,
                                                                                   SerializationConfigurationQuery.GetCriteria(serializationConfigurationType)) > 0;
        }
        public SimpleAction ExportToXmlAction
        {
            get { return _exportToXmlAction; }
        }

        void SelectExportConfiguration(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs)
        {
            var objectSpace = Application.CreateObjectSpace();
            var serializationConfigurationType = TypesInfo.Instance.SerializationConfigurationType;
            var collectionSource = new CollectionSource(objectSpace, serializationConfigurationType);
            collectionSource.Criteria["ObjectType"] = SerializationConfigurationQuery.GetCriteria(View.ObjectTypeInfo.Type);
            string findListViewId = Application.FindListViewId(TypesInfo.Instance.SerializationConfigurationType);
            simpleActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateListView(findListViewId, collectionSource,
                                                                                                     false);
            simpleActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            simpleActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
            dialogController.AcceptAction.Execute += ExportActionOnExecute;

        }

        void ExportActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            foreach (object selectedObject in simpleActionExecuteEventArgs.SelectedObjects) {
                ExportObjects(View.CollectionSource.Collection,(ISerializationConfiguration) selectedObject);
            }                
        }

        protected virtual void ExportObjects(IList viewSelectedObject, ISerializationConfiguration serializationConfiguration) {
            XDocument xDocument = new ExportEngine().Export(viewSelectedObject,serializationConfiguration);
            xDocument.Save(serializationConfiguration.Name+".xml");
        }
    }
}