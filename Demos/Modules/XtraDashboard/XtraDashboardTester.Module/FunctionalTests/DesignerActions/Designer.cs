using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace XtraDashboardTester.Module.FunctionalTests.DesignerActions {
    public class Designer:ObjectViewController<DetailView,DashboardDefinition> {
        private readonly SimpleAction _invalidNewDashboardTypes;
        private IPropertyEditor _dashboardTypesEditor;
        private IPropertyEditor _xmlPropertyEditor;
        private readonly SimpleAction _invalidNewDashboardXml;

        public Designer(){
            _invalidNewDashboardTypes = new SimpleAction(this,"InvalidNewDashboardTypes",PredefinedCategory.View);
            _invalidNewDashboardXml = new SimpleAction(this,"InvalidNewDashboardXml",PredefinedCategory.View);
            var simpleAction = new SimpleAction(this,"SetNewDashboardFields" ,PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var dashboardDefinition = ((DashboardDefinition) View.CurrentObject);
            _dashboardTypesEditor.SetValue("Customer;Person");
            if (Application.IsHosted())
                dashboardDefinition.Xml = GetType().GetResourceString("NewDashboard.xml");
        }

        protected override void OnActivated(){
            base.OnActivated();
            if(View.ViewEditMode==ViewEditMode.Edit){
                _dashboardTypesEditor = View.GetItems<IPropertyEditor>().First(editor => editor.MemberInfo.Name=="DashboardTypes");
                _dashboardTypesEditor.ValueRead+=DashboardTypesEditorOnValueRead;
                _xmlPropertyEditor = View.GetItems<IPropertyEditor>().First(editor => editor.MemberInfo.Name=="Xml");
                _xmlPropertyEditor.ValueRead += XMLPropertyEditorOnValueRead;
            }
        }

        private void XMLPropertyEditorOnValueRead(object sender, EventArgs eventArgs){
            _xmlPropertyEditor.ValueRead-=XMLPropertyEditorOnValueRead;
            string xml = ((DashboardDefinition) View.CurrentObject).Xml+"";
            _invalidNewDashboardXml.Active["xml"] = !(xml.Contains("Customer") && xml.Contains("Person") && xml.Contains("Chart"));
        }

        private void DashboardTypesEditorOnValueRead(object sender, EventArgs eventArgs){
            _dashboardTypesEditor.ValueRead-=DashboardTypesEditorOnValueRead;
            _invalidNewDashboardTypes.Active["dashboradtypes"] = (string)_dashboardTypesEditor.ControlValue != "Customer, Person";
        }
    }
}
