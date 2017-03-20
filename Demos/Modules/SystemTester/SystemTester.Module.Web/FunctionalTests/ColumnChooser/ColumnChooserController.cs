using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule.CallbackHandlers;
using Xpand.Persistent.Base.General.Controllers;

namespace SystemTester.Module.Web.FunctionalTests.ColumnChooser {
    public class ColumnChooserController:ObjectViewController<ListView, ColumnChooserObject> {
        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<ListViewFastCallbackHandlerController>().Active[""] = false;
            Frame.GetController<EasyTestController>().ParametrizedAction.Execute+=ParametrizedActionOnExecute;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<ListViewFastCallbackHandlerController>().Active[""] = true;
            Frame.GetController<EasyTestController>().ParametrizedAction.Execute -= ParametrizedActionOnExecute;
        }

        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            if (parametrizedActionExecuteEventArgs.ParameterCurrentValue.ToString()== "CheckSameCaptionColumns") {
                var columnWrappers = ((ASPxGridListEditor) View.Editor).Columns.Where(wrapper => wrapper.Caption=="Reference Name");
                ((ParametrizedAction) sender).Value = null;
                foreach (var columnWrapper in columnWrappers){
                    ((ParametrizedAction) sender).Value += columnWrapper.PropertyName+",";
                }
                ((ParametrizedAction) sender).Value = ((ParametrizedAction) sender).Value.ToString().Trim(',');
            }
        }
    }
}
