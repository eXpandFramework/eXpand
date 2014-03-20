using System.Web.UI.WebControls;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using System;
using System.Linq;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.PropertyEditors;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    public class DashboardViewerModelAdapter : Dashboard.PropertyEditors.DashboardViewerModelAdapter {
        protected override Type GetControlType() {
            return typeof(ASPxDashboardViewer);
        }
    }

    [PropertyEditor(typeof(String), false)]
    public class DashboardViewEditorWeb : WebPropertyEditor, IComplexViewItem,IDashboardViewEditor {
        ASPxDashboardViewer _asPxDashboardViewer;
        XafApplication _application;
        IObjectSpace _objectSpace;

        public DashboardViewEditorWeb(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _application = application;
        }

        protected override WebControl CreateViewModeControlCore() {
            _asPxDashboardViewer = CreateDashboardViewer();
            return _asPxDashboardViewer;
        }

        protected override WebControl CreateEditModeControlCore() {
            _asPxDashboardViewer = CreateDashboardViewer();
            return _asPxDashboardViewer;
        }

        protected override void ReadEditModeValueCore() { }

        protected override object GetControlValueCore() {
            return null;
        }

        private ASPxDashboardViewer CreateDashboardViewer() {
            var control = new ASPxDashboardViewer{ DashboardId = Definition.Name, RegisterJQuery = true,Width = Unit.Percentage(100)};
            control.DashboardLoading += DashboardLoading;
            control.DataLoading += DataLoading;
            return control;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (_asPxDashboardViewer != null && unwireEventsOnly) {
                _asPxDashboardViewer.DashboardLoading -= DashboardLoading;
                _asPxDashboardViewer.DataLoading -= DataLoading;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        void DashboardLoading(object sender, DashboardLoadingEventArgs e) {
            var template = CurrentObject as IDashboardDefinition;
            if (template != null) e.DashboardXml = template.Xml;
        }

        void DataLoading(object sender, DataLoadingWebEventArgs e) {
            if (e.Data == null) {
                var dsType = Definition.DashboardTypes.First(t => t.Caption == e.DataSourceName).Type;
                e.Data = _objectSpace.GetObjects(dsType);
            }
        }

        IDashboardDefinition Definition {
            get { return CurrentObject as IDashboardDefinition; }
        }

        public ASPxDashboardViewer DashboardViewer {
            get { return (ASPxDashboardViewer)Control; }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public XafApplication Application {
            get { return _application; }
        }
    }
}
