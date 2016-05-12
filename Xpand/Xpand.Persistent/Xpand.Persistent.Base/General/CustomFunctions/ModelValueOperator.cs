using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.General.CustomFunctions{
    public class ModelValueOperator : ICustomFunctionOperator {
        public const string OperatorName = "ModelValue";
        private static readonly ModelValueOperator _instance = new ModelValueOperator();

        public static ModelValueOperator Instance{
            get { return _instance; }
        }

        public string Name {
            get { return OperatorName; }
        }

        public object Evaluate(params object[] operands){
            var path = operands.First().ToString();
            if (!path.StartsWith("Application"))
                path = "Application/" + path;
            var nodeByPath = CaptionHelper.ApplicationModel.GetNodeByPath(path);
            return nodeByPath.GetValue(operands.Last().ToString());
        }

        public Type ResultType(params Type[] operands) {
            return typeof(string);
        }
    }
}
