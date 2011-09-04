using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using Xpand.Persistent.Base.Validation.FromIPropertyValueValidator;

namespace Xpand.ExpressApp.Validation {
    [ToolboxBitmap(typeof(XpandValidationModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandValidationModule : ModuleBase {
        public XpandValidationModule() {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var registrator = new ValidationRulesRegistrator(moduleManager);
            registrator.RegisterRule(typeof(RuleRequiredForAtLeast1Property), typeof(IRuleRequiredForAtLeast1PropertyProperties));
            registrator.RegisterRule(typeof(RuleFromIPropertyValueValidator), typeof(IRuleFromIPropertyValueValidatorProperties));
        }
    }
}
