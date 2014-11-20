using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Web.FunctionalTests.MasterDetail {
    public class MasterDetail:ViewController<ListView>{
        public MasterDetail(){
            TargetObjectType = typeof (Master);
            var singleChoiceAction = new SimpleAction(this,"NewDetail",PredefinedCategory.View);
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var newObjectViewController = View.EditView.GetItems<ListPropertyEditor>().First().Frame.GetController<NewObjectViewController>();
            newObjectViewController.NewObjectAction.DoExecute();
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var asPxGridListEditor = ((ASPxGridListEditor)View.Editor);
            asPxGridListEditor.Grid.DataBound+=GridOnDataBound;
        }

        private void GridOnDataBound(object sender, EventArgs eventArgs){
            var asPxGridView = ((ASPxGridView) sender);
            asPxGridView.FocusedRowIndex = 0;
        }

    }
}
