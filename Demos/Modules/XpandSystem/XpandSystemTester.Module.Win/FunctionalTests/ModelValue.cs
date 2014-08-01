using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Helpers;

namespace XpandSystemTester.Module.Win.FunctionalTests {
    public class ModelValue:WindowController {
        public ModelValue(){
            TargetWindowType=WindowType.Main;
            var parametrizedAction = new ParametrizedAction(this,GetType().Name,PredefinedCategory.ObjectsCreation, typeof(string));
            parametrizedAction.Execute+=ParametrizedActionOnExecute;
        }

        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            var operands = parametrizedActionExecuteEventArgs.ParameterCurrentValue.ToString().Split(';');
            var path = operands[0];
            if (!path.StartsWith("Application"))
                path = "Application/" + path;
            var modelNode = Application.Model.GetNodeByPath(path);
            var modelValueInfo = modelNode.NodeInfo.ValuesInfo.First(info => info.Name==operands[1]);
            var typeConverter = modelValueInfo.TypeConverter;
            var value =typeConverter!=null? typeConverter.ConvertFrom(operands[2]):operands[2].Change(modelValueInfo.PropertyType);
            modelNode.SetValue(operands[1], value);
        }
    }
}
