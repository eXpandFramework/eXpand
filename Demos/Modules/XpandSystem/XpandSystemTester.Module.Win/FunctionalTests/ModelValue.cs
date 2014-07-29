using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Helpers;

namespace XpandSystemTester.Module.Win.FunctionalTests {
    public class ModelValue:ViewController {
        public ModelValue(){
            var parametrizedAction = new ParametrizedAction(this,GetType().Name,PredefinedCategory.ObjectsCreation, typeof(string));
            parametrizedAction.Execute+=ParametrizedActionOnExecute;
        }

        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            var operands = parametrizedActionExecuteEventArgs.ParameterCurrentValue.ToString().Split(';');
            var path = operands[0];
            if (!path.StartsWith("Application"))
                path = "Application/" + path;
            var modelNode = Application.Model.GetNodeByPath(path);
            var propertyType = modelNode.NodeInfo.ValuesInfo.First(info => info.Name==operands[1]).PropertyType;
            var value = operands[2].Change(propertyType);
            modelNode.SetValue(operands[1], value);
        }
    }
}
