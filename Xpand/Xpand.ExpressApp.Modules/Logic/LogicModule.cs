using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic.DomainLogic;
using Xpand.ExpressApp.Security;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    [ToolboxItem(true)]
    public class LogicModule : XpandModuleBase, IModelXmlConverter {
        readonly LogicRuleCollector _logicRuleCollector = new LogicRuleCollector();

        public LogicModule() {
            RequiredModuleTypes.Add(typeof(SecurityModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }

        public LogicRuleCollector LogicRuleCollector => _logicRuleCollector;

        public override void CustomizeLogics(CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            customLogics.RegisterLogic(typeof(IModelViewContext), typeof(ModelViewContextDomainLogic));
            customLogics.RegisterLogic(typeof(ILogicRule), typeof(LogicRuleDomainLogic));
            customLogics.RegisterLogic(typeof(IContextLogicRule), typeof(ContextLogicRuleDomainLogic));
            customLogics.RegisterLogic(typeof(IModelExecutionContext), typeof(ModelExecutionContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelActionExecutionContext), typeof(ModelActionExecutionContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelExecutionContexts), typeof(ModelExecutionContextsDomainLogic));
            customLogics.RegisterLogic(typeof(IModelFrameTemplateContexts), typeof(ModelFrameTemplateContextsDomainLogic));
            customLogics.RegisterLogic(typeof(IModelExecutionContextsGroup), typeof(ModelExecutionContextsGroupDomainLogic));
            customLogics.RegisterLogic(typeof(IModelFrameTemplateContext), typeof(ModelFrameTemplateContextDomainLogic));
            customLogics.RegisterLogic(typeof(IModelLogicRule), typeof(ModelLogicRuleDomainLogic));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            _logicRuleCollector.Attach(this);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var logicRuleTypes = typesInfo.DomainSealedInfos<IContextLogicRule>();
            foreach (var logicRuleType in logicRuleTypes){
                logicRuleType.AddAttribute(new NewObjectCreateGroupAttribute("Logic"));    
            }
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