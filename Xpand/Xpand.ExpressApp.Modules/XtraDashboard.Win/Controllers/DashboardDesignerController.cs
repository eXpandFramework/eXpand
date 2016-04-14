using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using System.Xml;
using System.Windows.Forms;
using DevExpress.DashboardWin;
using Xpand.ExpressApp.XtraDashboard.Win.Templates;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class DashboardDesignerController : Dashboard.Controllers.DashboardDesignerController {
        public DashboardDesignerController() {

        }

        protected override void DashboardEditExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashboardEditExecute(sender, e);
            using (var form = new DashboardDesignerForm { ObjectSpace = ObjectSpace }) {
                form.LoadTemplate(((IDashboardDefinition)View.CurrentObject), Application);
                OnDashboardDesignerOpening(new DashboardDesignerOpeningEventArgs(form.Designer));
                form.ShowDialog();
                if (View is ListView)
                    ObjectSpace.CommitChanges();
            }

        }

        protected override void DashbardExportXMLExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashbardExportXMLExecute(sender, e);
            var def = (IDashboardDefinition)View.CurrentObject;

            if (!string.IsNullOrEmpty(def.Xml)) {
                var saveFileDialog = new SaveFileDialog {
                    AddExtension = true,
                    Filter = @"XML files (*.xml)|*.xml",
                    FileName = def.Name + ".xml"
                };
                if (saveFileDialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
                    var xdoc = new XmlDocument();
                    xdoc.LoadXml(def.Xml);
                    xdoc.Save(saveFileDialog.FileName);
                }
            }
        }

        protected override void DashboardImportXMLExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashboardImportXMLExecute(sender, e);
            var def = (IDashboardDefinition)View.CurrentObject;
            var openFileDialog = new OpenFileDialog {
                AddExtension = true,
                Filter = @"XML files (*.xml)|*.xml",
                FileName = def.Name + ".xml"
            };
            if (openFileDialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
                var xdoc = new XmlDocument();
                xdoc.Load(openFileDialog.FileName);
                def.Xml = GetXMLAsString(xdoc);
            }
        }
    }

    public class DashboardDesignerOpeningEventArgs : Dashboard.Controllers.DashboardDesignerOpeningEventArgs {
        public DashboardDesignerOpeningEventArgs(DashboardDesigner designer) {
            Designer = designer;
        }

        public DashboardDesigner Designer { get; private set; }
    }

}