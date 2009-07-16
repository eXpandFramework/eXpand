using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.Security.Calculators;

namespace eXpand.ExpressApp.ModelArtifactState.ActionsState.Declerative
{
    public partial class CustomizeActionStateViewController : CustomizeStateControllerBase
    {
        public CustomizeActionStateViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string ActivationContext
        {
            get { return ModelArtifactState.ActivationContext.DeclarativeActionActivation; }
        }

        protected override void ForceActivation()
        {
            StateCalculator.ActivationCalculated(this, (b, attribute) =>
                                                                {
//                ((ActionStateRuleAttribute)attribute).Action.Active[ActivationContext] = b;
                                                                    ActionBase actionBase = GetAction(((ActionStateRuleAttribute)attribute).ActionId,Frame);
                                                                    actionBase.Active[ActivationContext] = b;
                                                                }, typeof(ActionStateRuleAttribute));

        }

    }
}