using CThru;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using TypeMock;

namespace eXpand.ExpressApp.WorldCreator.CThru
{
    public sealed class WorldCreatorCThruModule : ModuleBase
    {
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            
            var validationModule =                
                (ValidationModule) moduleManager.Modules.FindModule(typeof (ValidationModule));
            validationModule.CustomizeRegisteredRuleTypes += (o, eventArgs) => {
                CThruEngine.AddAspect(new ExistentMembersEnableValidationAspect());
                CThruEngine.StartListening();                
            };
            validationModule.RuleSetInitialized += (sender, args) => {
                CThruEngine.StopListeningAndReset();
                InternalMockManager.Locked = true;
            };            
        }
    }

}


