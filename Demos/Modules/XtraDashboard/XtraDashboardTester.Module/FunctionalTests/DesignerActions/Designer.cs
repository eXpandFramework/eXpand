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
        private const string InvalidNewDashboardXml = "InvalidNewDashboardXml";
        private const string InvalidNewDashboardTypes = "InvalidNewDashboardTypes";
        private const string SetNewDashboardFields = "SetNewDashboardFields";

        private IPropertyEditor _dashboardTypesEditor;
        private IPropertyEditor _xmlPropertyEditor;
        private readonly SingleChoiceAction _designerAction;

        public Designer(){
            _designerAction = new SingleChoiceAction(this,"Designer",PredefinedCategory.View);
            _designerAction.Items.Add(new ChoiceActionItem(SetNewDashboardFields, null));
            _designerAction.Items.Add(new ChoiceActionItem(InvalidNewDashboardTypes, InvalidNewDashboardTypes));
            _designerAction.Items.Add(new ChoiceActionItem(InvalidNewDashboardXml, InvalidNewDashboardXml));
            _designerAction.Execute += _designerAction_Execute;
        }

        private void _designerAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e){
            var selectedChoiceActionItem = e.SelectedChoiceActionItem;
            if (selectedChoiceActionItem.Caption == SetNewDashboardFields){
                var dashboardDefinition = ((DashboardDefinition)View.CurrentObject);
                _dashboardTypesEditor.SetValue("Customer;Person");
                if (Application.IsHosted())
                    dashboardDefinition.Xml = GetType().GetResourceString("NewDashboard.xml");
            }
        }

        protected override void OnActivated(){
            base.OnActivated();
            if(View.ViewEditMode==ViewEditMode.Edit){
                _dashboardTypesEditor = View.GetItems<IPropertyEditor>().First(editor => editor.MemberInfo.Name=="DashboardTypes");
                _dashboardTypesEditor.ValueRead+=DashboardTypesEditorOnValueRead;
                _xmlPropertyEditor = View.GetItems<IPropertyEditor>().FirstOrDefault(editor => editor.MemberInfo.Name=="Xml");
                if (_xmlPropertyEditor != null) _xmlPropertyEditor.ValueRead += XMLPropertyEditorOnValueRead;
            }
        }

        private void XMLPropertyEditorOnValueRead(object sender, EventArgs eventArgs){
            _xmlPropertyEditor.ValueRead-=XMLPropertyEditorOnValueRead;
            var dashboardDefinition = (DashboardDefinition) View.CurrentObject;
            if (dashboardDefinition != null){
                string xml = dashboardDefinition.Xml+"";
                _designerAction.Items.Find(InvalidNewDashboardXml).Active["xml"] = !(xml.Contains("Customer") && xml.Contains("Person") && xml.Contains("Chart"));
            }
        }

        private void DashboardTypesEditorOnValueRead(object sender, EventArgs eventArgs){
            _dashboardTypesEditor.ValueRead-=DashboardTypesEditorOnValueRead;
            _designerAction.Items.Find(InvalidNewDashboardTypes).Active["dashboradtypes"] = (string)_dashboardTypesEditor.ControlValue != "Customer, Person";
        }
    }
}
