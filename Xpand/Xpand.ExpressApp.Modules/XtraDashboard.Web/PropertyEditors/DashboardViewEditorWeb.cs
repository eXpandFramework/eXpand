using DevExpress.DashboardWeb;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using System;
using DevExpress.DashboardCommon;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.PropertyEditors;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {

    [PropertyEditor(typeof(String), false)]
    public class DashboardViewEditorWeb : DashboardDesignerEditor,IDashboardViewEditor {

        public DashboardViewEditorWeb(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override IObjectDataSourceCustomFillService AttachService(XpandDashboardDataProvider xpandDashboardDataProvider){
            return xpandDashboardDataProvider.AttachViewService(Control.ServiceContainer,(IDashboardData) Definition);
        }

        protected override WorkingMode GetWorkingMode(){
            return WorkingMode.ViewerOnly;
        }
    }
}
