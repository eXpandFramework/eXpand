using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using Xpand.Persistent.Base.Validation.FromIPropertyValueValidator;

namespace Xpand.ExpressApp.Validation
{
    public sealed partial class XpandValidationModule : ModuleBase
    {
        public XpandValidationModule()
        {
            InitializeComponent();
        }

        protected override void CustomizeModelApplicationCreatorProperties(ModelApplicationCreatorProperties creatorProperties) {
            base.CustomizeModelApplicationCreatorProperties(creatorProperties);
            creatorProperties.RegisterObject(typeof(IModelRuleBase), typeof(RuleRequiredForAtLeast1Property), typeof(IRuleRequiredForAtLeast1PropertyProperties));
            creatorProperties.RegisterObject(typeof(IModelRuleBase), typeof(RuleFromIPropertyValueValidator), typeof(IRuleFromIPropertyValueValidatorProperties));
        }
    }
}
