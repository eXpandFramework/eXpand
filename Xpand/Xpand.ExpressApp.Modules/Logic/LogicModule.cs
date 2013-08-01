using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Logic.DomainLogic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {

    [ToolboxItem(false)]
    public class LogicModule : XpandModuleBase, IModelXmlConverter {
        public override void CustomizeLogics(CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            customLogics.RegisterLogic(typeof(IModelViewContext), typeof(ModelViewContextDomainLogic));
            customLogics.RegisterLogic(typeof(ILogicRule), typeof(LogicRuleExecutionContextGroupDomainLogic));
            customLogics.RegisterLogic(typeof(IModelExecutionContext), typeof(ModelExecutionContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelExecutionContextsGroup), typeof(ModelExecutionContextsGroupDefaultContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelFrameTemplateContext), typeof(ModelFrameTemplateContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelLogicRule), typeof(ModelLogicRuleDomainLogic));
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
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