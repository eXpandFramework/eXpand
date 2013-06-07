using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.DashboardWin;
using Xpand.ExpressApp.XtraDashboard.Win.Helpers;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors {
    [PropertyEditor(typeof(String), false)]
    public class DashboardViewEditor : WinPropertyEditor, IComplexPropertyEditor {
        XafApplication _application;
        IObjectSpace _objectSpace;

        public DashboardViewEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        public DashboardViewer DashboardViewer {
            get { return (DashboardViewer)Control; }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public XafApplication Application {
            get { return _application; }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _application = application;
        }

        protected override object CreateControlCore() {
            return new DashboardViewer { Margin = new Padding(0), Padding = new Padding(0) };
        }

        protected override void ReadValueCore() {
            Control.BeginInvoke(new Action(() => {
                var template = CurrentObject as IDashboardDefinition;
                DashboardViewer.Dashboard = template.CreateDashBoard(ObjectSpace, false);
            }));

        }
    }
}