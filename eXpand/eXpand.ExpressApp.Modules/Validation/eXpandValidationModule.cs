using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using eXpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using eXpand.Persistent.Base.Validation.FromIPropertyValueValidator;

namespace eXpand.ExpressApp.Validation
{
    public sealed partial class eXpandValidationModule : ModuleBase
    {
        public eXpandValidationModule()
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
