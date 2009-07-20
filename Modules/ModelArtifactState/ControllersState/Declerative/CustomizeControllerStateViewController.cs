using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.Providers;
using eXpand.ExpressApp.Security.Calculators;

namespace eXpand.ExpressApp.ModelArtifactState.ControllersState.Declerative
{
    public partial class CustomizeControllerStateViewController : CustomizeControllerStateControllerBase
    {
        public CustomizeControllerStateViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string ActivationContext
        {
            get { return ModelArtifactState.ActivationContext.DeclarativeControllerActivation; }
        }

        protected override void ForceActivation()
        {
            StateCalculator.StateCalculated(this, (b, attribute) =>
                                                                {
                                                                    var controllerActivationRuleAttribute = (ControllerStateRuleAttribute)attribute;
                                                                    var provider = new ControllerProvider(controllerActivationRuleAttribute,this, controllerActivationRuleAttribute.ControllerType.FullName);
                                                                    foreach (var controller in provider.Find())
                                                                    {
                                                                        if (controller!= null)
                                                                            controller.Active[ActivationContext] = b;
                                                                    }
                                                                }, typeof(ControllerStateRuleAttribute));

        }
    }
}