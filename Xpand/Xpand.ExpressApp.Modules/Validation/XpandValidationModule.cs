using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using Xpand.Persistent.Base.Validation.FromIPropertyValueValidator;

namespace Xpand.ExpressApp.Validation {
    [ToolboxItem(false)]
    public sealed class XpandValidationModule : XpandModuleBase {
        public XpandValidationModule() {
            RequiredModuleTypes.Add(typeof(ValidationModule));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new RuleTypeGeneratorUpdater());
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleRequiredForAtLeast1Property),
                                     typeof(IRuleRequiredForAtLeast1PropertyProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleFromIPropertyValueValidator),
                                     typeof(IRuleFromIPropertyValueValidatorProperties));
        }
    }
}