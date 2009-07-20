using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;
using eXpand.ExpressApp.Security.Calculators;

namespace eXpand.ExpressApp.ModelArtifactState.ActionsState.Views
{
    public partial class CustomizeActionStateController : CustomizeStateControllerBase
    {
        public CustomizeActionStateController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string ActivationContext
        {
            get { return ModelArtifactState.ActivationContext.ViewsActionActivation; }
            
        }

        protected override void ForceActivation()
        {
            DictionaryActivationCalculator.ActivationCalculated(this, null,BOModel.SyncInfoController.ConditionalActionState,
                                                                (wrapperBase, b) =>
                                                                    {
                                                                        var action = GetAction(((DictionaryActionStateNodeWrapper) wrapperBase).Action.Id,Frame);
                                                                        action.Active[ActivationContext] = b;
                                                                    },node => new DictionaryActionStateNodeWrapper(node,Application.Modules[0].ModuleManager.ControllersManager));

        }

    }
}