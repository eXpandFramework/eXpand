using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic {

    [ToolboxItem(false)]
    public class LogicModule : XpandModuleBase, IModelXmlConverter {
        public void ConvertXml(ConvertXmlParameters parameters) {
            if (typeof(IModelExecutionContext).IsAssignableFrom(parameters.NodeType)) {
                switch (parameters.Values["Name"]) {
                    case "ViewControlAdding":
                        parameters.Values["Name"] = ExecutionContext.ViewChanged.ToString();
                        break;
                    case "ObjectChanged":
                        parameters.Values["Name"] = ExecutionContext.ObjectSpaceObjectChanged.ToString();
                        break;
                }
            }
        }
    }
}