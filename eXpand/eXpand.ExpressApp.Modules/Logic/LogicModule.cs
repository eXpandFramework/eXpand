using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.Logic {
    public class LogicModule:ExpressApp.ModuleBase {
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
//            extenders.Add<IModelApplication, IModelApplicationLogic>();
        }      
    }
}