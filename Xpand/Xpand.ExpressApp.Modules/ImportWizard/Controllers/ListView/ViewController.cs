using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using System.Linq;

namespace eXpand.ExpressApp.IO.Controllers.ListView
{
    public partial class ViewController : ViewController<DevExpress.ExpressApp.ListView>
    {
        readonly SimpleAction _exportToXmlAction;

        public ViewController()
        {
            _exportToXmlAction = new SimpleAction(Container) {
                                                                 Id = "ExportToXml",
                                                                 Caption = "ExportToXml",
                                                                 Category = PredefinedCategory.Export.ToString()
                                                             };
            _exportToXmlAction.Execute += SelectExportConfiguration;
            Actions.Add(_exportToXmlAction);
            InitializeComponent();
            RegisterActions(components);

        }

        public SimpleAction ExportToXmlAction
        {
            get { return _exportToXmlAction; }
        }

        void SelectExportConfiguration(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs)
        {
            XDocument xDocument = new ExportEngine().Export(View.SelectedObjects.OfType<XPBaseObject>());
            var serializationConfiguration = SerializationConfigurationQuery.Find(ObjectSpace.Session, View.ObjectTypeInfo.Type);
            xDocument.Save(serializationConfiguration.TypeToSerialize.Name + ".xml");
        }


    }
}