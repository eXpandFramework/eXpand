using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;
using eXpand.ExpressApp.Security.Calculators;

namespace eXpand.ExpressApp.ModelArtifactState.ControllersState.Views
{
    public partial class CustomizeControllerStateController : CustomizeControllerStateControllerBase
    {
        public CustomizeControllerStateController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string ActivationContext
        {
            get { return ModelArtifactState.ActivationContext.ViewsControllerActivation; }
        }

        protected override void ForceActivation()
        {
            DictionaryActivationCalculator.
                ActivationCalculated(this, null, BOModel.SyncInfoController.ConditionalControlllerState,
                                     (wrapperBase, b) => UpdateController(((DictionaryControllerStateNodeWrapper)wrapperBase).ControllerType, b), 
                                     node => new DictionaryControllerStateNodeWrapper(node,Application.Modules[0].ModuleManager.ControllersManager));

        }
        private void UpdateController(Type ControllerType, bool IsActive)
        {
            Controller theController;
            if (GetController(ControllerType, out theController,Frame))
                theController.Active[ActivationContext] = IsActive;
        }


    }
}