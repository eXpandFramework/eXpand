using CThru;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using TypeMock;

namespace eXpand.ExpressApp.WorldCreator.CThru
{
    public partial class RuleSetInitializationController : DevExpress.ExpressApp.Validation.RuleSetInitializationController
    {
        public RuleSetInitializationController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {
            if (Application != null) {
                var module = (ValidationModule) Application.Modules.FindModule(typeof (ValidationModule));
                if (module != null) {
                    CThruEngine.AddAspect(new ExistentMembersEnableValidationAspect());
                    CThruEngine.StartListening();
                    InternalMockManager.Locked = false;
                    module.RuleSetInitialized += (sender, args) => {
                        CThruEngine.StopListeningAndReset();
                        InternalMockManager.Locked = true;
                    };
                    module.InitializeRuleSet();
                }
                else {
                    Tracing.Tracer.LogWarning("Cannot find Validation module in Module list: {0}",typeof (ValidationModule).AssemblyQualifiedName);
                }
            }
        }
        
    }
}
