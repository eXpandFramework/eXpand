using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Helpers;

namespace XpandSystemTester.Module.FunctionalTests {
    public class XpandEasyTestController:ViewController{
        private readonly ParametrizedAction _parameterAction;
        public XpandEasyTestController(){
            _parameterAction = new ParametrizedAction(this, "Parameter Value", PredefinedCategory.ObjectsCreation, typeof(string));
            _parameterAction.Execute += ParametrizedActionOnExecute;
            
        }

        public ParametrizedAction ParameterAction{
            get { return _parameterAction; }
        }

        public virtual string ChangeColumnCaption(string caption){
            return null;
        }
        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs) {
            var currentValue = parametrizedActionExecuteEventArgs.ParameterCurrentValue.ToString();
            var operands = currentValue.Split(';');
            var path = operands[0];
            if (!path.StartsWith("Application"))
                path = "Application/" + path;
            var modelNode = Application.Model.GetNodeByPath(path);
            var modelValueInfo = modelNode.NodeInfo.ValuesInfo.First(info => info.Name == operands[1]);
            var typeConverter = modelValueInfo.TypeConverter;
            var value = typeConverter != null ? typeConverter.ConvertFrom(operands[2]) : operands[2].Change(modelValueInfo.PropertyType);
            modelNode.SetValue(operands[1], value);
        }

        public string Value {
            get { return _parameterAction.Value as string; }
        }
    }
}
